namespace brok1.Instance.Types.Utils;
public static class TextDatabase
{
    public static void SaveData(bool newUsersFile = false)
    {
        using (StreamWriter sw = new StreamWriter($"users" + (newUsersFile ? DateTime.Now.ToString("dd:MM") : "") + ".txt"))
        {
            sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(BotUser.AllUsers));
        }
        using (StreamWriter sw = new StreamWriter($"lotteries.txt"))
        {
            sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Lottery.AllLotteries));
        }
        using (StreamWriter sw = new StreamWriter($"newUsers.txt"))
        {
            sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(newUsers.AllNewUsers));
        }
        using (StreamWriter sw = new StreamWriter($"sponsors.txt"))
        {
            sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Sponsor.AllSponsors));
        }
        using (StreamWriter sw = new StreamWriter($"rabattcodes.txt"))
        {
            sw.WriteLine(string.Join("\n", UsersRabattCodes.AllRabattCodes.Select(m => m.code)));
        }
    }
    public static void LoadData()
    {
        if (!File.Exists("users.txt"))
            File.Create("users.txt").Close();
        if (!File.Exists("lotteries.txt"))
            File.Create("lotteries.txt").Close();
        if (!File.Exists("newUsers.txt"))
            File.Create("newUsers.txt").Close();
        if (!File.Exists("sponsors.txt"))
            File.Create("sponsors.txt").Close();
        if (!File.Exists("rabattcodes.txt"))
            File.Create("rabattcodes.txt").Close();

        Console.WriteLine("startedLoadData");
        using (StreamReader sw = new StreamReader("users.txt"))
        {
            string text = sw.ReadToEnd();
            List<BotUser> data = text != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<BotUser[]>(text).ToList() : new List<BotUser>();
            BotUser.AllUsers = data;
        }
        using (StreamReader sw = new StreamReader("newUsers.txt"))
        {
            string text = sw.ReadToEnd();
            List<newUsers> data = text != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<newUsers[]>(text).ToList() : new List<newUsers>();
            newUsers.AllNewUsers = data;
        }
        using (StreamReader sw = new StreamReader("sponsors.txt"))
        {
            string text = sw.ReadToEnd();
            List<Sponsor> data = text != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<Sponsor[]>(text).ToList() : new List<Sponsor>();
            Sponsor.AllSponsors = data;
        }
        using (StreamReader sw = new StreamReader("lotteries.txt"))
        {
            string text = sw.ReadToEnd();
            List<Lottery> data = text != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<Lottery[]>(text).ToList() : new List<Lottery>();
            Lottery.AllLotteries = data;
        }
        using (StreamReader sw = new StreamReader("rabattcodes.txt"))
        {
            string text = sw.ReadToEnd().Trim().Replace("\r", "");
            List<RabattCode> data = text != "" ? text.Split("\n").Select(m => new RabattCode(m)).ToList() : new List<RabattCode>();
            UsersRabattCodes.AllRabattCodes = data;
        }
        Console.WriteLine("endedLoadData");

        if (Lottery.AllLotteries.Count != 0)
        {
            if (Lottery.AllLotteries.Last().lotteryEnd > DateTime.Now)
            {
                Lottery.LotteryNow!.isWorking = true;
                LotteryManager.Check(Lottery.LotteryNow).GetAwaiter().GetResult();
            }
            else
            {
                LotteryManager.CreateLottery();
            }
        }
        else
        {
            LotteryManager.CreateLottery();
        }

        Console.WriteLine($"users: {BotUser.AllUsers.Count}");
        Console.WriteLine($"lotteries: {Lottery.AllLotteries.Count}");
        Console.WriteLine($"newUsers: {newUsers.AllNewUsers.Count}");
    }


    public static void SaveTimer()
    {
        var saveDataTimer = new System.Timers.Timer(1 * 24 * 3600 * 1000);
        saveDataTimer.Elapsed += (s, e) =>
        {
            try
            {
                TextDatabase.SaveData(true);
                Console.WriteLine("saved");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        };
        saveDataTimer.Start();
    }
}
