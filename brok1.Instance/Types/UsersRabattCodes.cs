namespace brok1.Instance.Types;

public class UsersRabattCodes
{
    public static List<RabattCode> AllRabattCodes = new List<RabattCode>();
    public static object rabattCodesLocker = new object();

    /// <summary>
    /// Каждому коду под индексом соответстует период. codes.Length = periods.Length
    /// </summary>
    public string[] codes = new string[5];
    public static DateTime[] periods = new DateTime[5]
    {
        new DateTime(2023, 3, 8, 15, 59, 59, DateTimeKind.Utc),
        new DateTime(2023, 3, 29, 15, 59, 59, DateTimeKind.Utc),
        new DateTime(2023, 4, 19, 0, 59, 59, DateTimeKind.Utc),
        new DateTime(2023, 5, 10, 15, 59, 59, DateTimeKind.Utc),
        new DateTime(2023, 5, 31, 15, 59, 59, DateTimeKind.Utc),
    };

    /// <summary>
    /// Если true - то он еще не получал код за текущий период => он может получить код
    /// </summary>
    /// <returns></returns>
    public bool CanGetCode()
    {
        int index = GetIndexNumber();
        return codes[index] == null;
    }
    public bool GetCode()
    {
        if (!CanGetCode())
            return false;

        lock (UsersRabattCodes.rabattCodesLocker)
        {
            var arr = UsersRabattCodes.AllRabattCodes.Where(s => !s.code.Contains("_used")).ToArray();

            //Если тут false, то свободных кодов не осталось
            if (arr.Length == 0)
                return false;

            int rndNum = Pseudorandom.GetRandom(0, arr.Length - 1);
            string randomCode = arr[rndNum].code;

            int index = GetIndexNumber();
            int periodNumber = GetPeriodNumber();
            codes[index] = randomCode;

            for (int i = 0; i <= UsersRabattCodes.AllRabattCodes.Count - 1; i++)
            {
                if (UsersRabattCodes.AllRabattCodes[i] == arr[rndNum])
                {
                    UsersRabattCodes.AllRabattCodes[i].code += $"_used{periodNumber}";
                    break;
                }
            }
            return true;
        }
    }
    public static int GetPeriodNumber()
    {
        for (int i = 0; i <= periods.Length - 1; i++)
        {
            if ((periods[i] - DateTime.UtcNow).TotalSeconds > 0)
            {
                return (i + 4);
            }
        }
        return 8;
    }
    public static int GetIndexNumber() => GetPeriodNumber() - 4;
}
