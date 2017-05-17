using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// File Move Result
    /// </summary>
    public class FileMoveResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FileMoveResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets the episode.
        /// </summary>
        /// <value>
        /// The episode.
        /// </value>
        public MatchedFile Episode { get; set; }
        /// <summary>
        /// Gets or sets the destination file path.
        /// </summary>
        /// <value>
        /// The destination file path.
        /// </value>
        public string DestinationFilePath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMoveResult"/> class.
        /// </summary>
        /// <param name="success">if set to <c>true</c> [success].</param>
        /// <param name="episode">The episode.</param>
        public FileMoveResult(bool success, MatchedFile episode)
        {
            Success = success;
            Episode = episode ?? throw new ArgumentNullException(nameof(episode));
            DestinationFilePath = string.Empty;
        }
    }
}
