using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class NotifyAllUsersCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public NotifyAllUsersCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.callback = callback;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            if (!BotUser.ADMINS.Contains(callback.From.Id))
                return;

            string[] splittedCallback = callback.Data.Split(" ");
            string answer = splittedCallback[2];

            await bot.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId);
            if (answer == "yes")
            {
                NotifyMessage notify = new NotifyMessage(user.adminMessageToSend, false);
                notify.ik = user.adminIkToSend;

                _ = NotifyManager.NotifyAsync(bot, notify, Types.Enums.ENotify.Users);
                await bot.SendMessage(user.userid, $"Начинается рассылка сообщения всем юзерам. Рассылка займет примерно {BotUser.AllUsers.Count} секунд.", replyMarkup: null);

                user.adminPanel.stage = EAdminPanelStage.Other;
                user.adminMessageToSend = null;
                user.adminIkToSend = null;
            }
        }
    }
}
