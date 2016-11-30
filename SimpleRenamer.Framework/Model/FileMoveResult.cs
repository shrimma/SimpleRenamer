
namespace SimpleRenamer.Common.Model
{
    public class FileMoveResult
    {
        public bool Success { get; set; }
        public MatchedFile Episode { get; set; }

        public string DestinationFilePath { get; set; }

        public FileMoveResult(bool success, MatchedFile episode)
        {
            Success = success;
            Episode = episode;
            DestinationFilePath = string.Empty;
        }
    }
}
