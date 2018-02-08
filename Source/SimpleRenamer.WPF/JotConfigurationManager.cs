using Jot.DefaultInitializer;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// App Configuration Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IConfigurationManager" />
    public class JotConfigurationManager : IConfigurationManager
    {
        private List<string> ignoredFiles;
        /// <summary>
        /// Gets or sets the ignored files.
        /// </summary>
        /// <value>
        /// The ignored files.
        /// </value>
        [Trackable]
        public List<string> IgnoredFiles
        {
            get
            {
                if (ignoredFiles == null)
                {
                    ignoredFiles = new List<string>();
                }
                return ignoredFiles;
            }
            set
            {
                ignoredFiles = value;
            }
        }

        private List<RegexExpression> regexExpressions;
        /// <summary>
        /// Gets or sets the regex expressions.
        /// </summary>
        /// <value>
        /// The regex expressions.
        /// </value>
        [Trackable]
        public List<RegexExpression> RegexExpressions
        {
            get
            {
                if (regexExpressions == null)
                {
                    regexExpressions = new List<RegexExpression>
                    {
                        new RegexExpression("^((?<series_name>.+?)[. _-]+)?s(?<season_num>\\d+)[. _-]*e(?<ep_num>\\d+)(([. _-]*e|-)(?<extra_ep_num>(?!(1080|720)[pi])\\d+))*[. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", true, true),
                        new RegexExpression("^((?<series_name>.+?)[\\[. _-]+)?(?<season_num>\\d+)x(?<ep_num>\\d+)(([. _-]*x|-)(?<extra_ep_num>(?!(1080|720)[pi])(?!(?<=x)264)\\d+))*[\\]. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", true, true),
                        new RegexExpression("^((?<series_name>.*[^ (_.])[ (_.]+((?<ShowYearA>\\d{4})([ (_.]+S(?<season_num>\\d{1,2})E(?<ep_num>\\d{1,2}))?|(?<!\\d{4}[ (_.])S(?<SeasonB>\\d{1,2})E(?<EpisodeB>\\d{1,2})|(?<EpisodeC>\\d{3}))|(?<ShowNameB>.+))", true, true),
                        new RegexExpression("^((?<movie_title>.*[^ (_.])[ (_.]+(?!(1080|720)[pi])(?<movie_year>\\d{4})(.*))", true, false)
                    };
                }
                return regexExpressions;
            }
            set
            {
                regexExpressions = value;
            }
        }

        private ISettings settings;
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        [Trackable]
        public ISettings Settings
        {
            get
            {
                if (settings == null)
                {
                    //defaults
                    settings = new Settings
                    {
                        SubDirectories = true,
                        RenameFiles = true,
                        CopyFiles = false,
                        DestinationFolderMovie = string.Empty,
                        DestinationFolderTV = string.Empty,
                        NewFileNameFormat = "{ShowName} - S{Season}E{Episode} - {EpisodeName}",
                        ValidExtensions = new List<string>
                        {
                            ".264",
                            ".3g2",
                            ".3gp",
                            ".arf",
                            ".asf",
                            ".asx",
                            ".avi",
                            ".bik",
                            ".dash",
                            ".dvr",
                            ".flv",
                            ".h264",
                            ".m2t",
                            ".m2ts",
                            ".m4v",
                            ".mkv",
                            ".mod",
                            ".mov",
                            ".mp4",
                            ".mpeg",
                            ".mpg",
                            ".mts",
                            ".ogv",
                            ".rmvb",
                            ".swf",
                            ".tod",
                            ".tp",
                            ".ts",
                            ".vob",
                            ".webm",
                            ".wmv"
                        },
                        WatchFolders = new List<string>()
                    };
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        private List<Mapping> showNameMappings;
        /// <summary>
        /// Gets or sets the show name mappings.
        /// </summary>
        /// <value>
        /// The show name mappings.
        /// </value>
        [Trackable]
        public List<Mapping> ShowNameMappings
        {
            get
            {
                if (showNameMappings == null)
                {
                    showNameMappings = new List<Mapping>();
                }
                return showNameMappings;
            }
            set
            {
                showNameMappings = value;
            }
        }

        /// <summary>
        /// Gets the The TV Database API key.
        /// </summary>
        /// <value>
        /// The TV database API key.
        /// </value>
        public string TvDbApiKey
        {
            get
            {
                return "820147144A5BB54E";
            }
        }

        /// <summary>
        /// Gets the The Movie Database API key.
        /// </summary>
        /// <value>
        /// The Movie database API key.
        /// </value>
        public string TmDbApiKey
        {
            get
            {
                return "e9b955f1140da97e65df7e1bce1780bc";
            }
        }

        /// <summary>
        /// Gets the one true error URL.
        /// </summary>
        /// <value>
        /// The one true error URL.
        /// </value>
        public string OneTrueErrorUrl
        {
            get
            {
                return "https://jsote.uksouth.cloudapp.azure.com/OneTrueError/";
            }
        }

        /// <summary>
        /// Gets the one true error application key.
        /// </summary>
        /// <value>
        /// The one true error application key.
        /// </value>
        public string OneTrueErrorApplicationKey
        {
            get
            {
                return "d574b9a210704b1ba1d75bb70442e173";
            }
        }

        /// <summary>
        /// Gets the one true error shared secret.
        /// </summary>
        /// <value>
        /// The one true error shared secret.
        /// </value>
        public string OneTrueErrorSharedSecret
        {
            get
            {
                return "696c4b84e8d04a8c99cbc17570b1cc05";
            }
        }
    }
}
