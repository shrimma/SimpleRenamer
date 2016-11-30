using System.Collections.Generic;

namespace SimpleRenamer.Common.Model
{
    public class ShowNameMapping
    {
        private List<Mapping> mappings = new List<Mapping>();
        public List<Mapping> Mappings { get { return mappings; } }
    }
}
