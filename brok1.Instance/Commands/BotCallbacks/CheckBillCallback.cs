using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Qiwi.BillPayments.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class CheckBillCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public CheckBillCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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
            string billId = splittedCallback[2];
            try
            {
                var bill = await QiwiManager.CheckBillAsync(billId);
                if (bill.Status.ValueEnum == BillStatusEnum.Paid)
                {
                    await bot.SendTextMessageAsync(user.userid, "Счет оплачен", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    await bot.DeleteMessageAsync(callback.Message.Chat.Id, callback.Message.MessageId);
                    await QiwiManager.SuccessfulPayAsync(bot, user);
                }
                else
                {
                    await bot.SendTextMessageAsync(user.userid, "Счет не оплачен", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                }
            }
            catch (Exception e)
            {
                string sad = e.ToString();
            }
        }
    }
}
