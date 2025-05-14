using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCallbacks
{
    public class EditRandomCallback : ICommand<CallbackQuery>
    {
        private readonly ITelegramBotClient bot;
        private readonly CallbackQuery callback;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public EditRandomCallback(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct, ILocalization localization, BotUser user)
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
            long id = long.Parse(splittedCallback[0]);

            await bot.DeleteMessage(callback.Message.Chat.Id, callback.Message.MessageId);
            if (answer == "yes")
            {
                var user1 = BotUser.AllUsers.FirstOrDefault(m => m.userid == id);
                user1.pseudorandom.EditChance(100);

                NotifyMessage notify = new NotifyMessage($"Он выиграет в следующей крутке:\n" +
                    $"adminId: {callback.From.Id}\n" +
                    $"adminUserName: @{callback.From.Username}\n\n" +
                    $"UserId: {user1.userid}\n" +
                    $"UserName: {user1.username}\n" +
                    $"<a href=\"tg://user?id={user1.userid}\">Ссылка</a>\n");
                _ = NotifyManager.NotifyAsync(bot, notify, Types.Enums.ENotify.Admins);
            }
        }
    }
}
