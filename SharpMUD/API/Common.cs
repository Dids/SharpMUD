namespace SharpMUD.API
{
    internal class Common
    {
        internal static string APIHost => "www.hackmud.com";

        internal static string CombineMethodLink(string endpoint) => $"https://{APIHost}/mobile/{endpoint}.json";
    }
}
