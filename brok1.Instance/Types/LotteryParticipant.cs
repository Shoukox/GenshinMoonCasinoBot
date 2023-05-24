namespace brok1.Instance.Types;

public class LotteryParticipant
{
    public long participantId { get; set; }
    public BotUser user { get; set; }
    public long ticketId { get; set; }
    public long lotteryId { get; set; }

    public LotteryParticipant()
    {
        user = new BotUser();
    }
}
