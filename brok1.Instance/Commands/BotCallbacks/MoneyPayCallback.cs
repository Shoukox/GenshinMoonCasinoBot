using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class MoneyPayCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public MoneyPayCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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

            await bot.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId);
            await bot.SendMessage(callback.Message!.Chat.Id, "В разработке...");
            //if (user.paydata.payStatus == EPayStatus.WaitingForConfirmation)
            //{
            //    user.stage = EStage.moneyAddAnsweredYes;
            //    user.paydata.payStatus = EPayStatus.WaitingForPay;
            //    string sendText = localization.money_billCreated().ReplaceLocals(new[] { $"{user.paydata.billResponse.Amount.ValueString}" });
            //    var ik = new InlineKeyboardMarkup(
            //        new InlineKeyboardButton[][]
            //            {
            //                new InlineKeyboardButton[] { new InlineKeyboardButton("Оплатить счет") {Url=$"{user.paydata.billResponse.PayUrl.AbsoluteUri}"} }, new InlineKeyboardButton[] { new InlineKeyboardButton("Проверить оплату") { CallbackData=$"{callback.From.Id} checkbill {user.paydata.billResponse.BillId}"} }
            //          }
            //        );
            //    await bot.SendMessage(user.userid, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: ik);
            //}
        }
    }
}
