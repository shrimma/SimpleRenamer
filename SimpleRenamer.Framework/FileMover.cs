
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
namespace SimpleRenamer.Framework
{
    public class FileMover : IFileMover
    {
        private bool? OnMonoCached;
        private IBannerDownloader bannerDownloader;
        private ILogger logger;
        private Settings settings;

        public FileMover(IBannerDownloader bannerDl, ILogger log, ISettingsFactory settingsFactory)
        {
            if (bannerDl == null)
            {
                throw new ArgumentNullException(nameof(bannerDl));
            }
            if (settingsFactory == null)
            {
                throw new ArgumentNullException(nameof(settingsFactory));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            bannerDownloader = bannerDl;
            logger = log;
            settings = settingsFactory.GetSettings();
        }

        public async Task<FileMoveResult> CreateDirectoriesAndDownloadBannersAsync(TVEpisode episode, Mapping mapping, bool downloadBanner)
        {
            logger.TraceMessage("CreateDirectoriesAndDownloadBannersAsync - Start");
            FileMoveResult result = new FileMoveResult(true, episode);
            string ext = Path.GetExtension(episode.FilePath);
            int season;
            int.TryParse(episode.Season, out season);
            //use the destination folder and showname etc to define final destination
            string showDirectory = string.Empty;
            if (mapping != null && !string.IsNullOrEmpty(mapping.CustomFolderName))
            {
                showDirectory = Path.Combine(settings.DestinationFolder, mapping.CustomFolderName);
            }
            else
            {
                showDirectory = Path.Combine(settings.DestinationFolder, episode.ShowName);
            }
            string seasonDirectory = Path.Combine(showDirectory, string.Format("Season {0}", season));
            result.DestinationFilePath = Path.Combine(seasonDirectory, episode.NewFileName + ext);

            //create our destination folder if it doesn't already exist
            if (!Directory.Exists(seasonDirectory))
            {
                Directory.CreateDirectory(seasonDirectory);
            }

            try
            {
                if (downloadBanner)
                {
                    bool bannerResult;
                    if (!string.IsNullOrEmpty(episode.ShowImage) && !File.Exists(Path.Combine(showDirectory, "Folder.jpg")))
                    {
                        //Grab Show banner if required                        
                        bannerResult = await bannerDownloader.SaveBannerAsync(episode.ShowImage, showDirectory);
                    }
                    if (!string.IsNullOrEmpty(episode.SeasonImage) && !File.Exists(Path.Combine(seasonDirectory, "Folder.jpg")))
                    {
                        //Grab Season banner if required
                        bannerResult = await bannerDownloader.SaveBannerAsync(episode.SeasonImage, seasonDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
                //we don't really care if image download fails
            }

            logger.TraceMessage("CreateDirectoriesAndDownloadBannersAsync - End");
            return result;
        }

        public async Task<bool> MoveFileAsync(TVEpisode episode, string destinationFilePath)
        {
            logger.TraceMessage("MoveFileAsync - Start");
            try
            {
                FileInfo fromFile = new FileInfo(episode.FilePath);
                FileInfo toFile = new FileInfo(destinationFilePath);
                if (QuickOperation(fromFile, toFile))
                {
                    OSMoveRename(fromFile, toFile);
                }
                else
                {
                    CopyItOurself(settings, fromFile, toFile);
                }

                logger.TraceMessage("MoveFileAsync - End");
                return true;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
                return false;
            }
        }

        private bool QuickOperation(FileInfo fromFile, FileInfo toFile)
        {
            logger.TraceMessage("QuickOperation - Start");
            if ((fromFile == null) || (toFile == null) || (fromFile.Directory == null) || (toFile.Directory == null))
            {
                return false;
            }

            logger.TraceMessage("QuickOperation - End");
            return (settings.RenameFiles && !settings.CopyFiles && (fromFile.Directory.Root.FullName.ToLower() == toFile.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
        }

        private string TempFileName(FileInfo f)
        {
            return f.FullName + ".simplerenametemp";
        }

        private bool FileIsSame(FileInfo a, FileInfo b)
        {
            return String.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        private void KeepTimestamps(FileInfo from, FileInfo to)
        {
            to.CreationTime = from.CreationTime;
            to.CreationTimeUtc = from.CreationTimeUtc;
            to.LastAccessTime = from.LastAccessTime;
            to.LastAccessTimeUtc = from.LastAccessTimeUtc;
            to.LastWriteTime = from.LastWriteTime;
            to.LastWriteTimeUtc = from.LastWriteTimeUtc;
        }

        private void OSMoveRename(FileInfo fromFile, FileInfo toFile)
        {
            logger.TraceMessage("OSMoveRename - Start");
            try
            {
                if (FileIsSame(fromFile, toFile))
                {
                    // XP won't actually do a rename if its only a case difference
                    string tempName = TempFileName(toFile);
                    fromFile.MoveTo(tempName);
                    File.Move(tempName, toFile.FullName);
                }
                else
                {
                    fromFile.MoveTo(toFile.FullName);
                }

                KeepTimestamps(fromFile, toFile);
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
            logger.TraceMessage("OSMoveRename - End");
        }

        private bool OnMono()
        {
            if (!OnMonoCached.HasValue)
            {
                OnMonoCached = System.Type.GetType("Mono.Runtime") != null;
            }
            return OnMonoCached.Value;
        }

        private static bool OnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private void CopyItOurself(Settings settings, FileInfo fromFile, FileInfo toFile)
        {
            logger.TraceMessage("CopyItOurself - Start");
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

                string tempName = TempFileName(toFile);
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

                for (;;)
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
            catch (System.Threading.ThreadAbortException tae)
            {
                logger.TraceException(tae);
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
                logger.TraceException(ex);
                if (useWin32)
                {
                    NicelyStopAndCleanUp_Win32(copier, toFile);
                }
                else
                {
                    NicelyStopAndCleanUp_Streams(msr, msw, toFile);
                }
            }

            logger.TraceMessage("CopyItOurself - Start");
        }

        private void NicelyStopAndCleanUp_Win32(WinFileIO copier, FileInfo toFile)
        {
            copier.Close();
            string tempName = TempFileName(toFile);
            if (File.Exists(tempName))
            {
                File.Delete(tempName);
            }
        }

        private void NicelyStopAndCleanUp_Streams(BinaryReader msr, BinaryWriter msw, FileInfo toFile)
        {
            if (msw != null)
            {
                msw.Close();
                string tempName = TempFileName(toFile);
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
