using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class ActionMatchedFiles : IActionMatchedFiles
    {
        private ILogger _logger;
        private IBackgroundQueue _backgroundQueue;
        private IFileMover _fileMover;
        private IConfigurationManager _configurationManager;
        private Settings settings;
        public event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;
        public event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public ActionMatchedFiles(ILogger logger, IBackgroundQueue backgroundQueue, IFileMover fileMover, IConfigurationManager configManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backgroundQueue = backgroundQueue ?? throw new ArgumentNullException(nameof(backgroundQueue));
            _fileMover = fileMover ?? throw new ArgumentNullException(nameof(fileMover));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            settings = _configurationManager.Settings;
        }

        public async Task<bool> Action(ObservableCollection<MatchedFile> scannedEpisodes, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Creating directory structure and downloading any missing banners"));
                //perform pre actions on TVshows
                List<MatchedFile> tvShowsToMove = await PreProcessTVShows(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.TvShow).ToList(), ct);
                //perform pre actions on movies
                List<MatchedFile> moviesToMove = await PreProcessMovies(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.Movie).ToList(), ct);
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Finished creating directory structure and downloading banners."));

                //concat final list of files to move
                List<MatchedFile> filesToMove = new List<MatchedFile>();
                if (tvShowsToMove != null && tvShowsToMove.Count > 0)
                {
                    filesToMove.AddRange(tvShowsToMove);
                }
                if (moviesToMove != null && moviesToMove.Count > 0)
                {
                    filesToMove.AddRange(moviesToMove);
                }

                //move these files
                if (filesToMove != null && filesToMove.Count > 0)
                {
                    await MoveFiles(filesToMove, ct);
                }

                return true;
            });
        }

        private async Task<List<MatchedFile>> PreProcessTVShows(List<MatchedFile> scannedEpisodes, CancellationToken ct)
        {
            List<ShowSeason> uniqueShowSeasons = new List<ShowSeason>();
            List<MatchedFile> ProcessFiles = new List<MatchedFile>();
            ShowNameMapping snm = _configurationManager.ShowNameMappings;
            foreach (MatchedFile ep in scannedEpisodes)
            {
                if (settings.RenameFiles)
                {
                    Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(ep.TVDBShowId)).FirstOrDefault();
                    //check if this show season combo is already going to be processed
                    ShowSeason showSeason = new ShowSeason(ep.ShowName, ep.Season);
                    bool alreadyGrabbedBanners = false;
                    foreach (ShowSeason unique in uniqueShowSeasons)
                    {
                        if (unique.Season.Equals(showSeason.Season) && unique.Show.Equals(showSeason.Show))
                        {
                            alreadyGrabbedBanners = true;
                            break;
                        }
                    }
                    if (alreadyGrabbedBanners)
                    {
                        //if we have already processed this show season combo then dont download the banners again
                        MatchedFile result = await await _backgroundQueue.QueueTask(() => _fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, false));
                        if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                        {
                            ProcessFiles.Add(result);
                            _logger.TraceMessage(string.Format("Successfully processed file without banners: {0}", result.SourceFilePath));
                        }
                        else
                        {
                            _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                        }
                    }
                    else
                    {
                        MatchedFile result = await await _backgroundQueue.QueueTask(() => _fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, true));
                        if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                        {
                            ProcessFiles.Add(result);
                            uniqueShowSeasons.Add(showSeason);
                            _logger.TraceMessage(string.Format("Successfully processed file and downloaded banners: {0}", result.SourceFilePath));
                        }
                        else
                        {
                            _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                        }
                    }
                }
                else
                {
                    MatchedFile result = await await _backgroundQueue.QueueTask(() => _fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false));
                    if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                    {
                        ProcessFiles.Add(result);
                        _logger.TraceMessage(string.Format("Successfully processed file without renaming: {0}", result.SourceFilePath));
                    }
                    else
                    {
                        _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                    }
                }
                ct.ThrowIfCancellationRequested();
                //fire event here
                RaiseFilePreProcessedEvent(this, new FilePreProcessedEventArgs());
            }

            return ProcessFiles;
        }

        private async Task<List<MatchedFile>> PreProcessMovies(List<MatchedFile> scannedMovies, CancellationToken ct)
        {
            List<MatchedFile> ProcessFiles = new List<MatchedFile>();
            foreach (MatchedFile ep in scannedMovies)
            {
                if (settings.RenameFiles)
                {
                    MatchedFile result = await await _backgroundQueue.QueueTask(() => _fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false));
                    if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                    {
                        ProcessFiles.Add(result);
                        _logger.TraceMessage(string.Format("Successfully processed file and downloaded banners: {0}", result.SourceFilePath));
                    }
                    else
                    {
                        _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                    }

                }
                else
                {
                    MatchedFile result = await await _backgroundQueue.QueueTask(() => _fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false));
                    if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                    {
                        ProcessFiles.Add(result);
                        _logger.TraceMessage(string.Format("Successfully processed file without renaming: {0}", result.SourceFilePath));
                    }
                    else
                    {
                        _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                    }
                }
                //fire event here
                RaiseFilePreProcessedEvent(this, new FilePreProcessedEventArgs());
                ct.ThrowIfCancellationRequested();
            }
            return ProcessFiles;
        }

        private async Task<bool> MoveFiles(List<MatchedFile> filesToMove, CancellationToken ct)
        {
            //actually move/copy the files one at a time
            foreach (MatchedFile file in filesToMove)
            {
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Moving file {file.SourceFilePath} to {file.DestinationFilePath}."));
                bool result = await await _backgroundQueue.QueueTask(() => _fileMover.MoveFileAsync(file, ct));
                if (result)
                {
                    RaiseFileMovedEvent(this, new FileMovedEventArgs(file));
                    RaiseProgressEvent(this, new ProgressTextEventArgs($"Finished {file.DestinationFilePath}."));
                    _logger.TraceMessage(string.Format("Successfully {2} {0} to {1}", file.SourceFilePath, file.DestinationFilePath, settings.CopyFiles ? "copied" : "moved"));
                }
                else
                {
                    _logger.TraceMessage(string.Format("Failed to {2} {0} to {1}", file.SourceFilePath, file.DestinationFilePath, settings.CopyFiles ? "copy" : "move"));
                }
                ct.ThrowIfCancellationRequested();
            }

            return true;
        }
    }
}
