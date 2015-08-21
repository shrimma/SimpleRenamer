using SimpleRenamer.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleRenamer.ConsoleApp
{
    public class Program
    {
        public static Settings settings;
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            SetSettings();
            List<string> videoFiles = FileWatcher.SearchTheseFolders(settings);
            MatchTVShows(videoFiles, settings);
            Console.WriteLine("Ending");
            Console.ReadLine();
        }

        public static void MatchTVShows(List<string> videoFiles, Settings settings)
        {
            bool result = false;
            TVEpisode tempEp = null;
            foreach (string fileName in videoFiles)
            {
                tempEp = FileMatcher.SearchMe(fileName);
                if (tempEp != null)
                {
                    //now lets scrape the episode name and incorporate this in the filename (if setting allows)
                    if (settings.RenameFiles)
                    {
                        //TODO we need to sort this out bruv
                        LolTemp(tempEp, settings);
                    }
                    else
                    {
                        tempEp.NewFileName = Path.GetFileNameWithoutExtension(tempEp.FilePath);
                    }
                    //and finally lets copy/move the file
                    FileMover.MoveFile(tempEp, settings);
                }
            }
        }

        private static async void LolTemp(TVEpisode episode, Settings settings)
        {
            TVEpisode newEp = await TVShowMatcher.ScrapeDetails(episode, settings);
            Console.WriteLine("Show:{0}, Season:{1},Episode:{2}, Name:{3}", newEp.ShowName, newEp.Season, newEp.Episode, newEp.EpisodeName);
        }

        /// <summary>
        /// Set temp values for the console test
        /// </summary>
        public static void SetSettings()
        {
            settings = new Settings();
            settings.SubDirectories = true;
            settings.RenameFiles = true;
            settings.CopyFiles = true;
            settings.NewFileNameFormat = "{ShowName} - S{Season}E{Episode} - {EpisodeName}}";
            settings.ValidExtensions = new List<string>();
            settings.ValidExtensions.Add(".avi");
            settings.ValidExtensions.Add(".mkv");
            settings.WatchFolders = new List<string>();
            settings.WatchFolders.Add(@"C:\Temp\Simple\");
            settings.WatchFolders.Add(@"C:\Temp\Simple2\");
            settings.DestinationFolder = @"C:\Temp\Root\";
        }
    }
}
