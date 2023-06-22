using brok1.Instance.Localization;
using brok1.Instance.Types;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class ReferalCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public ReferalCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.callback = callback;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            string sendText = localization.button_referal().ReplaceLocals(new string[] {$"{BotInfo.bot.Username}", $"{user.userid}", $"{user.referalUsersCount}"});
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Доп.информация 📬") { CallbackData = $"{callback.Message.Chat.Id} referalInfo" });
            await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableWebPagePreview: true, replyMarkup: ik);
        }
    }
}
