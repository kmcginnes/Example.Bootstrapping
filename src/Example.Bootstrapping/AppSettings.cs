namespace Example.Bootstrapping
{
    public interface IAppSettings
    {
        string DataDirectory { get; }
        string FfmpegPath { get; }
        int DefaultVolume { get; }
    }

    public class AppSettings : IAppSettings
    {
        public string DataDirectory { get; set; }
        public string FfmpegPath { get; set; }
        public int DefaultVolume { get; set; }
    }
}