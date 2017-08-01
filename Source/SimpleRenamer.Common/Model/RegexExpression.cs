using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// RegexExpression
    /// </summary>
    [JsonObject("regexExpression")]
    public class RegexExpression : IEquatable<RegexExpression>
    {
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        [JsonProperty("expression")]
        public string Expression { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is for tv show.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is for tv show; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("isForTvShow")]
        public bool IsForTvShow { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexExpression" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="isTvShow">if set to <c>true</c> [is tv show].</param>
        public RegexExpression(string expression, bool enabled, bool isTvShow)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }
            Expression = expression;
            IsEnabled = enabled;
            IsForTvShow = isTvShow;
        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="RegexExpression"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RegexExpression other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    Expression == other.Expression ||
                    Expression != null &&
                    Expression.Equals(other.Expression)
                ) &&
                (
                    IsEnabled == other.IsEnabled
                ) &&
                (
                    IsForTvShow == other.IsForTvShow
                );
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as RegexExpression);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (Expression != null)
                {
                    hashCode = (hashCode * 16777619) + Expression.GetHashCode();
                }
                hashCode = (hashCode * 16777619) + IsEnabled.GetHashCode();
                hashCode = (hashCode * 16777619) + IsForTvShow.GetHashCode();

                return hashCode;
            }
        }
        #endregion Equality
    }
}
