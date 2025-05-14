using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class CheckFeedbacksCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public CheckFeedbacksCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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
            string sendText = "Посмотреть отзывы участников вы сможете <a href=\"https://t.me/+53z0tg2GH4JlYmMy\">здесь</a>";
            var rk = new InlineKeyboardMarkup(new InlineKeyboardButton("Посмотреть отзывы") { Url = "https://t.me/+53z0tg2GH4JlYmMy" });

            await bot.SendMessage(callback.Message.Chat.Id, sendText);
            await bot.AnswerCallbackQuery(callback.Id);
        }
    }
}
