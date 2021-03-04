using MetaBuddyLib.Config;
using UnityEngine;

namespace MetaBuddy.Settings
{
    public class MetaBuddySettings : ScriptableObject
    {        
#pragma warning disable 0414
        [SerializeField]
        public bool suppressBanner;

        [SerializeField]
        public bool ignoreFilesInDotGitIgnore;

        [SerializeField]
        public bool verboseLogging;

        [SerializeField]
        public bool analyseOnEditorStartup;

        [SerializeField]
        public bool logIgnoredFiles;

#pragma warning restore 0414

        public const string SuppressBannerPropertyName = nameof(suppressBanner);
        public const string IgnoreFilesInDotGitIgnorePropertyName = nameof(ignoreFilesInDotGitIgnore);
        public const string VerboseLoggingPropertyName = nameof(verboseLogging);
        public const string RunOnEditorStartPropertyName = nameof(analyseOnEditorStartup);
        public const string LogIgnoredFilesPropertyName = nameof(logIgnoredFiles);

        internal static MetaBuddySettings CreateFromConfig(ConfigModel config)
        {
            var settings = CreateInstance<MetaBuddySettings>();

            settings.suppressBanner = config.NoBanner;
            settings.ignoreFilesInDotGitIgnore = !config.NoDotGitIgnore;
            settings.verboseLogging = config.Verbose;
            settings.analyseOnEditorStartup = config.AnalyseOnEditorStartup;
            settings.logIgnoredFiles = config.ListIgnoredFiles;

            return settings;
        }

        internal ConfigModel ToConfig()
        {
            var config = new ConfigModel()
            {
                CheckCommand = CheckCommand.Changes,
                NoBanner = suppressBanner,
                NoDotGitIgnore = !ignoreFilesInDotGitIgnore,
                Verbose = verboseLogging,
                AnalyseOnEditorStartup = analyseOnEditorStartup,
                ListIgnoredFiles = logIgnoredFiles
            };           

            return config;
        }
    }
}

