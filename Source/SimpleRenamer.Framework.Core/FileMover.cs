using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class FileMover : IFileMover
    {
        private bool? OnMonoCached;
        private IBannerDownloader _bannerDownloader;
        private ILogger _logger;
        private Settings settings;

        public FileMover(ILogger logger, IConfigurationManager configManager, IBannerDownloader bannerDownloader)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            settings = configManager.Settings;
            _bannerDownloader = bannerDownloader ?? throw new ArgumentNullException(nameof(bannerDownloader));
        }

        public async Task<MatchedFile> CreateDirectoriesAndDownloadBannersAsync(MatchedFile episode, Mapping mapping, bool downloadBanner)
        {
            _logger.TraceMessage("CreateDirectoriesAndDownloadBannersAsync - Start");
            string ext = Path.GetExtension(episode.SourceFilePath);
            if (episode.FileType == FileType.TvShow)
            {
                //use the destination folder and showname etc to define final destination
                string showDirectory = string.Empty;
                if (mapping != null && !string.IsNullOrEmpty(mapping.CustomFolderName))
                {
                    showDirectory = Path.Combine(settings.DestinationFolderTV, mapping.CustomFolderName);
                }
                else
                {
                    showDirectory = Path.Combine(settings.DestinationFolderTV, episode.ShowName);
                }
                string seasonDirectory = Path.Combine(showDirectory, string.Format("Season {0}", episode.Season));
                episode.DestinationFilePath = Path.Combine(seasonDirectory, episode.NewFileName + ext);

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
                            bannerResult = await _bannerDownloader.SaveBannerAsync(episode.ShowImage, showDirectory);
                        }
                        if (!string.IsNullOrEmpty(episode.SeasonImage) && !File.Exists(Path.Combine(seasonDirectory, "Folder.jpg")))
                        {
                            //Grab Season banner if required
                            bannerResult = await _bannerDownloader.SaveBannerAsync(episode.SeasonImage, seasonDirectory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.TraceException(ex);
                    //we don't really care if image download fails
                }
            }
            else if (episode.FileType == FileType.Movie)
            {
                string movieDirectory = Path.Combine(settings.DestinationFolderMovie, $"{episode.ShowName} ({episode.Year})");
                episode.DestinationFilePath = Path.Combine(movieDirectory, episode.NewFileName + ext);
                //create our destination folder if it doesn't already exist
                if (!Directory.Exists(movieDirectory))
                {
                    Directory.CreateDirectory(movieDirectory);
                }
            }

            _logger.TraceMessage("CreateDirectoriesAndDownloadBannersAsync - End");
            return episode;
        }

        public async Task<bool> MoveFileAsync(MatchedFile episode, CancellationToken ct)
        {
            _logger.TraceMessage("MoveFileAsync - Start");
            FileInfo fromFile = new FileInfo(episode.SourceFilePath);
            FileInfo toFile = new FileInfo(episode.DestinationFilePath);
            if (QuickOperation(fromFile, toFile))
            {
                OSMoveRename(fromFile, toFile);
            }
            else
            {
                CopyItOurself(settings, fromFile, toFile, ct);
            }

            _logger.TraceMessage("MoveFileAsync - End");
            return true;
        }

        private bool QuickOperation(FileInfo fromFile, FileInfo toFile)
        {
            _logger.TraceMessage("QuickOperation - Start");
            if ((fromFile == null) || (toFile == null) || (fromFile.Directory == null) || (toFile.Directory == null))
            {
                return false;
            }

            _logger.TraceMessage("QuickOperation - End");
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
            _logger.TraceMessage("OSMoveRename - Start");
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
            _logger.TraceMessage("OSMoveRename - End");
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

        private void CopyItOurself(Settings settings, FileInfo fromFile, FileInfo toFile, CancellationToken ct)
        {
            _logger.TraceMessage("CopyItOurself - Start");
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
                    ct.ThrowIfCancellationRequested();
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
                _logger.TraceException(tae);
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
            catch (OperationCanceledException oce)
            {
                _logger.TraceException(oce);
                if (useWin32)
                {
                    NicelyStopAndCleanUp_Win32(copier, toFile);
                }
                else
                {
                    NicelyStopAndCleanUp_Streams(msr, msw, toFile);
                }
                //rethrow the cancellation
                throw;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
                if (useWin32)
                {
                    NicelyStopAndCleanUp_Win32(copier, toFile);
                }
                else
                {
                    NicelyStopAndCleanUp_Streams(msr, msw, toFile);
                }
                throw;
            }

            _logger.TraceMessage("CopyItOurself - End");
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
