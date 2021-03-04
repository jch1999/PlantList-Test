using MetaBuddy.Analysis;
using MetaBuddy.Util;
using MetaBuddyLib.Log;

namespace MetaBuddy.App
{
    public class ServiceRegistry
    {
        public CheckerFactory CheckerFactory  { get; private set; }
        public AnalysisRunner AnalysisRunner { get; private set; }
        public ConfigFetcher ConfigFetcher { get; private set; }
        public ExceptionLogger ExceptionLogger { get; private set; }

        private readonly UnityLogger _logger;
        public IMetaBuddyLogger Logger => _logger;
        public ILogHistory LogHistory => _logger;

        public ServiceRegistry()
        {
            _logger = new UnityLogger();
            CheckerFactory = new CheckerFactory(Logger);
            ExceptionLogger = new ExceptionLogger(Logger);
            ConfigFetcher = new ConfigFetcher(Logger);
            AnalysisRunner = new AnalysisRunner
            (
                Logger,
                CheckerFactory,
                ExceptionLogger
            );
        }
    }
}
