using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace brok1.Instance.Commands.BotCommands
{
    public class TestCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public TestCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
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

            if(message.Text == "/test users")
            {
                using var sr = new StreamReader("usersIds.txt");
                string[] text = (await sr.ReadToEndAsync()).Split("\n");
                BotUser.AllUsers = text.Select(m => new BotUser() {userid = long.Parse(m), freeSpinsUsedAfterWin = Random.Shared.Next(1, 140) }).ToList();
            }
            if(message.Text == "/test chance")
            {
                for(int i = 0; i<=BotUser.AllUsers.Count-1; i++)
                {
                    BotUser.AllUsers[i].freeSpinsUsedAfterWin = Random.Shared.Next(90, 120);
                }
            }
            if(message.Text == "/test usersfix") {
                for (int i = 0; i <= BotUser.AllUsers.Count - 1; i++)
                {
                    BotUser.AllUsers[i].freeSpinsUsedAfterWin = Random.Shared.Next(90, 120);
                    BotUser.AllUsers[i].username = "";
                    BotUser.AllUsers[i].balance = 0;
                    BotUser.AllUsers[i].moneyadded = 0;
                    BotUser.AllUsers[i].moneyused = 0;
                    BotUser.AllUsers[i].spins = 0;
                }
            }

            user.lastFreeSpin = DateTime.Now.AddDays(-1);
        }
    }
}
