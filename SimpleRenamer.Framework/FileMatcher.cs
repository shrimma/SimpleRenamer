﻿using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public static class FileMatcher
    {
        private static RegexFile regexExpressions = null;
        private static string regexFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RegexExpressions.xml");
        public static TVEpisode SearchMe(string fileName)
        {
            string showname = null;
            string season = null;
            string episode = null;

            try
            {
                regexExpressions = ReadExpressionFile();
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
                            return new TVEpisode(fileName, showname, season, episode);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static async Task<TVEpisode> SearchMeAsync(string fileName)
        {
            string showname = null;
            string season = null;
            string episode = null;

            try
            {
                regexExpressions = ReadExpressionFile();
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
                            return new TVEpisode(fileName, showname, season, episode);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static RegexFile ReadExpressionFile()
        {
            RegexFile snm = new RegexFile();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(regexFilePath))
            {
                return snm;
            }
            else
            {
                using (FileStream fs = new FileStream(regexFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    snm = (RegexFile)serializer.Deserialize(fs);
                }
                return snm;
            }
        }

        public static void WriteExpressionFile(RegexFile regexMatchers)
        {
            //only write the file if there is data
            if (regexMatchers != null && regexMatchers.RegexExpressions.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(regexFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    serializer.Serialize(writer, regexMatchers);
                }
            }
        }

        private static string GetTrueShowName(string input)
        {
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
            return output.Trim();
        }

        private static bool IsJoiningWord(string input)
        {
            foreach (string word in JoiningWords)
            {
                if (input.Equals(word.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }
        private static string[] JoiningWords
        {
            get { return joiningWords.Split(','); }
        }

        private static string joiningWords = "the,of,and";
    }
}
