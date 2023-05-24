namespace brok1.Instance.Types;

public class Spin
{
    public double chance
    {
        get
        {
            return amount switch
            {
                0 => 5, //doesn't exist rn
                1 => 5,
                19 => 5,
                79 => 5,
                49 => 15,
                149 => 5,
                209 => 15,
                399 => 15,
                _ => -1,
            };
        }
    }
    public int amount { get; set; }
    public int count { get; set; }
    public int native_count { get; set; }

    public int RemoveOneSpin()
    {
        count -= 1;

        if (count <= 0)
        {
            return -1;
        }
        else
        {
            return count;
        }
    }
    public double[] GetCurrentChance(BotUser user)
    {
        if (chance < 1) //if gived by admin panel
            return new double[]{
                1,
                1
            };

        double fact_chance = amount switch
        {
            0 => 0.1 + user.freeSpinsUsedAfterWin * 0.1,
            79 => (native_count - count) * 1 + chance * 0.8,
            209 => (native_count - count) * 2 + chance * 0.8,
            399 => (native_count - count) * 2 + chance * 0.95,
            _ => chance * 0.8,
        };
        double visual_chance = amount switch
        {
            0 => 1 + user.freeSpinsUsedAfterWin * 0.1,
            79 => (native_count - count) * 1 + chance,
            209 => (native_count - count) * 2 + chance,
            399 => (native_count - count) * 2 + chance,
            _ => chance
        };
        Console.WriteLine($"visual: {visual_chance}, fact: {fact_chance}");
        return new double[]
        {
            Math.Round(fact_chance, 1),
            Math.Round(visual_chance, 1)
        };
    }
    public bool LoadData(string spinsstring)
    {
        if (spinsstring == "")
            return false;

        string[] splitted = spinsstring.Split("-");

        if (splitted.Length != 4)
            return false;

        amount = int.Parse(splitted[0]);
        //chance = double.Parse(splitted[1]);
        count = int.Parse(splitted[2]);
        native_count = int.Parse(splitted[3]);
        return true;
    }
    public string SaveData()
    {
        return $"{amount}-{chance}-{count}-{native_count}";
    }
}
