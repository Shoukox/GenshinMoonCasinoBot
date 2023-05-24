using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Commands.BotCommands
{
    public class AdminProcessedCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public AdminProcessedCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
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

            await bot.SendTextMessageAsync(message.Chat.Id, $"Обработано с момента запуска бота: {BotMessage.processedMessages}");
        }
    }
}
