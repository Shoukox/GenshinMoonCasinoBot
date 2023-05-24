using brok1.Instance.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Types;

public class BotUser
{
    public static List<BotUser> AllUsers = new List<BotUser>();
    public static ChatId[] ADMINS = new ChatId[] { 728384906, 358798501 };

    public long userid { get; set; }
    public string username { get; set; }
    public double balance { get; set; }
    public double moneyadded { get; set; }
    public double moneyused { get; set; }
    public List<Spin> spinsList { get; set; } //see spinsList.Add(...)
    public int freeSpinsUsedAfterWin { get; set; }

    public Queue<DateTime> dateTimeOfLastFiveMessages = new Queue<DateTime>(5);
    public DateTime bannedUntil;


    public bool notifyEnabled { get; set; }
    public bool hasPayed
    {
        get
        {
            return moneyadded != 0;
        }
    }
    public Pseudorandom pseudorandom { get; set; }
    public bool wasNotified = false;
    public DateTime lastFreeSpin { get; set; }
    public DateTime nextFreeSpin
    {
        get
        {
            return lastFreeSpin.AddHours(24 - lastFreeSpin.Hour).AddMinutes(60 - lastFreeSpin.Minute).AddSeconds(60-lastFreeSpin.Second);
        }
    }
    public EStage stage { get; set; }
    public EWithdrawing withdrawing { get; set; }
    public AdminPanel adminPanel { get; set; }
    public PayData paydata { get; set; }
    private int _spins { get; set; }
    public int spins
    {
        get
        {
            if (spinsList.Count > 0 && _spins == 0)
            {
                return spinsList.Select(m => m.count).Sum();
            }
            else
            {
                return _spins;
            }
        }
        set
        {
            _spins = value;
        }

    }
    public int moons { get; set; }
    public int crystals { get; set; }
    public bool PayProcessStarted { get; set; } //paycontroller //other.userspay
    public bool isSpinning { get; set; }
    public bool stoppedBot { get; set; }
    public bool wasRecentlyAdded = false;
    public bool isWishing = false;
    public bool isFeedbacking = false;
    public bool canFreeSpin
    {
        get
        {
            return DateTime.Now >= nextFreeSpin;
        }
    }
    public int referalUsersCount { get; set; }
    public DateTime lastInvitedReferal { get; set; }
    public BotUser()
    {
        pseudorandom = new Pseudorandom(0.1 + freeSpinsUsedAfterWin * 0.1);
        paydata = new PayData();
        adminPanel = new AdminPanel();
        spinsList = new List<Spin>();
        rabatt_codes = new UsersRabattCodes();
    }

    public Message adminMessageToSend = default;
    public InlineKeyboardMarkup adminIkToSend = default;

    public UsersRabattCodes rabatt_codes;
}
