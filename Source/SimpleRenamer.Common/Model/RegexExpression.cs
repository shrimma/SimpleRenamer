using System.Xml.Serialization;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// RegexExpression
    /// </summary>
    public class RegexExpression
    {
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        [XmlText]
        public string Expression { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is for tv show.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is for tv show; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsForTvShow { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexExpression" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="isTvShow">if set to <c>true</c> [is tv show].</param>
        public RegexExpression(string expression, bool enabled, bool isTvShow)
        {
            Expression = expression;
            IsEnabled = enabled;
            IsForTvShow = isTvShow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexExpression"/> class.
        /// </summary>
        public RegexExpression()
        {
        }
    }
}
