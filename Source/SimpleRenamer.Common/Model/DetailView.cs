using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// DetailView
    /// </summary>
    public class DetailView : IEquatable<DetailView>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the show.
        /// </summary>
        /// <value>
        /// The name of the show.
        /// </value>
        public string ShowName { get; set; }
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public string Year { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailView"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showName">The showname.</param>
        /// <param name="year">The year.</param>
        /// <param name="description">The description.</param>
        public DetailView(string id, string showName, string year, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (string.IsNullOrWhiteSpace(showName))
            {
                throw new ArgumentNullException(nameof(showName));
            }
            if (string.IsNullOrWhiteSpace(year))
            {
                throw new ArgumentNullException(nameof(year));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
            Id = id;
            ShowName = showName;
            Year = year;
            Description = description;
        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="DetailView"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DetailView other)
        {
            if (other == null)
            {
                return false;
            }

            return
                string.Equals(Id, other.Id) &&
                string.Equals(ShowName, other.ShowName) &&
                string.Equals(Year, other.Year) &&
                string.Equals(Description, other.Description);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as DetailView);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    hashCode = (hashCode * 16777619) + Id.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(ShowName))
                {
                    hashCode = (hashCode * 16777619) + ShowName.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(Year))
                {
                    hashCode = (hashCode * 16777619) + Year.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    hashCode = (hashCode * 16777619) + Description.GetHashCode();
                }

                return hashCode;
            }
        }
        #endregion Equality
    }
}
