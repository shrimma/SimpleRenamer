using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class ActionMatchedFiles : IPerformActionsOnShows
    {
        private ILogger logger;
        private ITVShowMatcher tvShowMatcher;
        private IBackgroundQueue backgroundQueue;
        private IFileMover fileMover;
        private IConfigurationManager configurationManager;
        private Settings settings;
        public event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;
        public event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public ActionMatchedFiles(ILogger log, ITVShowMatcher showMatch, IBackgroundQueue backgroundQ, IFileMover fileMove, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (showMatch == null)
            {
                throw new ArgumentNullException(nameof(showMatch));
            }
            if (backgroundQ == null)
            {
                throw new ArgumentNullException(nameof(backgroundQ));
            }
            if (fileMove == null)
            {
                throw new ArgumentNullException(nameof(fileMove));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            logger = log;
            tvShowMatcher = showMatch;
            backgroundQueue = backgroundQ;
            fileMover = fileMove;
            configurationManager = configManager;
            settings = configurationManager.Settings;
        }

        public async Task<bool> Action(ObservableCollection<MatchedFile> scannedEpisodes, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Creating directory structure and downloading any missing banners"));
                //perform pre actions on TVshows
                List<FileMoveResult> tvShowsToMove = await PreProcessTVShows(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.TvShow).ToList(), ct);
                //perform pre actions on movies
                List<FileMoveResult> moviesToMove = await PreProcessMovies(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.Movie).ToList(), ct);
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Finished creating directory structure and downloading banners."));

                //concat final list of files to move
                List<FileMoveResult> filesToMove = new List<FileMoveResult>();
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

        private async Task<List<FileMoveResult>> PreProcessTVShows(List<MatchedFile> scannedEpisodes, CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            List<ShowSeason> uniqueShowSeasons = new List<ShowSeason>();
            List<FileMoveResult> ProcessFiles = new List<FileMoveResult>();
            ShowNameMapping snm = configurationManager.ShowNameMappings;
            try
            {
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
                            FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, false, ct));
                            if (result.Success)
                            {
                                ProcessFiles.Add(result);
                                logger.TraceMessage(string.Format("Successfully processed file without banners: {0}", result.Episode.FilePath));
                            }
                        }
                        else
                        {
                            ct.ThrowIfCancellationRequested();
                            FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, true, ct));
                            if (result.Success)
                            {
                                ProcessFiles.Add(result);
                                uniqueShowSeasons.Add(showSeason);
                                logger.TraceMessage(string.Format("Successfully processed file and downloaded banners: {0}", result.Episode.FilePath));
                            }
                            else
                            {
                                logger.TraceMessage(string.Format("Failed to process {0}", result.Episode.FilePath));
                            }
                        }
                    }
                    else
                    {
                        FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false, ct));
                        if (result.Success)
                        {
                            ProcessFiles.Add(result);
                            logger.TraceMessage(string.Format("Successfully processed file without renaming: {0}", result.Episode.FilePath));
                        }
                    }
                    //fire event here
                    RaiseFilePreProcessedEvent(this, new FilePreProcessedEventArgs());
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }

            return ProcessFiles;
        }

        private async Task<List<FileMoveResult>> PreProcessMovies(List<MatchedFile> scannedMovies, CancellationToken ct)
        {
            List<FileMoveResult> ProcessFiles = new List<FileMoveResult>();
            try
            {
                foreach (MatchedFile ep in scannedMovies)
                {
                    if (settings.RenameFiles)
                    {
                        ct.ThrowIfCancellationRequested();
                        FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false, ct));
                        if (result.Success)
                        {
                            ProcessFiles.Add(result);
                            logger.TraceMessage(string.Format("Successfully processed file and downloaded banners: {0}", result.Episode.FilePath));
                        }
                        else
                        {
                            logger.TraceMessage(string.Format("Failed to process {0}", result.Episode.FilePath));
                        }

                    }
                    else
                    {
                        FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false, ct));
                        if (result.Success)
                        {
                            ProcessFiles.Add(result);
                            logger.TraceMessage(string.Format("Successfully processed file without renaming: {0}", result.Episode.FilePath));
                        }
                    }
                    //fire event here
                    RaiseFilePreProcessedEvent(this, new FilePreProcessedEventArgs());
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
            return ProcessFiles;
        }

        private async Task<bool> MoveFiles(List<FileMoveResult> filesToMove, CancellationToken ct)
        {
            try
            {
                //actually move/copy the files one at a time
                foreach (FileMoveResult fmr in filesToMove)
                {
                    RaiseProgressEvent(this, new ProgressTextEventArgs($"Moving file {fmr.Episode.FilePath} to {fmr.DestinationFilePath}."));
                    ct.ThrowIfCancellationRequested();
                    bool result = await await backgroundQueue.QueueTask(() => fileMover.MoveFileAsync(fmr.Episode, fmr.DestinationFilePath));
                    if (result)
                    {
                        RaiseFileMovedEvent(this, new FileMovedEventArgs(fmr.Episode));
                        RaiseProgressEvent(this, new ProgressTextEventArgs($"Finished {fmr.DestinationFilePath}."));
                        logger.TraceMessage(string.Format("Successfully {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copied" : "moved"));
                    }
                    else
                    {
                        logger.TraceMessage(string.Format("Failed to {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copy" : "move"));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }

            return true;
        }
    }
}
