using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class ReferalInfoCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public ReferalInfoCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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

            string answer = splittedCallback[2];
            await bot.DeleteMessageAsync(callback.Message.Chat.Id, callback.Message.MessageId);
            if (answer == "yes")
            {
                user.stage = EStage.moneyAddAnsweredYes;
                user.paydata.payStatus = EPayStatus.WaitingForAmount;
                string sendText = "💳 На какую сумму вы хотите пополнить баланс?";
                await bot.SendTextMessageAsync(user.userid, sendText);
            }
        }
    }
}
