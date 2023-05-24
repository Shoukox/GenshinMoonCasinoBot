namespace brok1.Instance.Localization
{
    public interface ILocalization
    {
        public string command_start();
        public string button_balance();
        public string button_roulette();
        public string button_help();
        public string button_info();
        public string button_moneyAdd();
        public string button_moneyOut();
        public string button_referal();
        public string button_referal_info();
        public string button_feedback();

        public string money_billCreated();
        public string money_billInfo();
        public string money_billCanceled();

        public string roulette_win();
        public string roulette_lose();
        public string roulette_limit();

        public string shop_item();
        public string notify_Text();

        public string notifyAdminAboutUserWantsToPay();
        public string notifyAdminAboutUserWantsToPayConfirmation();

        public string error_restartBot();
        public string error_noMoons();
        public string error_noCrystals();
        public string error_commandNotFound();
    }
}
