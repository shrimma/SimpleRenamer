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
        private IBannerDownloader _bannerDownloader;
        private ILogger _logger;
        private ISettings _settings;

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
            _settings = configManager.Settings;
            _bannerDownloader = bannerDownloader ?? throw new ArgumentNullException(nameof(bannerDownloader));
        }

        /// <summary>
        /// Create the folder structure and downloads banners if configured
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="mapping">The mapping of the file to TVDB</param>
        /// <param name="downloadBanner">Whether to download a banner</param>
        /// <returns></returns>
        public MatchedFile CreateDirectoriesAndQueueDownloadBanners(MatchedFile episode, Mapping mapping, bool downloadBanner)
        {
            _logger.TraceMessage($"Creating Directories for {episode.SourceFilePath}", EventLevel.Verbose);
            string ext = Path.GetExtension(episode.SourceFilePath);
            if (episode.FileType == FileType.TvShow)
            {
                //use the destination folder and showname etc to define final destination
                string showDirectory = string.Empty;
                if (!string.IsNullOrWhiteSpace(mapping?.CustomFolderName))
                {
                    showDirectory = Path.Combine(_settings.DestinationFolderTV, mapping.CustomFolderName);
                }
                else
                {
                    showDirectory = Path.Combine(_settings.DestinationFolderTV, episode.ShowName);
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
                        if (!string.IsNullOrWhiteSpace(episode.ShowImage) && !File.Exists(Path.Combine(showDirectory, "Folder.jpg")))
                        {
                            //Grab Show banner if required
                            bannerResult = _bannerDownloader.QueueBannerDownload(episode.ShowImage, showDirectory);
                        }
                        if (!string.IsNullOrWhiteSpace(episode.SeasonImage) && !File.Exists(Path.Combine(seasonDirectory, "Folder.jpg")))
                        {
                            //Grab Season banner if required
                            bannerResult = _bannerDownloader.QueueBannerDownload(episode.SeasonImage, seasonDirectory);
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
                string movieDirectory = Path.Combine(_settings.DestinationFolderMovie, folderName);
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
        /// <param name="cancellationToken">The cancellationtoken.</param>
        /// <returns></returns>
        public async Task<bool> MoveFileAsync(MatchedFile episode, CancellationToken cancellationToken)
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
                await CopyItOurselfAsync(fromFile, toFile, cancellationToken);
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
            return (_settings.RenameFiles && !_settings.CopyFiles && (fromFile.Directory.Root.FullName.ToLower() == toFile.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
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
                string tempName = string.Format("{0}.simplerenametemp", toFile.FullName);
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
        /// Copies it ourself.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="fromFile">From file.</param>
        /// <param name="toFile">To file.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        private async Task CopyItOurselfAsync(FileInfo fromFile, FileInfo toFile, CancellationToken cancellationToken)
        {
            _logger.TraceMessage("CopyItOurself - Start", EventLevel.Verbose);

            try
            {
                var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                var bufferSize = 4096;

                using (var sourceStream = new FileStream(fromFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
                {
                    using (var destinationStream = new FileStream(toFile.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions))
                    {
                        await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken);
                    }
                }

                KeepTimestamps(fromFile, toFile);

                // if that was a move/rename, delete the source
                if (!_settings.CopyFiles)
                {
                    fromFile.Delete();
                }
            }
            catch (ThreadAbortException tae)
            {
                _logger.TraceException(tae);
                return;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
                throw;
            }

            _logger.TraceMessage("CopyItOurself - End", EventLevel.Verbose);
        }
    }
}
