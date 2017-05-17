using System;
using System.ComponentModel;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MatchedFile : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged implementation

        /// <summary>
        /// The file path
        /// </summary>
        private string filePath;
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (value != filePath)
                {
                    filePath = value;
                    Notify("FilePath");
                }
            }
        }

        /// <summary>
        /// The TVDB show identifier
        /// </summary>
        private string tvdbShowId;
        /// <summary>
        /// Gets or sets the TVDB show identifier.
        /// </summary>
        /// <value>
        /// The TVDB show identifier.
        /// </value>
        public string TVDBShowId
        {
            get { return tvdbShowId; }
            set
            {
                if (value != tvdbShowId)
                {
                    tvdbShowId = value;
                    Notify("TVDBShowId");
                }
            }
        }

        /// <summary>
        /// The TMDB show identifier
        /// </summary>
        private int tmdbShowId;
        /// <summary>
        /// Gets or sets the TMDB show identifier.
        /// </summary>
        /// <value>
        /// The TMDB show identifier.
        /// </value>
        public int TMDBShowId
        {
            get { return tmdbShowId; }
            set
            {
                if (value != tmdbShowId)
                {
                    tmdbShowId = value;
                    Notify("TMDBShowId");
                }
            }
        }

        /// <summary>
        /// The show name
        /// </summary>
        private string showName;
        /// <summary>
        /// Gets or sets the name of the show.
        /// </summary>
        /// <value>
        /// The name of the show.
        /// </value>
        public string ShowName
        {
            get { return showName; }
            set
            {
                if (value != showName)
                {
                    showName = value;
                    Notify("ShowName");
                }
            }
        }

        /// <summary>
        /// The season
        /// </summary>
        private string season;
        /// <summary>
        /// Gets or sets the season.
        /// </summary>
        /// <value>
        /// The season.
        /// </value>
        public string Season
        {
            get { return season; }
            set
            {
                if (value != season)
                {
                    season = value;
                    Notify("Season");
                }
            }
        }

        /// <summary>
        /// The episode
        /// </summary>
        private string episode;
        /// <summary>
        /// Gets or sets the episode.
        /// </summary>
        /// <value>
        /// The episode.
        /// </value>
        public string Episode
        {
            get { return episode; }
            set
            {
                if (value != episode)
                {
                    episode = value;
                    Notify("Episode");
                }
            }
        }

        /// <summary>
        /// The episode name
        /// </summary>
        private string episodeName;
        /// <summary>
        /// Gets or sets the name of the episode.
        /// </summary>
        /// <value>
        /// The name of the episode.
        /// </value>
        public string EpisodeName
        {
            get { return episodeName; }
            set
            {
                if (value != episodeName)
                {
                    episodeName = value;
                    Notify("EpisodeName");
                }
            }
        }

        /// <summary>
        /// The new file name
        /// </summary>
        private string newFileName;
        /// <summary>
        /// Gets or sets the new name of the file.
        /// </summary>
        /// <value>
        /// The new name of the file.
        /// </value>
        public string NewFileName
        {
            get { return newFileName; }
            set
            {
                if (value != newFileName)
                {
                    newFileName = value;
                    Notify("NewFileName");
                }
            }
        }

        /// <summary>
        /// The skipped exact selection
        /// </summary>
        private bool skippedExactSelection;
        /// <summary>
        /// Gets or sets a value indicating whether [skipped exact selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skipped exact selection]; otherwise, <c>false</c>.
        /// </value>
        public bool SkippedExactSelection
        {
            get { return skippedExactSelection; }
            set
            {
                if (value != skippedExactSelection)
                {
                    skippedExactSelection = value;
                    Notify("SkippedExactSelection");
                }
            }
        }

        /// <summary>
        /// The season image
        /// </summary>
        private string seasonImage;
        /// <summary>
        /// Gets or sets the season image.
        /// </summary>
        /// <value>
        /// The season image.
        /// </value>
        public string SeasonImage
        {
            get { return seasonImage; }
            set
            {
                if (value != seasonImage)
                {
                    seasonImage = value;
                    Notify("SeasonImage");
                }
            }
        }

        /// <summary>
        /// The show image
        /// </summary>
        private string showImage;
        /// <summary>
        /// Gets or sets the show image.
        /// </summary>
        /// <value>
        /// The show image.
        /// </value>
        public string ShowImage
        {
            get { return showImage; }
            set
            {
                if (value != showImage)
                {
                    showImage = value;
                    Notify("ShowImage");
                }
            }
        }

        /// <summary>
        /// The action this
        /// </summary>
        private bool actionThis;
        /// <summary>
        /// Gets or sets a value indicating whether [action this].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [action this]; otherwise, <c>false</c>.
        /// </value>
        public bool ActionThis
        {
            get { return actionThis; }
            set
            {
                if (value != actionThis)
                {
                    actionThis = value;
                    Notify("ActionThis");
                }
            }
        }

        /// <summary>
        /// The file type
        /// </summary>
        private FileType fileType;
        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>
        /// The type of the file.
        /// </value>
        public FileType FileType
        {
            get { return fileType; }
            set
            {
                if (value != fileType)
                {
                    fileType = value;
                    Notify("FileType");
                }
            }
        }

        /// <summary>
        /// The year
        /// </summary>
        private int year;
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year
        {
            get { return year; }
            set
            {
                if (value != year)
                {
                    year = value;
                    Notify("Year");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchedFile"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        public MatchedFile(string filePath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            FilePath = filePath;
            ShowName = fileName;
            FileType = FileType.Unknown;
            SkippedExactSelection = true;
            ActionThis = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchedFile"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="showName">Name of the show.</param>
        /// <param name="season">The season.</param>
        /// <param name="episode">The episode.</param>
        public MatchedFile(string filePath, string showName, string season, string episode)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(showName))
            {
                throw new ArgumentNullException(nameof(showName));
            }
            if (string.IsNullOrWhiteSpace(season))
            {
                throw new ArgumentNullException(nameof(season));
            }
            if (string.IsNullOrWhiteSpace(episode))
            {
                throw new ArgumentNullException(nameof(episode));
            }
            FilePath = filePath;
            ShowName = showName;
            Season = season;
            Episode = episode;
            FileType = FileType.TvShow;
            SkippedExactSelection = false;
            ActionThis = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchedFile"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="movieTitle">The movie title.</param>
        /// <param name="year">The year.</param>
        public MatchedFile(string filePath, string movieTitle, int year)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(movieTitle))
            {
                throw new ArgumentNullException(nameof(movieTitle));
            }
            FilePath = filePath;
            ShowName = movieTitle;
            Year = year;
            Season = year.ToString();
            FileType = FileType.Movie;
            SkippedExactSelection = false;
            ActionThis = true;
        }
    }
}
