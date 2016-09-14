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
    public class PerformActionsOnShows : IPerformActionsOnShows
    {
        private ILogger logger;
        private ITVShowMatcher tvShowMatcher;
        private IBackgroundQueue backgroundQueue;
        private IFileMover fileMover;
        private Settings settings;
        public event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;
        public event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;

        public PerformActionsOnShows(ILogger log, ITVShowMatcher showMatch, IBackgroundQueue backgroundQ, IFileMover fileMove, ISettingsFactory settingsFactory)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            logger = log;
            tvShowMatcher = showMatch;
            backgroundQueue = backgroundQ;
            fileMover = fileMove;
            settings = settingsFactory.GetSettings();
        }

        public async Task Action(ObservableCollection<TVEpisode> scannedEpisodes, CancellationToken ct)
        {
            List<FileMoveResult> filesToMove = await PreProcessTVShows(scannedEpisodes.Where(x => x.ActionThis == true).ToList(), ct);

            if (filesToMove != null && filesToMove.Count > 0)
            {
                await MoveTVShows(filesToMove, ct);
            }
        }

        private async Task<List<FileMoveResult>> PreProcessTVShows(List<TVEpisode> scannedEpisodes, CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            List<ShowSeason> uniqueShowSeasons = new List<ShowSeason>();
            List<FileMoveResult> ProcessFiles = new List<FileMoveResult>();
            ShowNameMapping snm = await tvShowMatcher.ReadMappingFileAsync();
            try
            {
                foreach (TVEpisode ep in scannedEpisodes)
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
                            FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, false));
                            if (result.Success)
                            {
                                ProcessFiles.Add(result);
                                logger.TraceMessage(string.Format("Successfully processed file without banners: {0}", result.Episode.FilePath));
                            }
                        }
                        else
                        {
                            ct.ThrowIfCancellationRequested();
                            FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, true));
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
                        FileMoveResult result = await await backgroundQueue.QueueTask(() => fileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, false));
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

        private async Task<bool> MoveTVShows(List<FileMoveResult> filesToMove, CancellationToken ct)
        {
            try
            {
                //actually move/copy the files one at a time
                foreach (FileMoveResult fmr in filesToMove)
                {
                    ct.ThrowIfCancellationRequested();
                    bool result = await await backgroundQueue.QueueTask(() => fileMover.MoveFileAsync(fmr.Episode, fmr.DestinationFilePath));
                    if (result)
                    {
                        RaiseFileMovedEvent(this, new FileMovedEventArgs(fmr.Episode));
                        logger.TraceMessage(string.Format("Successfully {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copied" : "moved"));
                    }
                    else
                    {
                        logger.TraceMessage(string.Format("Failed to {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copy" : "move"));
                    }
                }

                //add a bit of delay before the progress bar disappears
                await backgroundQueue.QueueTask(() => Thread.Sleep(500));
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }

            return true;
        }
    }
}
