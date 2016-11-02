using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Framework.TvdbModel
{
    /// <summary>
    /// Token
    /// </summary>
    [DataContract]
    public partial class Token : IEquatable<Token>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="_Token">_Token.</param>
        public Token(string _Token = null)
        {
            this._Token = _Token;
        }

        /// <summary>
        /// Gets or Sets _Token
        /// </summary>
        [DataMember(Name = "token", EmitDefaultValue = false)]
        public string _Token { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Token {\n");
            sb.Append("  _Token: ").Append(_Token).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as Token);
        }

        /// <summary>
        /// Returns true if Token instances are equal
        /// </summary>
        /// <param name="other">Instance of Token to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Token other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this._Token == other._Token ||
                    this._Token != null &&
                    this._Token.Equals(other._Token)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this._Token != null)
                    hash = hash * 59 + this._Token.GetHashCode();
                return hash;
            }
        }
    }
}
