
using System.IO;
using System.Threading.Tasks;
namespace SimpleRenamer.Framework
{
    public static class FileMover
    {
        public static bool MoveFile(TVEpisode episode, Settings settings)
        {
            string ext = Path.GetExtension(episode.FilePath);
            int season;
            int.TryParse(episode.Season, out season);
            //use the destination folder and showname etc to define final destination
            string destinationDirectory = Path.Combine(settings.DestinationFolder, episode.ShowName, string.Format("Season {0}", season));
            string destinationFilePath = Path.Combine(destinationDirectory, episode.NewFileName + ext);

            //create our destinaion folder if it doesn't already exist
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            try
            {
                //copy the file if settings want this
                if (settings.CopyFiles)
                {
                    File.Copy(episode.FilePath, destinationFilePath);
                }
                else
                {
                    File.Move(episode.FilePath, destinationFilePath);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static async Task<FileMoveResult> MoveFileAsync(TVEpisode episode, Settings settings)
        {
            FileMoveResult result = new FileMoveResult(true, episode);
            string ext = Path.GetExtension(episode.FilePath);
            int season;
            int.TryParse(episode.Season, out season);
            //use the destination folder and showname etc to define final destination
            string showDirectory = Path.Combine(settings.DestinationFolder, episode.ShowName);
            string seasonDirectory = Path.Combine(showDirectory, string.Format("Season {0}", season));
            string destinationFilePath = Path.Combine(seasonDirectory, episode.NewFileName + ext);

            //create our destinaion folder if it doesn't already exist
            if (!Directory.Exists(seasonDirectory))
            {
                Directory.CreateDirectory(seasonDirectory);
            }

            if (true)
            {
                bool bannerResult;
                if (!string.IsNullOrEmpty(episode.ShowImage))
                {
                    //Grab Show banner if required
                    bannerResult = await BannerDownloader.SaveBannerAsync(episode.ShowImage, showDirectory);
                }
                if (!string.IsNullOrEmpty(episode.SeasonImage))
                {
                    //Grab Season banner if required
                    bannerResult = await BannerDownloader.SaveBannerAsync(episode.SeasonImage, seasonDirectory);
                }
            }

            try
            {
                //copy the file if settings want this
                if (settings.CopyFiles)
                {
                    File.Copy(episode.FilePath, destinationFilePath);
                }
                else
                {
                    File.Move(episode.FilePath, destinationFilePath);
                }
            }
            catch
            {
                result.Success = false;
            }
            return result;
        }
    }
}
