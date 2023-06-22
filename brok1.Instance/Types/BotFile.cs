namespace brok1.Instance.Types
{
    public class BotFile
    {
        public static List<BotFile> AllGIFs = new List<BotFile>()
        {
            new BotFile("3star.mp4"),
            new BotFile("5star.mp4"),
            new BotFile("4star.mp4"), //purple
        };
        public static List<BotFile> AllPhotos = new List<BotFile>()
        {
            new BotFile("1.png"),
        };

        public string file_id { get; set; }
        public string file_name { get; set; }

        public BotFile(string file_name)
        {
            this.file_name = file_name;
        }
    }
}
