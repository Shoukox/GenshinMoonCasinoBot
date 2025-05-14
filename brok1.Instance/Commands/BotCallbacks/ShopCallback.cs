using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class ShopCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public ShopCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.callback = callback;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            string[] splittedCallback = callback.Data.Split(" ");
            string[] item = splittedCallback[2].Split("_");
            int count = int.Parse(item[0]);
            int amount = int.Parse(item[1]);

            if (user.balance < amount)
            {
                string sendText = "Недостаточно средств на балансе";
                await bot.SendMessage(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                return;
            }
            user.balance -= amount;
            user.moneyused += amount;
            user.spins += count;
            user.spinsList.Add(new Spin()
            {
                amount = amount,
                count = count,
                native_count = count
            });
            //Variables.db.UpdateOrInsertUsersTable(user, false);
            await bot.SendMessage(callback.Message.Chat.Id, $"Вы успешно купили за <b>{amount}</b> рублей следующее количество круток: <b>{count}</b>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}
