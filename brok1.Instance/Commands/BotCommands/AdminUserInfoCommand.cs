using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCommands
{
    public class AdminUserInfoCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public AdminUserInfoCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.message = message;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public async Task Execute()
        {
            if (!BotUser.ADMINS.Contains(message.From!.Id))
                return;

            long id = -1;
            if (!long.TryParse(message.Text!.Split(" ").Last(), out id))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Это не id");
                return;
            }

            BotUser? userToFind = BotUser.AllUsers.FirstOrDefault(m => m.userid == id);

            if (userToFind == default)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Такого нету");
                return;
            }

            string sendText = Newtonsoft.Json.JsonConvert.SerializeObject(userToFind);
            await bot.SendTextMessageAsync(message.Chat.Id, sendText);
        }
    }
}
