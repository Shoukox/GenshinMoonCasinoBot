using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCommands
{
    public class StartCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public StartCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.message = message;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            UsersManager.ClearUsersFlags(user);

            ReplyKeyboardMarkup rk;
            if (BotUser.ADMINS.Contains(message.Chat.Id))
            {
                rk = Keyboards.adminStartButtons;
            }
            else
            {
                rk = Keyboards.startButtons;
            }
            string sendText = localization.command_start();
            string[] splittedMessage = message.Text.Split(" ");
            await bot.SendMessage(message.Chat.Id, sendText, replyParameters: message.MessageId, replyMarkup: rk, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

            if (splittedMessage.Length == 2)
            {
                long id = -1;
                if (!long.TryParse(splittedMessage[1], out id)
                    || !user.wasRecentlyAdded)
                    return;

                user.wasRecentlyAdded = false;

                BotUser user1 = BotUser.AllUsers.First(m => m.userid == id);
                user1.referalUsersCount += 1;

                if (DateTime.UtcNow.Day - user1.lastInvitedReferal.Day >= 1 ||
                    DateTime.UtcNow.Month - user1.lastInvitedReferal.Month >= 1 ||
                    DateTime.UtcNow.Year - user1.lastInvitedReferal.Year >= 1)
                {
                    user1.spinsList.Add(new Spin() { amount = 0, count = 1, native_count = 1 });
                }

                sendText =
                    "Благодаря вашей реферальной ссылке в бота добавили нового пользователя!\n" +
                    $"Всего перешедших: {user1.referalUsersCount}";
                await bot.SendMessage(user1.userid, sendText);

                if (user1.referalUsersCount / 6f - user1.referalUsersCount / 6 != 0
                    || user1.stoppedBot)
                    return;

                user1.balance += 10;
                user1.lastInvitedReferal = DateTime.UtcNow;

                sendText = $"Ваша реферальная ссылка привела в бота уже {user1.referalUsersCount} человек. Вы получили бонусные 10 рублей на баланс.";
                await bot.SendMessage(user1.userid, sendText);
            }
        }
    }
}
