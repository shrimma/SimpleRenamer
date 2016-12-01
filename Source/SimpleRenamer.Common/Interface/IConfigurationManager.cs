using Sarjee.SimpleRenamer.Common.Model;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// The API key for TV Database
        /// </summary>
        string TvDbApiKey { get; }

        /// <summary>
        /// The API key for The Movie Database
        /// </summary>
        string TmDbApiKey { get; }

        string OneTrueErrorUrl { get; }

        string OneTrueErrorApplicationKey { get; }

        string OneTrueErrorSharedSecret { get; }

        string RegexFilePath { get; }

        string IgnoreListFilePath { get; }

        string ShowNameMappingFilePath { get; }

        Settings Settings { get; set; }

        RegexFile RegexExpressions { get; set; }

        IgnoreList IgnoredFiles { get; set; }

        ShowNameMapping ShowNameMappings { get; set; }

        void SaveConfiguration();
    }
}
