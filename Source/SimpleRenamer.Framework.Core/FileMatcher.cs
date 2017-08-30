using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
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
        private RegexFile _regexExpressions;
        private List<(Regex regex, bool isForTv)> _activeRegex;
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
            _regexExpressions = configManager.RegexExpressions;
        }

        /// <summary>
        /// Processes a list of files trying to match the filenames against the regular expressions
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="ct">The cancellationtoken.</param>
        /// <returns></returns>
        public async Task<List<MatchedFile>> SearchFilesAsync(List<string> files, CancellationToken ct)
        {
            _logger.TraceMessage($"Parsing all found files against the regular expressions.", EventLevel.Verbose);
            OnProgressTextChanged(new ProgressTextEventArgs($"Parsing file names for show or movie details"));
            ConcurrentBag<MatchedFile> matchedFiles = new ConcurrentBag<MatchedFile>();

            //grab the current active regularexpressions
            Initialize();

            //block for searching the files
            var searchFilesAsyncBlock = new ActionBlock<string>((file) =>
            {
                MatchedFile matchedFile = SearchFileName(file, ct);
                if (matchedFile != null)
                {
                    //if episode is not null then we matched so add to the output list                    
                    OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Matched file {0} with one of the regular expressions", matchedFile.SourceFilePath)));
                    matchedFiles.Add(matchedFile);
                }
                else
                {
                    //else we couldn't match the file so add a file with just filepath so user can manually match                    
                    matchedFile = new MatchedFile(file, Path.GetFileNameWithoutExtension(file));
                    OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Couldn't find a match for {0}!", file)));
                    matchedFiles.Add(matchedFile);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            //post all our files to our dataflow
            foreach (string file in files)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await searchFilesAsyncBlock.Completion;

            _logger.TraceMessage($"Parsed all found files against the regular expressions.", EventLevel.Verbose);
            return matchedFiles.ToList();
        }

        private void Initialize()
        {
            //add only the active regexp
            _activeRegex = new List<(Regex regex, bool isForTv)>();
            foreach (RegexExpression exp in _regexExpressions.RegexExpressions)
            {
                if (exp.IsEnabled)
                {
                    _activeRegex.Add((new Regex(exp.Expression, RegexOptions.IgnoreCase), exp.IsForTvShow));
                }
            }
        }

        /// <summary>
        /// Searches the file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="ct">The cancellationToken.</param>
        /// <returns></returns>
        private MatchedFile SearchFileName(string fileName, CancellationToken ct)
        {
            _logger.TraceMessage($"Checking {fileName} against regular expressions.", EventLevel.Verbose);
            string showname = null;
            string season = null;
            string episode = null;
            string movieTitle = null;
            string yearString = null;
            int year = 0;

            foreach ((Regex regex, bool isForTv) regEx in _activeRegex)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    //if this expression is enabled then match against the filename                    
                    Match fileMatch = regEx.regex.Match(Path.GetFileNameWithoutExtension(fileName));

                    //match for tv show regexp
                    if (regEx.isForTv)
                    {
                        showname = SanitizeFileName(fileMatch.Groups["series_name"].Value);
                        season = fileMatch.Groups["season_num"].Value;
                        episode = fileMatch.Groups["ep_num"].Value;

                        if (!string.IsNullOrEmpty(showname) && !string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(episode))
                        {
                            //if we found a showname, season, and episode in the filename then this is a match
                            _logger.TraceMessage($"Matched show details for {fileName}.", EventLevel.Verbose);
                            return new MatchedFile(fileName, showname, season, episode);
                        }
                    }
                    //else match for movie regexp
                    else
                    {
                        movieTitle = SanitizeFileName(fileMatch.Groups["movie_title"].Value);
                        yearString = fileMatch.Groups["movie_year"].Value;
                        int.TryParse(yearString, out year);

                        if (!string.IsNullOrEmpty(movieTitle) && !string.IsNullOrEmpty(yearString))
                        {
                            //if we found a movie title and year then this is a match
                            _logger.TraceMessage($"Matched movie details for {fileName}.", EventLevel.Verbose);
                            return new MatchedFile(fileName, movieTitle, year);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //we don't really care if one of the regex fails so swallow this exception
                    _logger.TraceException(ex, $"The RegularExpression {regEx.regex.ToString()} failed on {fileName}.");
                }
                ct.ThrowIfCancellationRequested();
            }

            _logger.TraceMessage($"No regex could match {fileName}.", EventLevel.Warning);
            return null;
        }

        /// <summary>
        /// Removes fullstops and correctly cases joining words
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string SanitizeFileName(string input)
        {
            _logger.TraceMessage($"Sanitizing FileName {input}.", EventLevel.Verbose);
            string output = null;
            string[] words = input.Split('.');
            int i = 1;
            foreach (string word in words)
            {
                if (IsJoiningWord(word.ToLowerInvariant()) && i > 1)
                {
                    output += word.ToLowerInvariant() + " ";
                }
                else
                {
                    output += System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word) + " ";
                }
                i++;
            }

            _logger.TraceMessage($"Sanitized FileName {input} to {output.Trim()}.", EventLevel.Verbose);
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
            foreach (string word in JoiningWords)
            {
                if (input.Equals(word.ToLowerInvariant()))
                {
                    _logger.TraceMessage($"{input} IsJoiningWord - True", EventLevel.Verbose);
                    return true;
                }
            }
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

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}
