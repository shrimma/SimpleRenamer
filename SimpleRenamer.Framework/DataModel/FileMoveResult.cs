
namespace SimpleRenamer.Framework.DataModel
{
    public class FileMoveResult
    {
        public bool Success { get; set; }
        public TVEpisode Episode { get; set; }

        public string DestinationFilePath { get; set; }

        public FileMoveResult(bool success, TVEpisode episode)
        {
            Success = success;
            Episode = episode;
            DestinationFilePath = string.Empty;
        }
    }
}
