
namespace SimpleRenamer.Framework
{
    public class TVEpisode
    {
        public string FilePath { get; set; }
        public string ShowName { get; set; }
        public string Season { get; set; }
        public string Episode { get; set; }
        public string EpisodeName { get; set; }
        public string NewFileName { get; set; }
        public TVEpisode(string filePath, string showName, string season, string episode)
        {
            FilePath = filePath;
            ShowName = showName;
            Season = season;
            Episode = episode;
        }

        public TVEpisode(string filePath, string showName, string season, string episode, string episodeName)
        {
            FilePath = filePath;
            ShowName = showName;
            Season = season;
            Episode = episode;
            EpisodeName = episodeName;
        }
    }
}
