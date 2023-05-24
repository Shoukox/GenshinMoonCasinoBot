using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Qiwi.BillPayments.Model.Out;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCommands.Handlers
{
    public static class ChangedStatusHandler
    {
        public static async Task isWishing(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            NotifyMessage notifyMessage = new NotifyMessage($"Пожелание.\nUserId: {user.userid}\nUserName: {user.username}\n\n");
            _ = NotifyManager.NotifyAsync(bot, notifyMessage, Types.Enums.ENotify.Admins);

            notifyMessage = new NotifyMessage(msg);
            _ = NotifyManager.NotifyAsync(bot, notifyMessage, Types.Enums.ENotify.Admins);

            await bot.SendTextMessageAsync(msg.Chat.Id, "Спасибо что оставили пожелание по улучшению бота!");
            user.isWishing = false;
        }
        public static async Task isFeedbacking(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            user.isFeedbacking = false;
            NotifyMessage notify = new NotifyMessage($"Отзыв.\nUserId: {user.userid}\nUserName: {user.username}\n\n");
            _ = NotifyManager.NotifyAsync(bot, notify, Types.Enums.ENotify.Admins);

            notify = new NotifyMessage(msg);
            _ = NotifyManager.NotifyAsync(bot, notify, Types.Enums.ENotify.Admins);

            await bot.SendTextMessageAsync(msg.Chat.Id, "Спасибо, что оставили отзыв!");
        }
        public static async Task WaitingForStringToEditSponsors(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string[] splittedMessage = msg.Text.Split("\n");

            var list = new List<Sponsor>();

            for (int i = 0; i <= splittedMessage.Length - 1; i++)
            {
                string[] splittedStr = splittedMessage[i].Split(" ");
                list.Add(new Sponsor() { channelId = long.Parse(splittedStr[0]), channelLink = splittedStr[1] });
            }
            Sponsor.AllSponsors = list;
            user.adminPanel.stage = EAdminPanelStage.Other;

            string sendText = "Успешно изменено";
            await bot.SendTextMessageAsync(user.userid, sendText);
        }
        public static async Task WaitingForButtonText(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From!.Id))
                return;


            string[] splittedByTwoChars = msg.Text.Split("||");
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i <= splittedByTwoChars.Length - 1; i++)
            {
                buttons.Add(new List<InlineKeyboardButton>());
                string[] splittedByOneChar = splittedByTwoChars[i].Split("|");
                foreach (var str in splittedByOneChar)
                {
                    string[] splittedString = str.Split(" - ");
                    string buttonName = splittedString[0];
                    string buttonLink = splittedString[1];

                    buttons[i].Add(new InlineKeyboardButton(buttonName) { Url = buttonLink });
                }
            }

            InlineKeyboardMarkup ik = new InlineKeyboardMarkup(buttons);
            user.adminIkToSend = ik;

            ik = new InlineKeyboardMarkup
                       (
                           new InlineKeyboardButton[]
                           {
                                    new InlineKeyboardButton("Да") {CallbackData = $"{user.userid} notifyAllUsers yes"},
                                    new InlineKeyboardButton("Нет") {CallbackData = $"{user.userid} notifyAllUsers no"}
                           }
                       );

            var text = $"\n\nОтправляем?";
            await bot.CopyMessageAsync(msg.Chat.Id, user.adminMessageToSend.Chat.Id, user.adminMessageToSend.MessageId, replyMarkup: user.adminIkToSend);
            await bot.SendTextMessageAsync(user.userid, text, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task WaitingForStringToNotifyAllUsers(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForConfirmationToNotifyAllUsers;
            user.adminMessageToSend = msg;
            //var ik = new InlineKeyboardMarkup
            //    (
            //        new InlineKeyboardButton[]
            //        {
            //            new InlineKeyboardButton("Да") {CallbackData = $"{user.userid} notifyAllUsers yes"},
            //            new InlineKeyboardButton("Нет") {CallbackData = $"{user.userid} notifyAllUsers no"}
            //        }
            //    );
            var ik = new ReplyKeyboardMarkup
                (
                    new KeyboardButton[][] {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Добавить кнопки"),
                                new KeyboardButton("Без кнопок")
                            },
                              new KeyboardButton[]
                            {
                                new KeyboardButton("Назад"),
                            }
                    }
                )
            { ResizeKeyboard = true };
            string text = "\n\nХотите добавить кнопки?";
            await bot.CopyMessageAsync(msg.Chat.Id, msg.Chat.Id, msg.MessageId);
            await bot.SendTextMessageAsync(user.userid, text, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task WaitingForNum(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            double num = 0;
            bool condition = double.TryParse(msg.Text, out num);

            string sendText = "";
            if (!condition)
            {
                sendText = "Вы ввели не число. Введите число без лишних надписей. Только цифры";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                return;
            }
            if (condition && "+-".Contains(msg.Text[0]))
            {
                user.adminPanel.settingNum = false;
                user.adminPanel.editNum = msg.Text[0];
            }
            else
            {
                user.adminPanel.settingNum = true;
            }
            user.adminPanel.num = num;
            user.adminPanel.stage = EAdminPanelStage.WaitingForUserNameOrUserId;

            sendText = "Отправьте UserName или UserId, которому будет проведена операция.";
            await bot.SendTextMessageAsync(user.userid, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
        }
        public static async Task WaitingForUserNameOrUserId(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            long id = 0;
            string sendText = "";
            if (msg.Text[0] == '@')
            {
                user.adminPanel.isUserName = true;
                user.adminPanel.userIdOrUserName = msg.Text;
            }
            if (!user.adminPanel.isUserName && !long.TryParse(msg.Text, out id))
            {
                sendText = "Вы ввели ни UserName, ни UserId. Повторите ввод.";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                return;
            }
            BotUser user1;
            if (user.adminPanel.isUserName)
            {
                user1 = BotUser.AllUsers.First(m => m.username.ToLower() == user.adminPanel.userIdOrUserName.ToLower());
                user.adminPanel.userIdOrUserName = msg.Text;
            }
            else
            {
                user1 = BotUser.AllUsers.First(m => m.userid == id);
                user.adminPanel.userIdOrUserName = $"{id}";
            }
            if (user1 == default)
            {
                sendText = "Нет такого юзера с таким айди\\юзернеймом. Повторите ввод, либо /restart";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                return;
            }

            switch (user.adminPanel.function)
            {
                case "editbalance":
                    if (!user.adminPanel.settingNum)
                    {
                        user1.balance += (int)user.adminPanel.num;
                    }
                    else
                    {
                        user1.balance = (int)user.adminPanel.num;
                    }
                    //Variables.db.UpdateOrInsertUsersTable(user1, false);
                    break;
                case "editrandom":
                    if (!user.adminPanel.settingNum)
                    {
                        user1.pseudorandom.EditChance(user1.pseudorandom.chance + user.adminPanel.num);
                    }
                    else
                    {
                        user1.pseudorandom.EditChance(user.adminPanel.num);
                    }
                    //Variables.db.UpdateOrInsertUsersTable(user1, false);
                    break;
                case "editmoon":
                    if (!user.adminPanel.settingNum)
                    {
                        user1.moons += (int)user.adminPanel.num;
                    }
                    else
                    {
                        user1.moons = (int)user.adminPanel.num;
                    }
                    //Variables.db.UpdateOrInsertUsersTable(user1, false);
                    break;
                case "editspins":
                    if (!user.adminPanel.settingNum)
                    {
                        user1.spins += (int)user.adminPanel.num;
                        user1.spinsList.Add
                            (
                                new Spin()
                                {
                                    amount = 10,
                                    count = (int)user.adminPanel.num,
                                    native_count = (int)user.adminPanel.num
                                }
                            );
                    }
                    else
                    {
                        user1.spins = (int)user.adminPanel.num;
                        user1.spinsList = new List<Spin>()
                            {
                                  new Spin()
                                  {
                                      amount = 1,
                                      count = (int)user.adminPanel.num,
                                      native_count = (int)user.adminPanel.num
                                  }
                            };
                    }
                    //Variables.db.UpdateOrInsertUsersTable(user1, false);
                    break;
            }
            user.adminPanel.stage = EAdminPanelStage.Other;
            sendText =
                $"Вы использовали {user.adminPanel.function} на <a href=\"tg://user?id={user1.userid}\">нем</a>\n" +
                $"Его баланс и шанс:\n" +
                $"balance: {user1.balance}\n" +
                $"moons: {user1.moons}\n" +
                $"spins: {user1.spins}\n" +
                $"native_chance: {user1.pseudorandom.chance}\n";
            ReplyKeyboardMarkup rk;
            if (BotUser.ADMINS.Contains(msg.Chat.Id))
            {
                rk = Keyboards.adminStartButtons;
            }
            else
            {
                rk = Keyboards.startButtons;
            }
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: rk);
        }
        public static async Task WaitingForAmount(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            int amount = 0;
            if (!int.TryParse(msg.Text, out amount))
            {
                string sendText = "Вы ввели не число. Введите число оплаты без надписи \"р.\" \"рублей\" и т.д.\n" +
                    "Например: 100";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                return;
            }
            else
            {
                string sendText;
                if (amount < 10)
                {
                    sendText = "Слишком маленькая сумма";
                    await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                    return;
                }
                user.paydata.payAmount = amount;
                user.paydata.payStatus = EPayStatus.WaitingForConfirmation;

                BillResponse response = new BillResponse();
                while (true)
                {
                    try
                    {
                        response = await QiwiManager.CreateBillAsync(amount);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }

                user.paydata.billResponse = response;
                var ik = new InlineKeyboardMarkup(
                        new InlineKeyboardButton("Оплатить") { CallbackData = $"{user.userid} moneyPay" }
                    );
                sendText = localization.money_billInfo().ReplaceLocals(new[] { $"{user.paydata.payAmount}", $"{user.userid}" });

                await bot.SendTextMessageAsync(user.userid, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: ik);

                //Variables.db.UpdateOrInsertUsersTable(user, false);
            }
        }
        public static async Task waitingForQIWINumber(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var ik = new InlineKeyboardMarkup(
                   new InlineKeyboardButton[]{
                        new InlineKeyboardButton("Да"){CallbackData = $"{user.userid} moonout yes"},
                        new InlineKeyboardButton("Нет"){CallbackData = $"{user.userid} moonout no"}
                   });
            string sendText = $"Ваш UID: {msg.Text}";
            user.stage = EStage.waitingQiwiNumberConfirmation;
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
    }
}
