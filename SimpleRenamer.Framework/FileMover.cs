
using System;
using System.IO;
using System.Threading;
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

        public static async Task<FileMoveResult> CreateDirectoriesAndDownloadBannersAsync(TVEpisode episode, Settings settings)
        {
            FileMoveResult result = new FileMoveResult(true, episode);
            string ext = Path.GetExtension(episode.FilePath);
            int season;
            int.TryParse(episode.Season, out season);
            //use the destination folder and showname etc to define final destination
            string showDirectory = Path.Combine(settings.DestinationFolder, episode.ShowName);
            string seasonDirectory = Path.Combine(showDirectory, string.Format("Season {0}", season));
            result.DestinationFilePath = Path.Combine(seasonDirectory, episode.NewFileName + ext);

            //create our destination folder if it doesn't already exist
            if (!Directory.Exists(seasonDirectory))
            {
                Directory.CreateDirectory(seasonDirectory);
            }

            try
            {
                //TODO add this as a setting
                if (true)
                {
                    bool bannerResult;
                    if (!string.IsNullOrEmpty(episode.ShowImage) && !File.Exists(Path.Combine(showDirectory, "Folder.jpg")))
                    {
                        //Grab Show banner if required
                        bannerResult = await BannerDownloader.SaveBannerAsync(episode.ShowImage, showDirectory);
                    }
                    if (!string.IsNullOrEmpty(episode.SeasonImage) && !File.Exists(Path.Combine(seasonDirectory, "Folder.jpg")))
                    {
                        //Grab Season banner if required
                        bannerResult = await BannerDownloader.SaveBannerAsync(episode.SeasonImage, seasonDirectory);
                    }
                }
            }
            catch
            {
                //we don't really care if image download failed
            }
            return result;
        }

        public static bool MoveFile(TVEpisode episode, Settings settings, string destinationFilePath)
        {
            try
            {
                Thread.Sleep(2000);
                FileInfo fromFile = new FileInfo(episode.FilePath);
                FileInfo toFile = new FileInfo(destinationFilePath);
                if (QuickOperation(settings, fromFile, toFile))
                {
                    OSMoveRename(fromFile, toFile);
                }
                else
                {
                    CopyItOurself(settings, fromFile, toFile);
                }
                return true;
            }
            catch
            {
                //TODO some error handling would be nice
                return false;
            }
        }

        public static bool QuickOperation(Settings settings, FileInfo fromFile, FileInfo toFile)
        {
            if ((fromFile == null) || (toFile == null) || (fromFile.Directory == null) || (toFile.Directory == null))
            {
                return false;
            }

            return (settings.RenameFiles && !settings.CopyFiles && (fromFile.Directory.Root.FullName.ToLower() == toFile.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
        }

        private static string TempFor(FileInfo f)
        {
            return f.FullName + ".simplerenametemp";
        }

        private static bool Same(FileInfo a, FileInfo b)
        {
            return String.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        private static void KeepTimestamps(FileInfo from, FileInfo to)
        {
            to.CreationTime = from.CreationTime;
            to.CreationTimeUtc = from.CreationTimeUtc;
            to.LastAccessTime = from.LastAccessTime;
            to.LastAccessTimeUtc = from.LastAccessTimeUtc;
            to.LastWriteTime = from.LastWriteTime;
            to.LastWriteTimeUtc = from.LastWriteTimeUtc;
        }

        private static void OSMoveRename(FileInfo fromFile, FileInfo toFile)
        {
            try
            {
                if (Same(fromFile, toFile))
                {
                    // XP won't actually do a rename if its only a case difference
                    string tempName = TempFor(toFile);
                    fromFile.MoveTo(tempName);
                    File.Move(tempName, toFile.FullName);
                }
                else
                {
                    fromFile.MoveTo(toFile.FullName);
                }

                KeepTimestamps(fromFile, toFile);
            }
            catch (System.Exception e)
            {

            }
        }

        private static bool? OnMonoCached;

        public static bool OnMono()
        {
            if (!OnMonoCached.HasValue)
            {
                OnMonoCached = System.Type.GetType("Mono.Runtime") != null;
            }
            return OnMonoCached.Value;
        }

        public static bool OnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private static void CopyItOurself(Settings settings, FileInfo fromFile, FileInfo toFile)
        {
            const int kArrayLength = 1 * 1024 * 1024;
            Byte[] dataArray = new Byte[kArrayLength];

            bool useWin32 = OnWindows() && !OnMono();

            WinFileIO copier = null;

            BinaryReader msr = null;
            BinaryWriter msw = null;

            try
            {
                long thisFileCopied = 0;
                long thisFileSize = fromFile.Length;

                string tempName = TempFor(toFile);
                if (File.Exists(tempName))
                {
                    File.Delete(tempName);
                }

                if (useWin32)
                {
                    copier = new WinFileIO(dataArray);
                    copier.OpenForReading(fromFile.FullName);
                    copier.OpenForWriting(tempName);
                }
                else
                {
                    msr = new BinaryReader(new FileStream(fromFile.FullName, FileMode.Open, FileAccess.Read));
                    msw = new BinaryWriter(new FileStream(tempName, FileMode.CreateNew));
                }

                for (; ; )
                {
                    int n = useWin32 ? copier.ReadBlocks(kArrayLength) : msr.Read(dataArray, 0, kArrayLength);
                    if (n == 0)
                    {
                        break;
                    }

                    if (useWin32)
                    {
                        copier.WriteBlocks(n);
                    }
                    else
                    {
                        msw.Write(dataArray, 0, n);
                    }
                    thisFileCopied += n;

                    double pct = (thisFileSize != 0) ? (100.0 * thisFileCopied / thisFileSize) : 100;
                    if (pct > 100.0)
                    {
                        pct = 100.0;
                    }
                    //TODO for the progress bar
                    //this.PercentDone = pct;
                }

                if (useWin32)
                {
                    copier.Close();
                }
                else
                {
                    msr.Close();
                    msw.Close();
                }

                // rename temp version to final name
                if (toFile.Exists)
                {
                    toFile.Delete(); // outta ma way!
                }
                File.Move(tempName, toFile.FullName);

                KeepTimestamps(fromFile, toFile);

                // if that was a move/rename, delete the source
                if (!settings.CopyFiles)
                {
                    fromFile.Delete();
                }

            } // try
            catch (System.Threading.ThreadAbortException)
            {
                if (useWin32)
                {
                    NicelyStopAndCleanUp_Win32(copier, toFile);
                }
                else
                {
                    NicelyStopAndCleanUp_Streams(msr, msw, toFile);
                }
                return;
            }
            catch (Exception ex)
            {
                if (useWin32)
                {
                    NicelyStopAndCleanUp_Win32(copier, toFile);
                }
                else
                {
                    NicelyStopAndCleanUp_Streams(msr, msw, toFile);
                }
            }
        }

        private static void NicelyStopAndCleanUp_Win32(WinFileIO copier, FileInfo toFile)
        {
            copier.Close();
            string tempName = TempFor(toFile);
            if (File.Exists(tempName))
            {
                File.Delete(tempName);
            }
        }

        private static void NicelyStopAndCleanUp_Streams(BinaryReader msr, BinaryWriter msw, FileInfo toFile)
        {
            if (msw != null)
            {
                msw.Close();
                string tempName = TempFor(toFile);
                if (File.Exists(tempName))
                {
                    File.Delete(tempName);
                }
            }
            if (msr != null)
            {
                msr.Close();
            }
        }
    }
}
