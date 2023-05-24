using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCommands
{
    public class AdminChanceCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public AdminChanceCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.message = message;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            if (!BotUser.ADMINS.Contains(message.From.Id))
                return;

            string[] splittedText = message.Text.Split(" ");
            double n = double.Parse(splittedText[2]);
            var user1 = BotUser.AllUsers.First(m => m.username.ToLower() == splittedText[1].ToLower());
            UsersManager.SetFreeSpinsUsedAfterWin(user1, (int)(n * 10 - 10));

            await bot.SendTextMessageAsync(message.Chat.Id, $"Изменено, его шанс {UsersManager.GetCurrentUsersVisualChance(user1)[1]}%");
        }
    }
}
