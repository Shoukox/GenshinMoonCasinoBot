using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Commands.BotCommands.Handlers
{
    public static class ReplyCommandsHandler
    {
        public static async Task Профиль(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = localization.button_balance().ReplaceLocals(new[] { $"{msg.From!.FirstName.ParseHTMLSymbols()} {msg.From.LastName.ParseHTMLSymbols()}", $"{Math.Round(UsersManager.GetCurrentUsersVisualChance(user)[1], 1)}", $"{user.moons}", $"{user.spins}", $"{user.crystals}", $"{user.balance}", $"{user.moneyadded}", $"{user.moneyused}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton[][]{
                        new InlineKeyboardButton[]{new InlineKeyboardButton("Реферальная система"){ CallbackData = $"{msg.From.Id} referal"}},
                        new InlineKeyboardButton[]{new InlineKeyboardButton("Пополнить") { CallbackData = $"{msg.From.Id} replenish"}, new InlineKeyboardButton("Вывести"){ CallbackData = $"{msg.From.Id} withdraw"} },
                    });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: ik, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Рулетка(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            double[] chance = UsersManager.GetCurrentUsersVisualChance(user);
            string sendText = localization.button_roulette().ReplaceLocals(new[] { $"{Math.Round(chance[1], 1)}" });
            var rk = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]{
                                    new KeyboardButton[]{ new KeyboardButton("🌙 Крутить Луну"), new KeyboardButton("Уведомления") },
                                    new KeyboardButton[]{ new KeyboardButton("Назад") },
                    }
                )
            { ResizeKeyboard = true };

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: rk);
        }
        public static async Task Обратная_связь(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = localization.button_feedback();

            var rk = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Ваши пожелания"),
                            new KeyboardButton("Отзывы")
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Назад"),
                        }
            })
            { ResizeKeyboard = true };

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: rk, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Ваши_пожелания(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "<b>Напишите идеи по улучшению бота.</b>\n\n" +
                  "<i>Мы обязательно рассмотрим ваше сообщение. Если идея нас зацепит, вам будет начислено 10р в качестве благодарности.</i>";
            user.isWishing = true;
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Бесплатный_код_на_Крылья(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            using var fs1 = System.IO.File.OpenRead("1.png");
            var jpg1 = new InputFileStream(fs1, "content.png");


            string text = "Раз ты тыкнул на эту кнопку, то уже знаешь о <b>новых крыльях</b>, которые можно получить, активировав промокоды.\n" +
                "За <b>один период</b> нужно активировать только <b>один промокод</b>, для получения крыльев - <b><i>всего 4 промокода.</i></b>\n\n" +
                "<i>У нас ты можешь получить этот код абсолютно бесплатно!</i>\n" +
                "Просто нажми на кнопку <b>«Получить код»☑️</b>\n\n" +
                "<b>Подробнее о новых крыльях <a href=\"https://t.me/hey_vadimchik/2992\">ЗДЕСЬ</a></b>";
            var rk = new ReplyKeyboardMarkup(new KeyboardButton[][]{
                        new KeyboardButton[] { new KeyboardButton("Получить код"), new KeyboardButton("Общая информация") },
                        new KeyboardButton[] { new KeyboardButton("Назад") },
                    })
            { ResizeKeyboard = true };
            await bot.SendPhotoAsync(msg.Chat.Id, jpg1, replyToMessageId: msg.MessageId, caption: text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: rk);
        }
        public static async Task Получить_код(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            //GGIMPACT id: -1001651002696
            //hey id: -1001795188948
            if (!await UsersManager.IsChatMemberAsync(bot, user.userid, -1001795188948))
            {
                string sendText = "Подпишись на наш канал, ведь мы раздаем коды совершенно бесплатно!\n" +
                    "1. https://t.me/+IN-oHJ2_ZVwzYjRi";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, disableWebPagePreview: true);
                return;
            }

            if (!user.rabatt_codes.CanGetCode())
            {
                string lastCode = user.rabatt_codes.codes[UsersRabattCodes.GetIndexNumber()];
                string sendText = $"Не торопись, ты уже взял свой код!\n<i>Дождись следующего периода, о нем мы сообщим на нашем канале @hey_Vadimchik</i>\n\n<b>Твой код в этом периоде: {lastCode}</b>";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                return;
            }

            if (!user.rabatt_codes.GetCode())
            {
                string sendText = "Ой, все коды разобрали, но ничего страшного, скоро мы закинем еще!\n\n" +
                    "Оповестим на нашем канале: \nhttps://t.me/+IN-oHJ2_ZVwzYjRi";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                return;
            }

            int index = UsersRabattCodes.GetIndexNumber();
            string code = user.rabatt_codes.codes[index];
            string text = $"Твой код: {code}";

            await bot.SendTextMessageAsync(msg.Chat.Id, text);
        }
        public static async Task Общая_информация(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            int periodNum = UsersRabattCodes.GetPeriodNumber();
            if (periodNum == 0)
            {
                string sendText = "Дождитесь следующего обновления!";
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText);
                return;
            }

            List<int> periodCodesCount = new List<int>();
            for (int i = 4; i <= periodNum - 1; i++)
            {
                periodCodesCount.Add(UsersRabattCodes.AllRabattCodes.Count(m => m.code.Contains($"_used{i}")));
            }

            string periodCodesCountText = "";
            if (periodCodesCount.Count != 0)
            {
                for (int i = 0; i <= periodCodesCount.Count - 1; i++)
                {
                    periodCodesCountText += $"<b>Раздали за {4 + i} период:</b> {periodCodesCount[i]}\n";
                }
            }

            string text =
                $"{periodCodesCountText}\n" +
                $"<b>Период {periodNum}</b>\n" +
                $"Было кодов: <i>{UsersRabattCodes.AllRabattCodes.Count(m => !m.code.Contains("_used") || m.code.EndsWith($"_used{periodNum}"))}</i>\n" +
                $"Разыграли: <i>{UsersRabattCodes.AllRabattCodes.Where(s => s.code.EndsWith($"_used{periodNum}")).Count()}</i>\n" +
                $"Осталось: <i>{UsersRabattCodes.AllRabattCodes.Where(s => !s.code.Contains($"_used")).Count()}</i>";

            await bot.SendTextMessageAsync(msg.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Отзывы(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "Пожалуйста, оставьте свой отзыв для других пользователей нашего бота!";
            var rk = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Отзывы участников"),
                            new KeyboardButton("Написать отзыв"),
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Назад"),
                        },
            })
            { ResizeKeyboard = true };

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: rk, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Отзывы_участников(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "Посмотреть <b>отзывы участников</b> вы сможете <a href=\"https://t.me/+53z0tg2GH4JlYmMy\">здесь</a>:";
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Посмотреть отзывы") { Url = "https://t.me/+53z0tg2GH4JlYmMy" });

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: ik, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableWebPagePreview: true);

        }
        public static async Task Написать_отзыв(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "<b>Оставьте свои отзыв.</b>\nНе забудьте прикрепить скриншоты с бота и игры.\n";
            user.isFeedbacking = true;
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Информация(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = localization.button_info();
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableWebPagePreview: true);
        }
        public static async Task Назад(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            UsersManager.ClearUsersFlags(user);
            ReplyKeyboardMarkup rk = null;
            if (BotUser.ADMINS.Contains(msg.Chat.Id))
            {
                rk = Keyboards.adminStartButtons;
            }
            else
            {
                rk = Keyboards.startButtons;
            }

            string sendText = localization.command_start();
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: rk, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public static async Task Крутить_луну(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "";
            InlineKeyboardMarkup ik = null;
            foreach (var sponsor in Sponsor.AllSponsors)
            {
                if (!await UsersManager.IsChatMemberAsync(bot, user.userid, sponsor.channelId))
                {
                    sendText =
                        $"Перед тем как крутить Луну, пожалуйста подпишись на меня и канал спонсора!\n\n" +
                        $"{string.Join("\n", Sponsor.AllSponsors.Select((m, index) => $"{index + 1}. {m.channelLink}"))}";
                    await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, disableWebPagePreview: true);
                    return;
                }
            }
            sendText = "Прокручиваем рулетку\n";
            if (user.canFreeSpin || user.spins >= 1 || user.spinsList.Count >= 1 /*|| Variables.WHITELIST.Contains(msg.From.Id)*/)
            {
                //уже крутит?
                if (user.isSpinning)
                {
                    sendText = "Подождите окончания прошлой рулетки";
                    await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                    return;
                }
                user.isSpinning = true;

                //вид крутки
                bool isSpinsList = false;
                bool isPayedSpin = isSpinsList || user.spins > 0;
                if (user.canFreeSpin && !isPayedSpin)
                {
                    user.freeSpinsUsedAfterWin += 1;
                    user.lastFreeSpin = DateTime.Now;
                }
                else if (user.spins >= 1)
                {
                    user.spins -= 1;
                }

                //крутка купленная
                if (user.spinsList.Count != 0)
                {
                    user.spinsList = user.spinsList.OrderByDescending(m => m.chance).ToList();
                    isSpinsList = true;
                }


                //выиграл или нет
                bool wonMoon = false;
                bool wonCrystals = user.pseudorandom.ProcessChance(user.hasPayed, true);
                if (isSpinsList)
                {
                    wonMoon = user.pseudorandom.ProcessChance(user.hasPayed, false, user.spinsList[0].GetCurrentChance(user)[0]);
                    if (user.spinsList[0].RemoveOneSpin() == -1)
                    {
                        user.spinsList.RemoveAt(0);
                    }
                }
                else
                {
                    wonMoon = user.pseudorandom.ProcessChance(user.hasPayed);
                }
                user.wasNotified = false;

                AnimationGIF[] gifs = (AnimationGIF[])AnimationGIF.AllGIFs.Clone();
                Message message = null;
                if (wonMoon)
                {
                    //по шансу выиграл - но так, проиграет
                    if (!user.pseudorandom.mustWin)
                    {
                        //3stargif
                        gifs[0].gifStream.Position = 0;
                        message = await bot.SendAnimationAsync(msg.Chat.Id, new InputFileStream(gifs[0].gifStream, "3star.mp4"), caption: "Крутим рулетку...");

                        await Task.Delay(7000);
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        sendText = localization.roulette_lose(); //lose
                        await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId);

                        ik = new InlineKeyboardMarkup
                            (
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton("Да"){CallbackData=$"{user.userid} editrandom yes"},
                                            new InlineKeyboardButton("Нет"){CallbackData=$"{user.userid} editrandom no"}
                                }
                            );
                        NotifyMessage notify = new NotifyMessage(
                            sendText + $"\n\nUserId: {user.userid}\n" +
                            $"UserName: {user.username}\n" +
                            $"<a href=\"tg://user?id={user.userid}\">Ссылка</a>\n" +
                            $"Всего денег закинул: {user.moneyadded}\n" +
                            $"Всего потратил: {user.moneyused}\n" +
                            $"Его баланс: {user.balance}\n" +
                            $"Всего круток сделано: {user.pseudorandom.success + user.pseudorandom.loss}", ik);
                        _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);
                    }
                    else
                    {
                        //5stargif
                        gifs[1].gifStream.Position = 0;

                        message = await bot.SendAnimationAsync(msg.Chat.Id, new InputFileStream(gifs[1].gifStream, "5star.mp4"), caption: "Крутим рулетку...");
                        await Task.Delay(7000);
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        if (!isPayedSpin)
                        {
                            user.freeSpinsUsedAfterWin = 0;
                        }
                        user.pseudorandom.EditChance(user.pseudorandom.native_chance, false);
                        user.moons += 1;
                        sendText = localization.roulette_win();

                        NotifyMessage notify = new NotifyMessage(sendText +
                            $"\n\nОн должен был выиграть по прихоти админа.\n" +
                            $"UserId: {user.userid}\n" +
                            $"UserName: {user.username}\n" +
                            $"<a href=\"tg://user?id={user.userid}\">Ссылка</a>\n" +
                            $"Всего денег закинул: {user.moneyadded}\n" +
                            $"Всего потратил: {user.moneyused}\n" +
                            $"Его баланс: {user.balance}\n" +
                            $"Всего круток сделано: {user.pseudorandom.success + user.pseudorandom.loss}");
                        _ = NotifyManager.NotifyAsync(bot, notify, ENotify.Admins);
                        await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                    }

                }
                else
                {
                    if (wonCrystals)
                    {
                        gifs[2].gifStream.Position = 0;
                        message = await bot.SendAnimationAsync(msg.Chat.Id, new InputFileStream(gifs[2].gifStream, "4star.mp4"), caption: "Крутим рулетку...");
                        await Task.Delay(7000);
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        sendText = "<b>Поздравляю, ты выиграл Кристалы (💎)!</b>\n\n<i>Ты можешь вывести их в своем профиле (/start -> Профиль)</i>\n";
                        user.crystals += 60;
                    }
                    else
                    {
                        gifs[0].gifStream.Position = 0;
                        message = await bot.SendAnimationAsync(msg.Chat.Id, new InputFileStream(gifs[0].gifStream, "3star.mp4"), caption: "Крутим рулетку...");
                        await Task.Delay(7000);
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        sendText = localization.roulette_lose();

                        if (user.freeSpinsUsedAfterWin > 140)
                        {
                            sendText += $"\n\nВаш наращенный шанс с бесплатных прокруток был сброшен. Вы использовали уже 140 бесплатных круток.";
                            user.freeSpinsUsedAfterWin = 0;
                        }
                    }
                    user.isSpinning = false;
                    await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                }
            }
            else
            {
                var nextSpin = user.nextFreeSpin - DateTime.Now;
                sendText = localization.roulette_limit().ReplaceLocals(new[] { $"{nextSpin.Hours}ч {nextSpin.Minutes}м {nextSpin.Seconds}с" });
                await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId);
            }
            user.isSpinning = false;
        }
        public static async Task Магазин(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string sendText = "Здесь вы можете приобрести прокруты в рулетке на выпадение луны. \n<b>high chance</b> – это повышенная вероятность выпадения дропа (шанс выше в 3 раза)\nС каждой обычной круткой из магазина шанс на следующий кручение вырастает на 1%, а с круткой на <b>high chance</b> на 2%. Максимальный шанс не выше 50%.\r\n";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: Keyboards.ShopButtons, replyToMessageId: msg.MessageId);
        }
        public static async Task Лотерея(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (Lottery.LotteryNow == null)
            {
                LotteryManager.CreateLottery();
            }
            string sendText =
            $"<b>Прими участие в Лотерее!</b>\n\n" +
            "Лотерея длится неделю, результаты оглашаются каждое вск в 18:00 среди участвующих.\n\nКаждый может купить неограниченное кол. лотерейных билетов, цена билета 20р. \n\n" +
            "Приз: <b>Благословение Полой луны</b>";


            ReplyKeyboardMarkup rk = new ReplyKeyboardMarkup
                (
                new KeyboardButton[][] {
                        new KeyboardButton[]
                        {
                          new KeyboardButton("Информация о розыгрыше"),
                          new KeyboardButton("🎟 Получить билетик")

                        },
                        new KeyboardButton[]
                        {
                          new KeyboardButton("Назад")
                        }
                    }
                )
            { ResizeKeyboard = true };

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: rk, replyToMessageId: msg.MessageId);
        }
        public static async Task Рассылка(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForStringToNotifyAllUsers;
            string sendText = "Отправьте сообщение, которое надо переслать.";
            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Спонсоры(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForStringToEditSponsors;
            string sendText = "Введите список спонсоров\n" +
                "Формат: «ID канала - ссылка канала». добавь тире между ними\n" +
                "\n" +
                "ID канала можно взять <a href=\"https://t.me/userinfobot\">тут</a>.";

            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId, disableWebPagePreview: true);
        }
        public static async Task Уведомления(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            ReplyKeyboardMarkup rk = new ReplyKeyboardMarkup(
                            new KeyboardButton[][]
                            {
                                    new KeyboardButton[]
                                    {
                                      new KeyboardButton("Включить"),
                                      new KeyboardButton("Выключить")

                                    },
                                    new KeyboardButton[]
                                    {
                                      new KeyboardButton("Назад")
                                    }
                            })
            { ResizeKeyboard = true };
            string status = user.notifyEnabled == true ? "включено" : "выключено";
            string sendText =
                "Включить уведомления?\n\n" +
                $"У вас: {status}";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId, replyMarkup: rk);
        }
        public static async Task Включить_уведомления(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            user.notifyEnabled = true;
            string sendText = $"Готово, уведомления включены.";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Выключить_уведомления(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            user.notifyEnabled = false;
            string sendText = $"Готово, уведомления выключены.";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Получить_билетик(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            int TicketCost = 20;

            string sendText = "";
            if (Lottery.LotteryNow == null)
            {
                sendText = "Лотерея еще не начата. Обратитесь к администратору бота, если лотерея не начинается долгое время";
                await bot.SendTextMessageAsync(user.userid, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                return;
            }
            if (user.balance < TicketCost)
            {
                sendText = "У вас недостаточно баланса для покупки билета";
                await bot.SendTextMessageAsync(user.userid, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
                return;
            }

            user.balance -= TicketCost;
            user.moneyused += TicketCost;


            LotteryParticipant part = null;
            lock (Lottery.lotteryLocker)
            {
                part = new LotteryParticipant();
                part.lotteryId = Lottery.LotteryNow.lotteryId;
                part.participantId = LotteryManager.GetNextParticipantId(Lottery.LotteryNow);
                part.ticketId = LotteryManager.GetNextTicketId(Lottery.LotteryNow);
                part.user = user;
            }
            Lottery.LotteryNow.lotteryParticipants.Add(part);
            sendText = "<b>Вы успешно купили билет!</b>\n\n" +
                $"Ваши билеты: {string.Join(", ", Lottery.LotteryNow.lotteryParticipants.Where(m => m.user.userid == user.userid).Select(m => m.ticketId))}";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
        }
        public static async Task Информация_о_розыгрыше(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var datetimeOffset = Lottery.LotteryNow!.lotteryEnd - DateTime.Now;
            string sendText =
               $"<b>Лотерея №{Lottery.LotteryNow.lotteryId}</b>\n\n" +
               $"Закончится через: {datetimeOffset.Days}д {datetimeOffset.Hours}ч {datetimeOffset.Minutes}м {datetimeOffset.Seconds}с\n" +
               $"Распроданных билетов: {Lottery.LotteryNow.lotteryParticipantsCount}\n" +
               $"Ваши билеты: {string.Join(", ", Lottery.LotteryNow.lotteryParticipants.Where(m => m.user.userid == user.userid).Select(m => m.ticketId))}";
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
        }
        public static async Task Админ_панель(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel = new AdminPanel();
            ReplyKeyboardMarkup rk = Keyboards.adminPanelButtons;
            user.adminPanel.stage = EAdminPanelStage.ChoosingAdminFunc;
            await bot.SendTextMessageAsync(user.userid, "Админ панель", replyMarkup: rk, replyToMessageId: msg.MessageId);
        }
        public static async Task Статистика(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From!.Id))
                return;

            int[] newUsersCount = new int[3];
            if (newUsers.AllNewUsers.Count != 0)
            {
                newUsersCount = UsersManager.GetNewUsersCount();
            }

            string sendText = $"👥 Всего пользователей: {BotUser.AllUsers.Count}\n" +
                $"├ Активных: {BotUser.AllUsers.Count(m => m.stoppedBot == false)}\n" +
                $"└ Не активных: {BotUser.AllUsers.Count(m => m.stoppedBot == true)}\n" +
                $"\n" +
                $"Приход" +
                $"\n├ За месяц: {newUsersCount[2]}" +
                $"\n├ За неделю: {newUsersCount[1]}" +
                $"\n└ За день: {newUsersCount[0]}";

            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
        }
        public static async Task Подкрутка(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            var rk = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                         new KeyboardButton[]
                        {
                            new KeyboardButton("Шанс след. крутки"),
                            new KeyboardButton("Изменить баланс"),
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Изменить кол-во лун"),
                            new KeyboardButton("Изменить кол-во круток"),
                        },
                         new KeyboardButton[]
                        {
                            new KeyboardButton("Назад"),
                        }
            });
            await bot.SendTextMessageAsync(user.userid, "Админ панель", replyMarkup: rk, replyToMessageId: msg.MessageId);
        }
        public static async Task Шанс_следующей_крутки(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForNum;
            user.adminPanel.function = "editrandom";
            string sendText =
                "Введите число, на которое вы хотите заменить значение свойства пользователя, в зависимости от выбранной функции.\n\n" +
                "Если вы хотите добавить или отнять от его текущего числа, то укажите +5 или -5 соответственно.";
            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Изменить_баланс(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForNum;
            user.adminPanel.function = "editbalance";
            string sendText =
                 "Введите число, на которое вы хотите заменить значение свойства пользователя, в зависимости от выбранной функции.\n\n" +
                 "Если вы хотите добавить или отнять от его текущего числа, то укажите +5 или -5 соответственно.";
            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Изменить_количество_лун(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForNum;
            user.adminPanel.function = "editmoon";
            string sendText =
                  "Введите число, на которое вы хотите заменить значение свойства пользователя, в зависимости от выбранной функции.\n\n" +
                  "Если вы хотите добавить или отнять от его текущего числа, то укажите +5 или -5 соответственно.";
            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task Изменить_количество_круток(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            if (!BotUser.ADMINS.Contains(msg.From.Id))
                return;

            user.adminPanel.stage = EAdminPanelStage.WaitingForNum;
            user.adminPanel.function = "editspins";
            string sendText =
                "Введите число, на которое вы хотите заменить значение свойства пользователя, в зависимости от выбранной функции.\n\n" +
                "Если вы хотите добавить или отнять от его текущего числа, то укажите +5 или -5 соответственно.";
            await bot.SendTextMessageAsync(user.userid, sendText, replyToMessageId: msg.MessageId);
        }
        public static async Task крутка1_19(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            string itemName = "1 крутка: 19р";
            int amount = 19;
            string sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 1_19" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task крутка5_79(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var itemName = "5 круток: 79р (выгоднее на 15%)";
            var amount = 79;
            var sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 5_79" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task крутка10_149(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var itemName = "10 круток: 149р (выгоднее на 20%)";
            var amount = 149;
            var sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 10_149" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task крутка1_49(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var itemName = "1 крутка <b>high chance</b>: 49р";
            var amount = 49;
            var sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 1_49" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task крутка5_209(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var itemName = "5 круток <b>high chance</b>: 209р (выгоднее на 15%)";
            var amount = 209;
            var sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 5_209" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task крутка10_399(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var itemName = "10 круток <b>high chance</b>: 399р (выгоднее на 20%)";
            var amount = 399;
            var sendText = localization.shop_item().ReplaceLocals(new[] { $"{itemName}", $"{user.balance}", $"{amount}" });
            var ik = new InlineKeyboardMarkup(new InlineKeyboardButton("Купить") { CallbackData = $"{user.userid} shop 10_399" });
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId, replyMarkup: ik);
        }
        public static async Task Добавить_кнопки(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var sendText = "Впишите все кнопки в формате:\n\nКнопка 1 - Ссылка 1 | Кнопка 2 - Ссылка 2\nЧтобы добавить кнопку на новый ряд, используйте ||";
            user.adminPanel.stage = EAdminPanelStage.WaitingForButtonText;
            await bot.SendTextMessageAsync(msg.Chat.Id, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: msg.MessageId);
        }
        public static async Task Без_кнопок(ITelegramBotClient bot, Message msg, BotUser user, ILocalization localization)
        {
            var ik = new InlineKeyboardMarkup
                                  (
                                      new InlineKeyboardButton[]
                                      {
                                    new InlineKeyboardButton("Да") {CallbackData = $"{user.userid} notifyAllUsers yes"},
                                    new InlineKeyboardButton("Нет") {CallbackData = $"{user.userid} notifyAllUsers no"}
                                      }
                                  );

            string text = "\n\nОтправляем?";
            await bot.CopyMessageAsync(msg.Chat.Id, user.adminMessageToSend.Chat.Id, user.adminMessageToSend.MessageId);
            await bot.SendTextMessageAsync(user.userid, text, replyMarkup: ik, replyToMessageId: msg.MessageId);
        }
    }
}
