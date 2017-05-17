using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// RegexFile
    /// </summary>
    public class RegexFile
    {
        /// <summary>
        /// Gets or sets the regex expressions.
        /// </summary>
        /// <value>
        /// The regex expressions.
        /// </value>
        public List<RegexExpression> RegexExpressions { get; set; }
    }
}
