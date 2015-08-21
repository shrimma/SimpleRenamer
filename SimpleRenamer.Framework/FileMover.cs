
using System.IO;
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

            }
            return true;
        }
    }
}
