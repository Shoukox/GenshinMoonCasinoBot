using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class ReplenishMoneyCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public ReplenishMoneyCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.callback = callback;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            string sendText = "";
            if (user.paydata.payStatus == EPayStatus.WaitingForPay)
            {
                if (user.paydata.billResponse == null)
                {
                    sendText = localization.error_restartBot();
                    await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    return;
                }

                await QiwiManager.CancelBillAsync(user.paydata.billResponse.BillId);
                user.paydata = new PayData();
                sendText = localization.money_billCanceled();
                await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            user.stage = EStage.moneyAddProcessing;
            user.paydata.payStatus = EPayStatus.Started;
            sendText = localization.button_moneyAdd();
            var ik = new InlineKeyboardMarkup(
                        new InlineKeyboardButton[]
                        {
                                        new InlineKeyboardButton("Продолжить") { CallbackData=$"{user.userid} moneyAdd yes"},
                        });
            await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, replyToMessageId: callback.Message.MessageId, replyMarkup: ik);
        }
    }
}
