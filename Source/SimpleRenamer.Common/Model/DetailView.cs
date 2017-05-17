using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// DetailView
    /// </summary>
    public class DetailView
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
        /// <param name="showname">The showname.</param>
        /// <param name="year">The year.</param>
        /// <param name="description">The description.</param>
        public DetailView(string id, string showname, string year, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (string.IsNullOrWhiteSpace(showname))
            {
                throw new ArgumentNullException(nameof(showname));
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
            ShowName = showname;
            Year = year;
            Description = description;
        }
    }
}
