using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IFileMatcher" />
    public class FileMatcher : IFileMatcher
    {
        private RegexFile regexExpressions;
        private ILogger _logger;
        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMatcher"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// logger
        /// </exception>
        public FileMatcher(ILogger logger, IConfigurationManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            regexExpressions = configManager.RegexExpressions;
        }

        /// <summary>
        /// Processes a list of files trying to match the filenames against the regular expressions
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="ct">The cancellationtoken.</param>
        /// <returns></returns>
        public async Task<List<MatchedFile>> SearchFilesAsync(List<string> files, CancellationToken ct)
        {
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Parsing file names for show or movie details"));
            object lockList = new object();
            List<MatchedFile> matchedFiles = new List<MatchedFile>();

            //block for searching the files
            var searchFilesAsyncBlock = new TransformBlock<string, MatchedFile>((file) =>
            {
                MatchedFile matchedFile = SearchFileName(file, ct);
                if (matchedFile != null)
                {
                    //if episode is not null then we matched so add to the output list
                    _logger.TraceMessage(string.Format("Matched {0}", matchedFile.SourceFilePath));
                    RaiseProgressEvent(this, new ProgressTextEventArgs($"Grabbed show or movie details from file names"));
                    return matchedFile;
                }
                else
                {
                    //else we couldn't match the file so add a file with just filepath so user can manually match
                    _logger.TraceMessage(string.Format("Couldn't find a match!"));
                    matchedFile = new MatchedFile(file, Path.GetFileNameWithoutExtension(file));
                    RaiseProgressEvent(this, new ProgressTextEventArgs($"Grabbed show or movie details from file names"));
                    return matchedFile;
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            //block for writing the outputs to a list
            var writeOutputBlock = new ActionBlock<MatchedFile>(c => matchedFiles.Add(c));
            //link the writing to completion of search
            searchFilesAsyncBlock.LinkTo(writeOutputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            //post all our files to our dataflow
            foreach (string file in files)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await writeOutputBlock.Completion;

            return matchedFiles;
        }

        /// <summary>
        /// Searches the file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="ct">The cancellationToken.</param>
        /// <returns></returns>
        private MatchedFile SearchFileName(string fileName, CancellationToken ct)
        {
            _logger.TraceMessage("SearchFileNameAsync - Start");
            string showname = null;
            string season = null;
            string episode = null;
            string movieTitle = null;
            string yearString = null;
            int year = 0;

            foreach (RegexExpression exp in regexExpressions.RegexExpressions)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (exp.IsEnabled)
                    {
                        //if this expression is enabled then match against the filename
                        Regex regexStandard = new Regex(exp.Expression, RegexOptions.IgnoreCase);
                        Match tvshow = regexStandard.Match(Path.GetFileNameWithoutExtension(fileName));

                        //match for tv show regexp
                        if (exp.IsForTvShow)
                        {
                            showname = GetTrueShowName(tvshow.Groups["series_name"].Value);
                            season = tvshow.Groups["season_num"].Value;
                            episode = tvshow.Groups["ep_num"].Value;

                            if (!string.IsNullOrEmpty(showname) && !string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(episode))
                            {
                                //if we found a showname, season, and episode in the filename then this is a match
                                _logger.TraceMessage("SearchFileNameAsync - Found showname, season, and episode in file name");
                                return new MatchedFile(fileName, showname, season, episode);
                            }
                        }
                        //else match for movie regexp
                        else
                        {
                            movieTitle = GetTrueShowName(tvshow.Groups["movie_title"].Value);
                            yearString = tvshow.Groups["movie_year"].Value;
                            int.TryParse(yearString, out year);

                            if (!string.IsNullOrEmpty(movieTitle) && !string.IsNullOrEmpty(yearString))
                            {
                                //if we found a movie title and year then this is a match
                                _logger.TraceMessage("SearchFileNameAsync - Found movietitle, and year in file name");
                                return new MatchedFile(fileName, movieTitle, year);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //we don't really care if one of the regex fails so swallow this exception
                    _logger.TraceException(ex, $"The RegularExpression {exp.Expression} failed on {fileName}.");
                }
                ct.ThrowIfCancellationRequested();
            }

            _logger.TraceMessage("SearchFileName - No regex could match the file - End");
            return null;
        }

        /// <summary>
        /// Gets the name of the true show.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string GetTrueShowName(string input)
        {
            _logger.TraceMessage("GetTrueShowName - Start", EventLevel.Verbose);
            string output = null;
            string[] words = input.Split('.');
            int i = 1;
            foreach (string word in words)
            {
                if (IsJoiningWord(word.ToLowerInvariant()) && i > 1)
                {
                    output += word + " ";
                }
                else
                {
                    output += System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word) + " ";
                }
                i++;
            }

            _logger.TraceMessage("GetTrueShowName - End", EventLevel.Verbose);
            return output.Trim();
        }

        /// <summary>
        /// Determines whether [is joining word] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if [is joining word] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsJoiningWord(string input)
        {
            _logger.TraceMessage("IsJoiningWord - Start", EventLevel.Verbose);
            foreach (string word in JoiningWords)
            {
                if (input.Equals(word.ToLowerInvariant()))
                {
                    _logger.TraceMessage("IsJoiningWord - True", EventLevel.Verbose);
                    return true;
                }
            }
            _logger.TraceMessage("IsJoiningWord - False", EventLevel.Verbose);
            return false;
        }
        private string[] JoiningWords
        {
            get { return joiningWords.Split(','); }
        }

        /// <summary>
        /// The joining words
        /// </summary>
        private string joiningWords = "the,of,and";
    }
}
