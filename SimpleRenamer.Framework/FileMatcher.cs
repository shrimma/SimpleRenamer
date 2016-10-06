using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class FileMatcher : IFileMatcher
    {
        private RegexFile regexExpressions;
        private ILogger logger;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public FileMatcher(ILogger log, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            logger = log;
            regexExpressions = configManager.RegexExpressions;
        }

        public async Task<List<MatchedFile>> SearchFilesAsync(List<string> files)
        {
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Parsing file names for show or movie details"));
            object lockList = new object();
            List<MatchedFile> episodes = new List<MatchedFile>();
            Parallel.ForEach(files, (file) =>
            {
                MatchedFile episode = SearchFileNameAsync(file).GetAwaiter().GetResult();
                if (episode != null)
                {
                    logger.TraceMessage(string.Format("Matched {0}", episode.EpisodeName));
                    lock (lockList)
                    {
                        episodes.Add(episode);
                    }
                }
                else
                {
                    logger.TraceMessage(string.Format("Couldn't find a match!"));
                    episode = new MatchedFile(file, string.Empty, string.Empty, string.Empty);
                    lock (lockList)
                    {
                        episodes.Add(episode);
                    }
                }
            });
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Grabbed show or movie details from file names"));

            return episodes;
        }

        private async Task<MatchedFile> SearchFileNameAsync(string fileName)
        {
            logger.TraceMessage("SearchFileNameAsync - Start");
            string showname = null;
            string season = null;
            string episode = null;
            string movieTitle = null;
            int year = 0;

            try
            {
                foreach (RegexExpression exp in regexExpressions.RegexExpressions)
                {
                    if (exp.IsEnabled)
                    {
                        //process the file name
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
                                logger.TraceMessage("SearchFileNameAsync - Found showname, season, and episode in file name");
                                return new MatchedFile(fileName, showname, season, episode);
                            }
                        }
                        //else match for movie regexp
                        else
                        {
                            if (!string.IsNullOrEmpty(movieTitle))
                            {
                                logger.TraceMessage("SearchFileNameAsync - Found movietitle, and year in file name");
                                return new MatchedFile(fileName, movieTitle, year);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
                return null;
            }

            logger.TraceMessage("SearchFileNameAsync - End NULL");
            return null;
        }

        private string GetTrueShowName(string input)
        {
            logger.TraceMessage("GetTrueShowName - Start");
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

            logger.TraceMessage("GetTrueShowName - End");
            return output.Trim();
        }

        private bool IsJoiningWord(string input)
        {
            logger.TraceMessage("IsJoiningWord - Start");
            foreach (string word in JoiningWords)
            {
                if (input.Equals(word.ToLowerInvariant()))
                {
                    logger.TraceMessage("IsJoiningWord - True");
                    return true;
                }
            }
            logger.TraceMessage("IsJoiningWord - False");
            return false;
        }
        private string[] JoiningWords
        {
            get { return joiningWords.Split(','); }
        }

        private string joiningWords = "the,of,and";
    }
}
