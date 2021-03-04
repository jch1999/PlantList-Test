using MetaBuddy.Analysis;
using MetaBuddy.App;
using UnityEditor;

namespace MetaBuddy
{
    public static class CLI
    {
        public static void Run()
        {
            var runner = ServiceLocator.Registry.AnalysisRunner;
            var configFetcher = ServiceLocator.Registry.ConfigFetcher;

            var config = configFetcher.FetchConfig(AnalysisRunner.HasRunOnce);
            var analysis = runner.AnalyseSync(AnalysisContext.CLI, config);

            var exitCode = (analysis.Errors.ErrorCount > 0) ? 1 : 0;

            EditorApplication.Exit(exitCode);
        }
    }
}
