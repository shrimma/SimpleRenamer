using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleRenamer.Framework.TmdbModel
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
