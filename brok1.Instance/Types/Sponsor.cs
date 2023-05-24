namespace brok1.Instance.Types;

public class Sponsor
{
    public static List<Sponsor> AllSponsors = new List<Sponsor>()
    {
        new Sponsor() { channelId = -1001795188948, channelLink = "https://t.me/hey_vadimchik" }
    };

    public long channelId { get; set; }
    public string channelLink { get; set; }
}
