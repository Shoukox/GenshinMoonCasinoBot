using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Types.Utils
{
    public class UsersManager
    {
        public static void ClearUsersFlags(BotUser user)
        {
            user.stage = EStage.Other;
            user.adminPanel = new AdminPanel();
            user.isFeedbacking = false;
            user.isWishing = false;
        }
        public static BotUser CheckUser(Telegram.Bot.Types.User user)
        {
            var user1 = BotUser.AllUsers.FirstOrDefault(m => m.userid == user.Id);
            if (user1 == default)
            {
                BotUser userToAdd = new BotUser
                {
                    userid = user.Id,
                    username = $"@{user.Username}",
                    balance = 0,
                    moneyadded = 0,
                    moneyused = 0,
                    spins = 0,
                    stoppedBot = false,
                    wasRecentlyAdded = true
                };

                BotUser.AllUsers.Add(userToAdd);

                var cN = newUsers.AllNewUsers.LastOrDefault();
                if (cN == default)
                {
                    newUsers.AllNewUsers.Add(new newUsers() { dateTime = DateTime.Now, usersCount = 1 });
                }
                else
                {
                    if (cN.dateTime.Day == DateTime.Now.Day && cN.dateTime.Month == DateTime.Now.Month && cN.dateTime.Year == DateTime.Now.Year)
                    {
                        cN.usersCount += 1;
                    }
                    else
                    {
                        newUsers.AllNewUsers.Add(new newUsers() { dateTime = DateTime.Now, usersCount = 1 });
                    }
                }

                user1 = userToAdd;
            }
            else
            {
                user1.username = $"@{user.Username}";
                user1.stoppedBot = false;
            }
            return user1;
        }
        public static double[] GetCurrentUsersVisualChance(BotUser user)
        {
            double chance = -1;
            if (user.spinsList.Count >= 1)
            {
                user.spinsList = user.spinsList.OrderByDescending(m => m.chance).ToList();
                return user.spinsList[0].GetCurrentChance(user); //[1] - visualChance
            }

            chance = 0.1 + user.freeSpinsUsedAfterWin * 0.1; //0.1% fact, 1% visual
            return new double[]
                {
                chance,
                chance + 0.9 //visualChance
                };

        }
        public static async Task<bool> IsChatMemberAsync(ITelegramBotClient bot, long userId, ChatId chatId)
        {
            try
            {
                var member = await bot.GetChatMember(chatId, userId);
                return (member.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator) ||
                            (member.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator) ||
                            (member.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public static void SetFreeSpinsUsedAfterWin(BotUser user, int n)
        {
            user.freeSpinsUsedAfterWin = n;
            user.pseudorandom = new Pseudorandom(0.1 + user.freeSpinsUsedAfterWin * 0.1);
        }

        /// <summary>
        /// [0] - day (1)
        /// [1] - week (7)
        /// [2] - month (30)
        /// </summary>
        public static int[] GetNewUsersCount()
        {
            int countDay = 0, countWeek = 0, countMonth = 0;
            if ((newUsers.AllNewUsers.Last().dateTime - newUsers.AllNewUsers.First().dateTime).Days <= 30)
            {
                countMonth = newUsers.AllNewUsers.Sum(m => m.usersCount);
            }
            else
            {
                countMonth = newUsers.AllNewUsers.Where(m => (newUsers.AllNewUsers.Last().dateTime - m.dateTime).Days <= 30).Sum(m => m.usersCount);
            }

            if ((newUsers.AllNewUsers.Last().dateTime - newUsers.AllNewUsers.First().dateTime).Days <= 7)
            {
                countWeek = newUsers.AllNewUsers.Sum(m => m.usersCount);
            }
            else
            {
                countWeek = newUsers.AllNewUsers.Where(m => (newUsers.AllNewUsers.Last().dateTime - m.dateTime).Days <= 7).Sum(m => m.usersCount);
            }

            countDay = newUsers.AllNewUsers.Last().usersCount;

            return new int[] { countDay, countWeek, countMonth };
        }
    }
}
