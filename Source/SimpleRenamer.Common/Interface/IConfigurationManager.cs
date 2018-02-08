using Sarjee.SimpleRenamer.Common.Model;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// Configuration Manager Interface
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the The TV Database API key.
        /// </summary>
        /// <value>
        /// The TV database API key.
        /// </value>
        string TvDbApiKey { get; }

        /// <summary>
        /// Gets the The Movie Database API key.
        /// </summary>
        /// <value>
        /// The Movie database API key.
        /// </value>
        string TmDbApiKey { get; }

        /// <summary>
        /// Gets the one true error URL.
        /// </summary>
        /// <value>
        /// The one true error URL.
        /// </value>
        string OneTrueErrorUrl { get; }

        /// <summary>
        /// Gets the one true error application key.
        /// </summary>
        /// <value>
        /// The one true error application key.
        /// </value>
        string OneTrueErrorApplicationKey { get; }

        /// <summary>
        /// Gets the one true error shared secret.
        /// </summary>
        /// <value>
        /// The one true error shared secret.
        /// </value>
        string OneTrueErrorSharedSecret { get; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        ISettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the regex expressions.
        /// </summary>
        /// <value>
        /// The regex expressions.
        /// </value>
        List<RegexExpression> RegexExpressions { get; set; }

        /// <summary>
        /// Gets or sets the ignored files.
        /// </summary>
        /// <value>
        /// The ignored files.
        /// </value>
        List<string> IgnoredFiles { get; set; }

        /// <summary>
        /// Gets or sets the show name mappings.
        /// </summary>
        /// <value>
        /// The show name mappings.
        /// </value>
        List<Mapping> ShowNameMappings { get; set; }
    }
}
