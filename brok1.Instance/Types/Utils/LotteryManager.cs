using Telegram.Bot;

namespace brok1.Instance.Types.Utils
{
    public static class LotteryManager
    {
        public static Task EndLottery(ITelegramBotClient bot, Lottery lottery)
        {
            List<Task> sendMessageTasks = new List<Task>();

            if (lottery is not null 
                && lottery.lotteryParticipants is not null 
                && lottery.lotteryParticipants is not null 
                && lottery.lotteryParticipants.Count != 0)
            {
                var winner = LotteryManager.GetWinner(lottery);
                lottery.lotteryWinner = winner;
                lottery.isWorking = false;
                winner.user.moons += 1;

                string sendText =
                    $"Лотерея окончена, спасибо всем, кто принимал участие!\n\n" +
                    $"Всего участвовало: {Lottery.LotteryNow.lotteryParticipants.Count}\n" +
                    $"Счастливый билет: {winner.ticketId}\n" +
                    $"Владелец билетика: <a href=\"tg://user?id={winner.user.userid}\">тык</a> ({winner.user.username})\n\n" +
                    $"Поздравляем!\n\n" +
                    $"Если вам не повезло - не расстраивайтесь. Мы ждем вас в следующей лотерее!";

                NotifyMessage notifyMessage = new NotifyMessage(sendText);
                _ = NotifyManager.NotifyAsync(bot, notifyMessage, Enums.ENotify.Admins);

                foreach (var part in Lottery.LotteryNow.lotteryParticipants.Select(m => m.user.userid).ToHashSet())
                {
                    sendMessageTasks.Add(bot.SendMessage(part, sendText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html));
                }
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~ creating new lottery
            CreateLottery();
            return Task.WhenAll(sendMessageTasks);
        }
        public static void CreateLottery()
        {
            lock (Lottery.lotteryLocker)
            {
                Lottery lottery = new Lottery();
                lottery.isWorking = true;
                foreach (var l in Lottery.AllLotteries)
                {
                    l.isWorking = false;
                }
                if (Lottery.LotteryNow != null)
                {
                    lottery.lotteryId = Lottery.LotteryNow.lotteryId + 1;
                }
                else
                {
                    if (Lottery.AllLotteries.Count != 0)
                    {
                        lottery.lotteryId = Lottery.AllLotteries.OrderBy(m => m.lotteryId).Last().lotteryId + 1;
                    }
                    else
                    {
                        lottery.lotteryId = 0;
                    }
                }
                lottery.lotteryEnd = DateTime.Now.AddDays(1);
                lottery.lotteryEnd = lottery.lotteryEnd.AddHours(21 - lottery.lotteryEnd.Hour).AddMinutes(0 - lottery.lotteryEnd.Minute);
                for (int i = 0; i <= 6; i++)
                {
                    if (lottery.lotteryEnd.DayOfWeek != DayOfWeek.Sunday)
                    {
                        lottery.lotteryEnd = lottery.lotteryEnd.AddDays(1);
                    }
                    else break;
                }
                Lottery.AllLotteries.Add(lottery);
                //Variables.db.UpdateOrInsertLotteriesTable(Variables.lottery);
                //Variables.db.DeleteFromParticipantsTable(-1);
            }
        }
        public static async Task Check(Lottery lottery)
        {
            var interval = lottery.lotteryEnd - DateTime.Now;
            if (interval.TotalMilliseconds <= 0)
            {
                if (!lottery.isWorking)
                    return;
                await EndLottery(BotInfo.botClient, lottery);
                return;
            }
            if (lottery.timer != null)
            {
                lottery.timer.Enabled = false;
                lottery.timer.Dispose();
            }
            lottery.timer = new System.Timers.Timer(interval.TotalMilliseconds);
            lottery.timer.AutoReset = false;
            lottery.timer.Elapsed += (s, e) =>
            {
                if (!lottery.isWorking)
                    return;

                _ = EndLottery(BotInfo.botClient, lottery);
                Console.WriteLine($"timer {lottery.lotteryId}");
            };
            lottery.timer.Start();
        }

        public static LotteryParticipant GetWinner(Lottery lottery) => lottery.lotteryParticipants[Pseudorandom.GetRandom(0, lottery.lotteryParticipantsCount - 1)];

        public static long GetNextParticipantId(Lottery lottery)
        {
            if (lottery.lotteryParticipants.Count != 0)
            {
                return lottery.lotteryParticipants.OrderBy(m => m.participantId).Last().participantId + 1;
            }
            else
            {
                return 0;
            }
        }

        public static long GetNextTicketId(Lottery lottery)
        {
            if (lottery.lotteryParticipants.Count != 0)
            {
                return lottery.lotteryParticipants.OrderBy(m => m.ticketId).Last().ticketId + 1;
            }
            else
            {
                return 1;
            }
        }

    }
}
