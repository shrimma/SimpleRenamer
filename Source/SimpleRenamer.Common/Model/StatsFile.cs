using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Model
{
    public class StatsFile
    {
        public static StatsFile StatsFileFromMatchedFile(MatchedFile matchedFile)
        {
            return new StatsFile
            {
                ExistingFileName = Path.GetFileName(matchedFile.SourceFilePath),
                NewFileName = matchedFile.NewFileName,
                FileMatchIdentifier = matchedFile.FileType == FileType.TvShow ? matchedFile.TVDBShowId : matchedFile.FileType == FileType.Movie ? matchedFile.TMDBShowId.ToString() : string.Empty,
                MediaTitle = matchedFile.ShowName,
                MediaType = matchedFile.FileType
            };
        }

        [JsonProperty]
        public string ExistingFileName { get; set; }

        [JsonProperty]
        public string NewFileName { get; set; }

        [JsonProperty]
        public string FileMatchIdentifier { get; set; }

        [JsonProperty]
        public string MediaTitle { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public FileType MediaType { get; set; }
    }
}
