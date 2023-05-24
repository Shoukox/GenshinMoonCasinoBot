using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class MoonOutCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public MoonOutCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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

            bool isAllDigit = true;
            string text = callback.Message.Text.Split(":")[1].Replace(" ", "");
            foreach (char chr in text)
            {
                if (!char.IsDigit(chr))
                    isAllDigit = false;
            }

            if (answer == "yes" && isAllDigit && user.stage == EStage.waitingQiwiNumberConfirmation)
            {
                user.stage = EStage.Other;

                string sendText = "";
                string helpText = "";
                int crystalsWas = user.crystals;
                if (user.withdrawing == EWithdrawing.Moon)
                {
                    helpText = "1 Луны";
                    user.moons -= 1;
                    sendText = "С вашего аккаунта была списана одна луна.\nОна зачислится в течение суток.\n";
                }
                else
                {
                    helpText = $"{crystalsWas} кристаллов";
                    user.crystals = 0;
                    sendText = $"С вашего аккаунта были списаны {crystalsWas} кристаллов.\nМы начислим их вам в течение суток.";
                }
                NotifyMessage notify = new NotifyMessage($"Запрос на вывод {helpText}.\nUserId: {user.userid}\nUserName: {user.username}\nСообщения с кошельком:\n\n{callback.Message.Text}");

                _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);
                await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            else
            {
                user.stage = EStage.waitingForQIWINumber;
                string sendText = "Повторите ввод. Если вы считаете, что это ошибка, перезапустите бота - /start";
                await bot.SendTextMessageAsync(callback.Message.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            //Variables.db.UpdateOrInsertUsersTable(user, false);
        }
    }
}
