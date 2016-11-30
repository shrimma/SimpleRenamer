using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleRenamer.Common.Movie.Model
{
    public class ChangeItemAdded : ChangeItemBase
    {
        public ChangeItemAdded()
        {
            Action = ChangeAction.Added;
        }

        [JsonProperty("value")]
        public JToken Value { get; set; }
    }
}
