using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Types
{
    public class NotifyMessage
    {
        public Telegram.Bot.Types.Message message;
        public Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup? ik;
        public string text;
        public bool isForwarding;
        public bool isMessage;

        public NotifyMessage(Telegram.Bot.Types.Message message, bool isForwarding = true)
        {
            this.message = message;
            this.isForwarding = isForwarding;
            this.isMessage = true;
        }

        public NotifyMessage(string text, InlineKeyboardMarkup? ik = null, bool isForwarding = false)
        {
            this.text = text;
            this.isForwarding = isForwarding;
            this.isMessage = false;
            this.ik = ik;
        }
    }
}
