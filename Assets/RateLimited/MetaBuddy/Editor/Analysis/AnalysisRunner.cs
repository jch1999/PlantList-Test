using System;
using System.Threading.Tasks;
using MetaBuddy.App;
using MetaBuddyLib.Analysis;
using MetaBuddyLib.Checking;
using MetaBuddyLib.Config;
using MetaBuddyLib.Log;

namespace MetaBuddy.Analysis
{
   public class AnalysisRunner
   {
        private readonly IMetaBuddyLogger _logger;
        private readonly CheckerFactory _checkerFactory;
        private readonly ExceptionLogger _exceptionLogger;
            
        public static bool HasRunOnce { get; private set; } = false;

        public AnalysisRunner
        (
            IMetaBuddyLogger logger,
            CheckerFactory checkerFactory,
            ExceptionLogger exceptionLogger
        )
        {
            _logger = logger;
            _checkerFactory = checkerFactory;
            _exceptionLogger = exceptionLogger;
        }
      
        private Task<AnalysisResults> BeginAnalysis(AnalysisContext context, ConfigModel config)
        {
            if (config.NoBanner == false)
            {
                _logger.LogNone(Product.Banner);
            }

            var task = Task<AnalysisResults>.Factory.StartNew
            (
                () => {
                    var shouldAnalyse = context != AnalysisContext.EditorStart || config.AnalyseOnEditorStartup;

                    return shouldAnalyse
                        ? CheckExecutor.Execute(config, _checkerFactory, _logger)
                        : null;
                }
            );

            return task;
        }

        public Task<AnalysisResults> AnalyseAsync(AnalysisContext context, ConfigModel config)
        {
            try
            {
                return BeginAnalysis(context, config);
            }
            catch (Exception e)
            {
                _exceptionLogger.LogException(e);
            }

            return null;
        }

        public AnalysisResults WaitForResults(Task<AnalysisResults> task)
        {
            try
            {
                task.Wait();
                HasRunOnce = true;
                return task.Result;
            }
            catch (AggregateException ae)
            {
                var innerException = ae.InnerException;
                _exceptionLogger.LogException(innerException);
            }

            return null;
        }

        public AnalysisResults AnalyseSync(AnalysisContext context, ConfigModel config)
        {            
            var task = BeginAnalysis(context, config);

            AnalysisResults results = null;
            if(task != null)
            {
                results = WaitForResults(task);
                HasRunOnce = true;
            }

            return results;
        }
    }
}
