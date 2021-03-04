using MetaBuddy.Settings;
using MetaBuddyLib.Config;
using MetaBuddyLib.Log;

namespace MetaBuddy.App
{
    public class ConfigFetcher
    {
        private readonly IMetaBuddyLogger _logger;

        public ConfigFetcher(IMetaBuddyLogger logger)
        {
            _logger = logger;
        }

        private static ConfigModel CreateConfig(bool hasRunOnce)
        {
            var defaultArgs = DefaultConfigFactory.Create();

            defaultArgs.NoBanner = hasRunOnce;

            return defaultArgs;
        }

        public ConfigModel FetchConfig(bool hasRunOnce)
        {
            var commandLineArgs = ArgumentParser.ParseCommandLine();
            _logger.SetDebug(commandLineArgs.Verbose);

            var defaultArgs = CreateConfig(hasRunOnce);

            var config = ConfigResolver.ResolveConfig
            (
                defaultArgs,
                commandLineArgs,
                _logger
            );
            _logger.SetDebug(config.Verbose);

            return config;
        }
    }
}
