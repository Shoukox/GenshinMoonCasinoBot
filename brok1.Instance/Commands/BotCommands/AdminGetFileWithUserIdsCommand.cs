using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCommands
{
    public class AdminGetFileWithUserIdsCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public AdminGetFileWithUserIdsCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.message = message;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            string name = $"{DateTime.Now:ddmmyyyyssffff}";
            string text = string.Join("\n", BotUser.AllUsers.Select(m => m.userid));
            using (StreamWriter sw = new StreamWriter($"usersIds_{name}.txt"))
            {
                sw.Write(text);
            }
            using var stream1 = System.IO.File.OpenRead($"usersIds_{name}.txt");
            InputFile inputOnlineFile = new InputFileStream(stream1, "usersIds.txt");
            await bot.SendDocumentAsync(message.Chat.Id, inputOnlineFile);
        }
    }
}
