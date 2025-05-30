namespace brok1.Instance.Types.Utils
{
    public static class Settings
    {
        public static void LoadAllSettings(BotConfiguration bot)
        {
            InitializeQiwi(bot);
            LoadData();
            StartSaveTimer();
        }
        private static void InitializeQiwi(BotConfiguration bot)
        {
            QiwiClass.qiwi = Qiwi.BillPayments.Client.BillPaymentsClientFactory.Create(bot.PrivateQiwiToken, objectMapper: new Qiwi.BillPayments.Json.Newtonsoft.NewtonsoftMapper());
        }
        private static void LoadData()
        {
            TextDatabase.LoadData();
        }
        private static void StartSaveTimer()
        {
            TextDatabase.SaveTimer();
        }
    }
}
