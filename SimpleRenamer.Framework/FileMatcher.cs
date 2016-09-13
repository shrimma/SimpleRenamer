using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class FileMatcher : IFileMatcher
    {
        private static RegexFile regexExpressions = null;
        private static string regexFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RegexExpressions.xml");
        private ILogger logger;

        public FileMatcher(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            logger = log;
        }

        public async Task<TVEpisode> SearchFileNameAsync(string fileName)
        {
            logger.TraceMessage("SearchFileNameAsync - Start");
            string showname = null;
            string season = null;
            string episode = null;

            try
            {
                regexExpressions = await ReadExpressionFileAsync();
                foreach (RegexExpression exp in regexExpressions.RegexExpressions)
                {
                    if (exp.IsEnabled)
                    {
                        //process the file name
                        Regex regexStandard = new Regex(exp.Expression, RegexOptions.IgnoreCase);
                        Match tvshow = regexStandard.Match(Path.GetFileNameWithoutExtension(fileName));
                        showname = GetTrueShowName(tvshow.Groups["series_name"].Value);
                        season = tvshow.Groups["season_num"].Value;
                        episode = tvshow.Groups["ep_num"].Value;

                        if (!string.IsNullOrEmpty(showname) && !string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(episode))
                        {
                            logger.TraceMessage("SearchFileNameAsync - Found showname, season, and episode in file name");
                            return new TVEpisode(fileName, showname, season, episode);
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

        public async Task<RegexFile> ReadExpressionFileAsync()
        {
            logger.TraceMessage("ReadExpressionFileAsync - Start");
            RegexFile snm = new RegexFile();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(regexFilePath))
            {
                logger.TraceMessage("ReadExpressionFileAsync - File doesn't exist so returning new object");
                return snm;
            }
            else
            {
                using (FileStream fs = new FileStream(regexFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    snm = (RegexFile)serializer.Deserialize(fs);
                }
                logger.TraceMessage("ReadExpressionFileAsync - File exists returning populated expressions");
                return snm;
            }
        }

        public async Task<bool> WriteExpressionFileAsync(RegexFile regexMatchers)
        {
            logger.TraceMessage("WriteExpressionFileAsync - Start");
            //only write the file if there is data
            if (regexMatchers != null && regexMatchers.RegexExpressions.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(regexFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    serializer.Serialize(writer, regexMatchers);
                }
            }

            logger.TraceMessage("WriteExpressionFileAsync - End");
            return true;
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
