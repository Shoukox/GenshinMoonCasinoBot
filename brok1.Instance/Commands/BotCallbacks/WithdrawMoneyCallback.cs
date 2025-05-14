using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class WithdrawMoneyCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public WithdrawMoneyCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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
            user.stage = EStage.chooseMoonOrCrystals;
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                    new("Луну") { CallbackData = $"{callback.From.Id} withdrawChoose moon" },
                    new("Кристаллы") { CallbackData = $"{callback.From.Id} withdrawChoose crystals" },
            });
            sendText = "Что вы хотите вывести?";
            await bot.SendMessage(callback.Message.Chat.Id, sendText, replyParameters: callback.Message.MessageId, replyMarkup: ik);
        }
    }
}
