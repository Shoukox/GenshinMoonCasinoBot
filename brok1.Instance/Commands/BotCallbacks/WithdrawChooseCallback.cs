using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class WithdrawChooseCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public WithdrawChooseCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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
            string[] splittedCallback = callback.Data.Split(" ");
            string toWithdraw = splittedCallback[2];
            if (toWithdraw == "moon")
            {
                user.withdrawing = EWithdrawing.Moon;
            }
            else
            {
                user.withdrawing = EWithdrawing.Crystals;
            }
            if (user.moons == 0 && user.withdrawing == EWithdrawing.Moon)
            {
                user.stage = EStage.Other;
                sendText = localization.error_noMoons();
                await bot.SendMessage(callback.Message.Chat.Id, sendText, replyParameters: callback.Message.MessageId);
                return;
            }
            if (user.crystals == 0 && user.withdrawing == EWithdrawing.Crystals)
            {
                user.stage = EStage.Other;
                sendText = localization.error_noCrystals();
                await bot.SendMessage(callback.Message.Chat.Id, sendText, replyParameters: callback.Message.MessageId);
                return;
            }
            sendText = localization.button_moneyOut();
            user.stage = EStage.waitingForQIWINumber;
            await bot.SendMessage(callback.Message.Chat.Id, sendText, replyParameters: callback.Message.MessageId);
        }
    }
}
