
namespace SimpleRenamer.Framework
{
    public class FileMoveResult
    {
        public bool Success { get; set; }
        public TVEpisode Episode { get; set; }

        public FileMoveResult(bool success, TVEpisode episode)
        {
            Success = success;
            Episode = episode;
        }
    }
}
