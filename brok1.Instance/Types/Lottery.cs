using brok1.Instance.Types.Utils;

namespace brok1.Instance.Types;

public class Lottery
{
    public static List<Lottery> AllLotteries = new List<Lottery>();
    public static Lottery LotteryNow => AllLotteries.LastOrDefault();
    public static object lotteryLocker = new object();

    public long lotteryId { get; set; }
    public List<LotteryParticipant> lotteryParticipants { get; set; }
    public Types.LotteryParticipant lotteryWinner { get; set; }
    public bool isWorking = false;

    public System.Timers.Timer timer;

    private int _lotteryParticipantsCount;
    public int lotteryParticipantsCount
    {
        get
        {
            if (lotteryParticipants == null)
                return _lotteryParticipantsCount;
            else
                return lotteryParticipants.Count;
        }
        set
        {
            _lotteryParticipantsCount = value;
        }
    }

    private DateTime _lotteryEnd;
    public DateTime lotteryEnd
    {
        get
        {
            return _lotteryEnd;
        }
        set
        {
            _lotteryEnd = value;
            _ = LotteryManager.Check(this);
        }
    }

    public Lottery()
    {
        lotteryParticipants = new List<LotteryParticipant>();
        lotteryWinner = new LotteryParticipant();
    }
}
