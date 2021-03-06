﻿using System;
using System.Threading;

namespace SharpMUD.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client(/*token here*/);
            client.ChatHistoryReceived += Client_ChatHistoryReceived;
            client.MessageReceived += Client_MessageReceived;
            client.MessageSent += Client_MessageSent;

            client.RequestAccountData();
            foreach(var user in client.AccountData.Users)
            {
                if(user.Name != "vdd")
                    user.Update = false;
            }
            client.RequestMessageHistory();

            Thread PollThread = new Thread(() => client.StartPolling(500));
            PollThread.Start();
        }

        private static void Client_MessageSent(object sender, Events.MessageEventArgs e)
        {
            var mesg = e.Message;
            Console.WriteLine($"{mesg.Channel} :: {mesg.From}: {mesg.Body}");
        }

        private static void Client_MessageReceived(object sender, Events.MessageEventArgs e)
        {
            var mesg = e.Message;
            Console.WriteLine($"{mesg.Channel} :: {mesg.From}: {mesg.Body}");
        }


        private static void Client_ChatHistoryReceived(object sender, Events.ChatHistoryEventArgs e)
        {
            /*while(!e.History.IsEmpty)
            {
                var msg = e.History.Pop();
                if(msg is Presence)
                {
                    var pres = msg as Presence;
                    Console.WriteLine($"{pres.Timestamp} | ${pres.Channel} :: ${pres.From} has ${(pres.Joined ? "joined" : "left")} the channel.");
                }
                else if(msg is Message)
                {
                    var mesg = msg as Message;
                    Console.WriteLine($"{mesg.Timestamp} | ${mesg.Channel} :: ${mesg.From} > ${mesg.Body}");
                }
            }*/
        }
    }
}
