namespace brok1.Instance.Types
{
    public class BotConfiguration
    {
        public string BotToken { get; set; } = string.Empty;
        public string QiwiToken { get; set; } = string.Empty;
        public string PublicQiwiToken { get; set; } = string.Empty;
        public string PrivateQiwiToken { get; set; } = string.Empty;
        public string HostAddress { get; set; } = string.Empty;
        public string Route => /*$"bot/{BotToken}"*/ $"";

        public static string Configuration = "BotConfiguration";
    }
}
