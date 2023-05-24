using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            var ik = new InlineKeyboardMarkup
              (
                  new InlineKeyboardButton[]
                  {
                                            new InlineKeyboardButton("Да"){CallbackData=$"{user.userid} editrandom yes"},
                                            new InlineKeyboardButton("Нет"){CallbackData=$"{user.userid} editrandom no"}
                  }
              );
            NotifyMessage notify = new NotifyMessage(
                "test" + $"\n\nUserId: {user.userid}\n" +
                $"UserName: {user.username}\n" +
                $"<a href=\"tg://user?id={user.userid}\">Ссылка</a>\n" +
                $"Всего денег закинул: {user.moneyadded}\n" +
                $"Всего потратил: {user.moneyused}\n" +
                $"Его баланс: {user.balance}\n" +
                $"Всего круток сделано: {user.pseudorandom.success + user.pseudorandom.loss}", ik);
            _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);

            notify = new NotifyMessage(await bot.SendTextMessageAsync(message.From.Id, "1"));
            _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);

            notify = new NotifyMessage(await bot.SendTextMessageAsync(message.From.Id, "2"));
            _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);

            user.lastFreeSpin = DateTime.Now.AddDays(-1);
            user.wasNotified = false;
        }
    }
}
