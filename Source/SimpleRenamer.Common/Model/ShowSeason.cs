
using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// ShowSeason
    /// </summary>
    public class ShowSeason
    {
        /// <summary>
        /// Gets or sets the show.
        /// </summary>
        /// <value>
        /// The show.
        /// </value>
        public string Show { get; set; }
        /// <summary>
        /// Gets or sets the season.
        /// </summary>
        /// <value>
        /// The season.
        /// </value>
        public string Season { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSeason"/> class.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="season">The season.</param>
        public ShowSeason(string show, string season)
        {
            if (string.IsNullOrWhiteSpace(show))
            {
                throw new ArgumentNullException(nameof(show));
            }
            if (string.IsNullOrWhiteSpace(season))
            {
                throw new ArgumentNullException(nameof(season));
            }
            Show = show;
            Season = season;
        }
    }
}
