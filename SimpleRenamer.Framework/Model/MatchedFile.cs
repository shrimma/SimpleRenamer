using System.ComponentModel;

namespace SimpleRenamer.Common.Model
{
    public class MatchedFile : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged implementation

        private string filePath;
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

        private string tvdbShowId;
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

        private int tmdbShowId;
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

        private string showName;
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

        private string season;
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

        private string episode;
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

        private string episodeName;
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

        private string newFileName;
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

        private bool skippedExactSelection;
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

        private string seasonImage;
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

        private string showImage;
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

        private bool actionThis;
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

        private FileType fileType;
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

        private int year;
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

        public MatchedFile(string filePath, string fileName)
        {
            FilePath = filePath;
            ShowName = fileName;
            FileType = FileType.Unknown;
            SkippedExactSelection = true;
            ActionThis = false;
        }

        public MatchedFile(string filePath, string showName, string season, string episode)
        {
            FilePath = filePath;
            ShowName = showName;
            Season = season;
            Episode = episode;
            FileType = FileType.TvShow;
            SkippedExactSelection = false;
            ActionThis = true;
        }

        public MatchedFile(string filePath, string movieTitle, int year)
        {
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
