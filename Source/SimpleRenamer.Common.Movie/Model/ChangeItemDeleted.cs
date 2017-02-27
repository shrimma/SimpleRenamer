using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class ChangeItemDeleted : ChangeItemBase
    {
        public ChangeItemDeleted()
        {
            Action = ChangeAction.Deleted;
        }

        [JsonProperty("original_value")]
        public JToken OriginalValue { get; set; }
    }
}
