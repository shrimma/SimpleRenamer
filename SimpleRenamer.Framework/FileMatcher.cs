using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleRenamer.Framework
{
    public static class FileMatcher
    {
        public static TVEpisode SearchMe(string fileName)
        {
            string showname = null;
            string season = null;
            string episode = null;

            string standard = @"^((?<series_name>.+?)[. _-]+)?s(?<season_num>\d+)[. _-]*e(?<ep_num>\d+)(([. _-]*e|-)(?<extra_ep_num>(?!(1080|720)[pi])\d+))*[. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$";
            string Fov = @"^((?<series_name>.+?)[\[. _-]+)?(?<season_num>\d+)x(?<ep_num>\d+)(([. _-]*x|-)(?<extra_ep_num>(?!(1080|720)[pi])(?!(?<=x)264)\d+))*[\]. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$";
            string wtf = @"^((?<series_name>.*[^ (_.])[ (_.]+((?<ShowYearA>\d{4})([ (_.]+S(?<season_num>\d{1,2})E(?<ep_num>\d{1,2}))?|(?<!\d{4}[ (_.])S(?<SeasonB>\d{1,2})E(?<EpisodeB>\d{1,2})|(?<EpisodeC>\d{3}))|(?<ShowNameB>.+))";
            List<string> regexExp = new List<string>();
            regexExp.Add(standard);
            regexExp.Add(Fov);
            regexExp.Add(wtf);
            try
            {
                foreach (string exp in regexExp)
                {
                    //process the file name                
                    Regex regexStandard = new Regex(exp, RegexOptions.IgnoreCase);
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
            catch
            {
                return null;
            }
            return null;
        }

        public static string GetTrueShowName(string input)
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

        public static bool IsJoiningWord(string input)
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
        public static string[] JoiningWords
        {
            get { return joiningWords.Split(','); }
        }

        private static string joiningWords = "the,of,and";
    }
}
