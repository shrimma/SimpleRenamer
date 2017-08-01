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
        public List<RegexExpression> RegexExpressions { get; set; } = new List<RegexExpression>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexFile"/> class.
        /// </summary>
        public RegexFile()
        {

        }
    }
}
