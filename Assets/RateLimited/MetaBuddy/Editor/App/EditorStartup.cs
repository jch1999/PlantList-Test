using MetaBuddy.Analysis;
using UnityEditor;

namespace MetaBuddy.App
{

    public static class EditorStartup
    {
        [InitializeOnLoadMethod]
        public static void OnEditorStartup()
        {
            var runner = ServiceLocator.Registry.AnalysisRunner;
            var configFetcher = ServiceLocator.Registry.ConfigFetcher;
            
            var config = configFetcher.FetchConfig(AnalysisRunner.HasRunOnce);
            runner.AnalyseSync(AnalysisContext.EditorStart, config);
        }
    }
}

