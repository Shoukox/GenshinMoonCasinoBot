namespace brok1.Instance.Types
{
    public class AnimationGIF
    {
        public static AnimationGIF[] AllGIFs = new AnimationGIF[]
        {
            new AnimationGIF("3star.mp4"),
            new AnimationGIF("5star.mp4"),
            new AnimationGIF("4star.mp4"), //purple
        };

        public FileStream gifStream;

        public AnimationGIF(string filename)
        {
            this.gifStream = File.OpenRead(filename);
        }
    }
}
