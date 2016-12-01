using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// Auth
    /// </summary>
    [DataContract]
    public partial class Auth : IEquatable<Auth>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Auth" /> class.
        /// </summary>
        /// <param name="Apikey">Apikey.</param>
        /// <param name="Username">Username.</param>
        /// <param name="Userkey">Userkey.</param>
        public Auth(string Apikey = null, string Username = null, string Userkey = null)
        {
            this.Apikey = Apikey;
            this.Username = Username;
            this.Userkey = Userkey;
        }

        /// <summary>
        /// Gets or Sets Apikey
        /// </summary>
        [DataMember(Name = "apikey", EmitDefaultValue = false)]
        public string Apikey { get; set; }
        /// <summary>
        /// Gets or Sets Username
        /// </summary>
        [DataMember(Name = "username", EmitDefaultValue = false)]
        public string Username { get; set; }
        /// <summary>
        /// Gets or Sets Userkey
        /// </summary>
        [DataMember(Name = "userkey", EmitDefaultValue = false)]
        public string Userkey { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Auth {\n");
            sb.Append("  Apikey: ").Append(Apikey).Append("\n");
            sb.Append("  Username: ").Append(Username).Append("\n");
            sb.Append("  Userkey: ").Append(Userkey).Append("\n");
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
            return this.Equals(obj as Auth);
        }

        /// <summary>
        /// Returns true if Auth instances are equal
        /// </summary>
        /// <param name="other">Instance of Auth to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Auth other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Apikey == other.Apikey ||
                    this.Apikey != null &&
                    this.Apikey.Equals(other.Apikey)
                ) &&
                (
                    this.Username == other.Username ||
                    this.Username != null &&
                    this.Username.Equals(other.Username)
                ) &&
                (
                    this.Userkey == other.Userkey ||
                    this.Userkey != null &&
                    this.Userkey.Equals(other.Userkey)
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
                if (this.Apikey != null)
                    hash = hash * 59 + this.Apikey.GetHashCode();
                if (this.Username != null)
                    hash = hash * 59 + this.Username.GetHashCode();
                if (this.Userkey != null)
                    hash = hash * 59 + this.Userkey.GetHashCode();
                return hash;
            }
        }
    }
}
