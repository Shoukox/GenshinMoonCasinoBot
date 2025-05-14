using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace brok1.Instance.Types.Utils
{
    public static class NotifyManager
    {
        public static async Task NotifyAsync(ITelegramBotClient bot, NotifyMessage notifyMessage, ENotify eNotify)
        {
            List<BotUser> usersToNotify = eNotify switch
            {
                ENotify.Users => BotUser.AllUsers,
                ENotify.Admins => BotUser.AllUsers.Where((user) => BotUser.ADMINS.Contains(user.userid)).ToList(),
                _ => throw new NotSupportedException()
            };
            for (int i = 0; i <= usersToNotify.Count - 1; i++)
            {
                var curUser = usersToNotify[i];

                try
                {
                    if (notifyMessage.isForwarding)
                        await bot.ForwardMessage(curUser.userid, notifyMessage.message.Chat.Id, notifyMessage.message.MessageId);
                    else
                    {
                        if (notifyMessage.isMessage)
                            await bot.CopyMessage(curUser.userid, notifyMessage.message.Chat.Id, notifyMessage.message.MessageId, replyMarkup: notifyMessage.ik, parseMode: ParseMode.Html);
                        else
                            await bot.SendMessage(curUser.userid, notifyMessage.text, parseMode: ParseMode.Html, replyMarkup: notifyMessage.ik);

                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("blocked"))
                    {
                        curUser.stoppedBot = true;
                    }
                    Console.WriteLine($"sending to {curUser.userid}: error({e.InnerException} {e.Message})");
                }

                await Task.Delay(2000);
            }
        }
    }
}
