using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpMUD.API;
using SharpMUD.Events;
using SharpMUD.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SharpMUD
{
    public class Client
    {
        private string _selectedUser;

        private int PollingInterval { get; set; }
        private double LastPollTime { get; set; }
        private Timer PollingTimer { get; set; }
        private List<string> RelayedIDs { get; set; }

        public string Password { get; set; }
        public string Token { get; private set; }
        public AccountData AccountData { get; private set; }

        public string SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if(!AccountData.Users.Exists(x => x.Name == value))
                    throw new InvalidOperationException("The selected user does not exist in the account data.");

                _selectedUser = value;
            }
        }

        public bool IsAuthenticated => (!string.IsNullOrEmpty(Token));

        public event EventHandler<ErrorEventArgs> ErrorOccured;
        public event EventHandler<AuthenticationEventArgs> Authenticated;
        public event EventHandler<AccountDataEventArgs> AccountDataReceived;
        public event EventHandler<ChatHistoryEventArgs> ChatHistoryReceived;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<PresenceEventArgs> PresenceReceived;
        public event EventHandler<MessageEventArgs> MessageSent;            

        public Client()
        {
            PollingInterval = 800;
            LastPollTime = Utilities.GetTime();

            PollingTimer = new Timer(PollingInterval);
            PollingTimer.Elapsed += PollingTimer_Elapsed;

            RelayedIDs = new List<string>();
        }

        public Client(string token) : this()
        {
            Token = token;
        }

        public string RequestToken(string password)
        {
            if(string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Cannot request a token without a password provided.");

            if(IsAuthenticated)
                throw new InvalidOperationException("Already authenticated.");

            var jObject = new JObject();
            jObject["pass"] = password ?? Password;

            try
            {
                var response = Requester.PostRequest(Methods.GetToken, jObject);
                var obj = JsonConvert.DeserializeObject(response) as JObject;

                var token = obj["chat_token"].ToString();
                var eventArgs = new AuthenticationEventArgs(token);

                Token = token;
                Authenticated?.Invoke(this, eventArgs);
                return token;
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
                return string.Empty;
            }
        }

        public void RequestAccountData()
        {
            if(!IsAuthenticated)
                throw new InvalidOperationException("Not authenticated. Request/set token first.");

            var jObject = new JObject();
            jObject["chat_token"] = Token;

            try
            {
                var response = Requester.PostRequest(Methods.AccountData, jObject);
                var obj = JsonConvert.DeserializeObject(response) as JObject;

                var accountData = new AccountData();

                var usersObj = obj["users"] as JObject;
                foreach(var kvp in usersObj)
                {
                    var usr = new User(kvp.Key);
                    foreach(var kvp2 in kvp.Value as JObject)
                    {
                        var chn = new Channel(kvp2.Key);
                        foreach(var user in kvp2.Value as JArray)
                        {
                            chn.Members.Add(user.ToString());
                        }
                    }
                    accountData.Users.Add(usr);
                }
                AccountData = accountData;
                SelectedUser = accountData.Users.First().Name;

                AccountDataReceived?.Invoke(this, new AccountDataEventArgs(accountData));
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public void RequestMessageHistory()
        {
            if(!IsAuthenticated)
                throw new InvalidOperationException("Not authenticated. Request/set token first.");

            if(AccountData == null)
                throw new InvalidOperationException("No account data requested. Request account data first.");

            var jObject = new JObject();
            jObject["chat_token"] = Token;
            jObject["usernames"] = JArray.FromObject(AccountData.Users.Where(x => x.Update).Select(x => x.Name).ToArray());

            try
            {
                var response = Requester.PostRequest(Methods.ChatHistory, jObject);
                var obj = JsonConvert.DeserializeObject(response) as JObject;

                var chatHistory = new ChatHistory();

                var chatObject = obj["chats"] as JObject;
                foreach(var kvp in chatObject)
                {
                    foreach(JObject msgObject in kvp.Value as JArray)
                    {
                        var channel = string.Empty;
                        if(msgObject["channel"] != null)
                        {
                            channel = msgObject["channel"].ToString();
                        }

                        if(msgObject["is_join"] != null)
                        {
                            var presence = new Presence(
                                msgObject["id"].ToString(),
                                double.Parse(msgObject["t"].ToString()),
                                kvp.Key,
                                msgObject["from_user"].ToString(),
                                channel,
                                bool.Parse(msgObject["is_join"].ToString())
                            );
                            chatHistory.Push(presence);
                        }
                        else
                        {
                            var message = new Message(
                                msgObject["id"].ToString(),
                                double.Parse(msgObject["t"].ToString()),
                                kvp.Key,
                                msgObject["from_user"].ToString(),
                                channel,
                                msgObject["msg"].ToString()
                            );
                            chatHistory.Push(message);
                        }
                    }
                }
                LastPollTime = chatHistory.Last.Timestamp;
                ChatHistoryReceived?.Invoke(this, new ChatHistoryEventArgs(chatHistory));
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public void StartPolling()
        {
            if(!IsAuthenticated)
                throw new InvalidOperationException("Not authenticated. Request/set token first.");

            if(AccountData == null)
                throw new InvalidOperationException("No account data requested. Request account data first.");

            PollingTimer.Start();
        }

        public void SendMessage(string channel, string message)
        {
            if(!IsAuthenticated)
                throw new InvalidOperationException("Not authenticated. Request/set token first.");

            if(AccountData == null)
                throw new InvalidOperationException("No account data requested. Request account data first.");

            try
            {
                var jObject = new JObject();
                jObject["chat_token"] = Token;
                jObject["username"] = SelectedUser;
                jObject["channel"] = channel;
                jObject["msg"] = message;

                Requester.PostRequest(Methods.MessageOutlet, jObject);
                MessageSent?.Invoke(this, new MessageEventArgs(new Message(string.Empty, Utilities.GetTime(), string.Empty, SelectedUser, channel, message)));
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public void TellMessage(string user, string message)
        {
            if(!IsAuthenticated)
                throw new InvalidOperationException("Not authenticated. Request/set token first.");

            if(AccountData == null)
                throw new InvalidOperationException("No account data requested. Request account data first.");

            try
            {
                var jObject = new JObject();
                jObject["chat_token"] = Token;
                jObject["username"] = SelectedUser;
                jObject["tell"] = user;
                jObject["msg"] = message;

                Requester.PostRequest(Methods.MessageOutlet, jObject);
                MessageSent?.Invoke(this, new MessageEventArgs(new Message(string.Empty, Utilities.GetTime(), user, SelectedUser, string.Empty, message)));
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        private void Poll()
        {
            var jObject = new JObject();
            jObject["chat_token"] = Token;
            jObject["usernames"] = JArray.FromObject(AccountData.Users.Where(x => x.Update).Select(x => x.Name).ToArray());
            jObject["after"] = LastPollTime + 0.005;

            try
            {
                var response = Requester.PostRequest(Methods.ChatHistory, jObject);
                var obj = JsonConvert.DeserializeObject(response) as JObject;

                var chatHistory = new ChatHistory();
                var chatObject = obj["chats"] as JObject;
                foreach(var kvp in chatObject)
                {
                    foreach(JObject msgObject in kvp.Value as JArray)
                    {
                        var channel = string.Empty;
                        if(msgObject["channel"] != null)
                        {
                            channel = msgObject["channel"].ToString();
                        }

                        if(msgObject["is_join"] != null)
                        {
                            var presence = new Presence(
                                msgObject["id"].ToString(),
                                double.Parse(msgObject["t"].ToString()),
                                kvp.Key,
                                msgObject["from_user"].ToString(),
                                channel,
                                bool.Parse(msgObject["is_join"].ToString())
                            );
                            chatHistory.Push(presence);
                        }
                        else
                        {
                            var message = new Message(
                                msgObject["id"].ToString(),
                                double.Parse(msgObject["t"].ToString()),
                                kvp.Key,
                                msgObject["from_user"].ToString(),
                                channel,
                                msgObject["msg"].ToString()
                            );
                            chatHistory.Push(message);
                        }
                    }
                }

                if(!chatHistory.IsEmpty)
                    LastPollTime = chatHistory.Last.Timestamp;

                while(!chatHistory.IsEmpty)
                {
                    var element = chatHistory.Pop();

                    if(element is Presence)
                    {
                        var pres = element as Presence;
                        var channel = AccountData.FindUser(pres.To).FindChannel(pres.Channel);

                        if(channel != null)
                        {
                            if(!pres.Joined)
                            {
                                if(channel.MemberExists(pres.From))
                                {
                                    channel.Members.Remove(pres.From);
                                }
                            }
                            else
                            {
                                if(!channel.MemberExists(pres.From))
                                {
                                    channel.Members.Add(pres.From);
                                }
                            }
                        }
                        PresenceReceived?.Invoke(this, new PresenceEventArgs(pres));
                        continue;
                    }
                    else if(element is Message)
                    {
                        if(element.From != element.To)
                        {
                            if(!RelayedIDs.Contains(element.ID))
                            {
                                RelayedIDs.Add(element.ID);
                                MessageReceived?.Invoke(this, new MessageEventArgs(element as Message));
                            }
                        }
                        else
                        {
                            if(!RelayedIDs.Contains(element.ID))
                            {
                                RelayedIDs.Add(element.ID);
                                MessageSent?.Invoke(this, new MessageEventArgs(element as Message));
                            }
                        }


                        if(RelayedIDs.Count > 100)
                        {
                            RelayedIDs.Clear();
                        }
                    }
                }         
            }
            catch(RequestException e)
            {
                ErrorOccured?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        private void PollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Poll();
        }
    }
}
