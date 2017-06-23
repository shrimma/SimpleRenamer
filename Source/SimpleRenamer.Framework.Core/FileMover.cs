using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// File Mover
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IFileMover" />
    public class FileMover : IFileMover
    {
        private bool? OnMonoCached;
        private IBannerDownloader _bannerDownloader;
        private ILogger _logger;
        private Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMover"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="bannerDownloader">The banner downloader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// logger
        /// or
        /// bannerDownloader
        /// </exception>
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

        /// <summary>
        /// Create the folder structure and downloads banners if configured
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="mapping">The mapping of the file to TVDB</param>
        /// <param name="downloadBanner">Whether to download a banner</param>
        /// <returns></returns>
        public async Task<MatchedFile> CreateDirectoriesAndDownloadBannersAsync(MatchedFile episode, Mapping mapping, bool downloadBanner)
        {
            _logger.TraceMessage($"Creating Directories for {episode.SourceFilePath}", EventLevel.Verbose);
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
                    _logger.TraceMessage($"Created Directory {seasonDirectory}", EventLevel.Verbose);
                }

                try
                {
                    if (downloadBanner)
                    {
                        _logger.TraceMessage($"Downloading images for {episode.SourceFilePath}", EventLevel.Verbose);
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
                string folderName = episode.Year > 0 ? $"{episode.ShowName} ({episode.Year})" : $"{episode.ShowName}";
                string movieDirectory = Path.Combine(settings.DestinationFolderMovie, folderName);
                episode.DestinationFilePath = Path.Combine(movieDirectory, episode.NewFileName + ext);
                //create our destination folder if it doesn't already exist
                if (!Directory.Exists(movieDirectory))
                {
                    Directory.CreateDirectory(movieDirectory);
                    _logger.TraceMessage($"Created Directory {movieDirectory}", EventLevel.Verbose);
                }
            }

            _logger.TraceMessage($"Created Directories and Downloaded Banners for {episode.SourceFilePath}.", EventLevel.Verbose);
            return episode;
        }

        /// <summary>
        /// Moves a file to the destination file path. If configured will also rename the file.
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="ct">The cancellationtoken.</param>
        /// <returns></returns>
        public async Task<bool> MoveFileAsync(MatchedFile episode, CancellationToken ct)
        {
            _logger.TraceMessage($"Moving File {episode.SourceFilePath} to {episode.DestinationFilePath}.", EventLevel.Verbose);
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

            _logger.TraceMessage($"Moved File {episode.SourceFilePath} to {episode.DestinationFilePath}.", EventLevel.Verbose);
            return true;
        }

        /// <summary>
        /// Determines if the move is a Quick operation.
        /// </summary>
        /// <param name="fromFile">From file.</param>
        /// <param name="toFile">To file.</param>
        /// <returns></returns>
        private bool QuickOperation(FileInfo fromFile, FileInfo toFile)
        {
            _logger.TraceMessage("QuickOperation - Start", EventLevel.Verbose);
            if ((fromFile == null) || (toFile == null) || (fromFile.Directory == null) || (toFile.Directory == null))
            {
                return false;
            }

            _logger.TraceMessage("QuickOperation - End", EventLevel.Verbose);
            return (settings.RenameFiles && !settings.CopyFiles && (fromFile.Directory.Root.FullName.ToLower() == toFile.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
        }

        /// <summary>
        /// Creates a temporary file name.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        private string TempFileName(FileInfo f)
        {
            return f.FullName + ".simplerenametemp";
        }

        /// <summary>
        /// Checks if file is the same
        /// </summary>
        /// <param name="a">File a.</param>
        /// <param name="b">File b.</param>
        /// <returns></returns>
        private bool FileIsSame(FileInfo a, FileInfo b)
        {
            return String.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        /// <summary>
        /// Keeps the timestamps.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        private void KeepTimestamps(FileInfo from, FileInfo to)
        {
            to.CreationTime = from.CreationTime;
            to.CreationTimeUtc = from.CreationTimeUtc;
            to.LastAccessTime = from.LastAccessTime;
            to.LastAccessTimeUtc = from.LastAccessTimeUtc;
            to.LastWriteTime = from.LastWriteTime;
            to.LastWriteTimeUtc = from.LastWriteTimeUtc;
        }

        /// <summary>
        /// Oses the move rename.
        /// </summary>
        /// <param name="fromFile">From file.</param>
        /// <param name="toFile">To file.</param>
        private void OSMoveRename(FileInfo fromFile, FileInfo toFile)
        {
            _logger.TraceMessage("OSMoveRename - Start", EventLevel.Verbose);
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
            _logger.TraceMessage("OSMoveRename - End", EventLevel.Verbose);
        }

        /// <summary>
        /// Called when [mono].
        /// </summary>
        /// <returns></returns>
        private bool OnMono()
        {
            if (!OnMonoCached.HasValue)
            {
                OnMonoCached = System.Type.GetType("Mono.Runtime") != null;
            }
            return OnMonoCached.Value;
        }

        /// <summary>
        /// Called when [windows].
        /// </summary>
        /// <returns></returns>
        private static bool OnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        /// <summary>
        /// Copies it ourself.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="fromFile">From file.</param>
        /// <param name="toFile">To file.</param>
        /// <param name="ct">The ct.</param>
        private void CopyItOurself(Settings settings, FileInfo fromFile, FileInfo toFile, CancellationToken ct)
        {
            _logger.TraceMessage("CopyItOurself - Start", EventLevel.Verbose);
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

            _logger.TraceMessage("CopyItOurself - End", EventLevel.Verbose);
        }

        /// <summary>
        /// Nicelies the stop and clean up win32.
        /// </summary>
        /// <param name="copier">The copier.</param>
        /// <param name="toFile">To file.</param>
        private void NicelyStopAndCleanUp_Win32(WinFileIO copier, FileInfo toFile)
        {
            copier.Close();
            string tempName = TempFileName(toFile);
            if (File.Exists(tempName))
            {
                File.Delete(tempName);
            }
        }

        /// <summary>
        /// Nicelies the stop and clean up streams.
        /// </summary>
        /// <param name="msr">The MSR.</param>
        /// <param name="msw">The MSW.</param>
        /// <param name="toFile">To file.</param>
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
