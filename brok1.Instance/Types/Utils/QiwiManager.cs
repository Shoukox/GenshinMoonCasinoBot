using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Types.Utils
{
    public static class QiwiManager
    {
        public static Task<BillResponse> CreateBillAsync(int amount)
        {
            Console.WriteLine("starting creating qiwi");
            var createBillInfo = new CreateBillInfo()
            {
                Amount = new MoneyAmount
                {
                    CurrencyEnum = CurrencyEnum.Rub,
                    ValueDecimal = Convert.ToDecimal(amount),
                },
                BillId = Guid.NewGuid().ToString(),
                Comment = $"Пополнение баланса в боте @Luna_VadimchikaBot",
                ExpirationDateTime = DateTime.Now.AddMinutes(25),
            };
            return QiwiClass.qiwi.CreateBillAsync(createBillInfo);
        }
        public static Task<BillResponse> CheckBillAsync(string billId) => QiwiClass.qiwi.GetBillInfoAsync(billId);
        public static Task<BillResponse> CancelBillAsync(string billId) => QiwiClass.qiwi.CancelBillAsync(billId);
        public static Task SuccessfulPayAsync(ITelegramBotClient botClient, BotUser user)
        {
            user.balance += user.paydata.payAmount;
            user.moneyadded += user.paydata.payAmount;

            string sendText = $"Благодарим вас за платеж! На ваш баланс было успешно перечислено {user.paydata.payAmount} рублей.";
            NotifyMessage notify = new NotifyMessage(sendText +
                $"\n\nUserId: {user.userid}\n" +
                $"UserName: {user.username}\n" +
                $"<a href=\"tg://user?id={user.userid}\">Ссылка</a>\n" +
                $"Всего денег закинул: {user.moneyadded}\n" +
                $"Всего потратил: {user.moneyused}\n" +
                $"Его баланс: {user.balance}\n" +
                $"Всего круток сделано: {user.pseudorandom.success + user.pseudorandom.loss}",
                new InlineKeyboardMarkup(
                       new InlineKeyboardButton[]
                       {
                                                new InlineKeyboardButton("Да"){CallbackData=$"{user.userid} editrandom yes"},
                                                new InlineKeyboardButton("Нет"){CallbackData=$"{user.userid} editrandom no"}
                       }));

            user.paydata = new PayData();

            return Task.WhenAll(new Task[]
                {
                    NotifyManager.NotifyAsync(botClient, notify, Enums.ENotify.Admins),
                    botClient.SendTextMessageAsync(user.userid, sendText)
                });
        }
    }
}
