using UnityEditor;
using UnityEngine;
using MetaBuddyLib.Analysis;
using MetaBuddy.App;
using MetaBuddyLib.Log;
using MetaBuddy.UI.Styles;
using MetaBuddy.UI.ErrorList;
using MetaBuddy.UI.Window.Styles;
using MetaBuddy.UI.Settings;
using MetaBuddy.UI.AnalysisSummary;
using MetaBuddy.Util;
using System.IO;
using MetaBuddy.UI.HelpBox;
using System.Threading.Tasks;
using System;
using MetaBuddy.Analysis;
using MetaBuddyLib.Config;

namespace MetaBuddy.UI.Window
{
    public class MetaBuddyWindow : EditorWindow
    {
        private const float MaxCompletionTime = 30.0f;
        private Task<AnalysisResults> _analysisTask = null;
        private double _analysisStartTime = 0.0;
        private float _progress = 0.0f;

        private ConfigModel _config;
        private AnalysisResults _results;
        private ErrorListGUI _errorsView;

        private readonly StyleCache _analyseButtonStyle = new StyleCache(new AnalyseButton());

        private readonly AnalysisSummaryGUI _summaryView = new AnalysisSummaryGUI();
        private readonly HelpBoxGUI _helpBox = new HelpBoxGUI();

        private IMetaBuddyLogger _logger;
        private PathResolver _imagePathResolver;

        private CachedTexture _analyseIcon;
        private CachedTexture _buddyIcon;

        private PathResolver CreateImagePathResolver()
        {
            var skinPath = (EditorGUIUtility.isProSkin)
                 ? "pro"
                 : "personal";

            var imagePath = Path.Combine("images", skinPath);

            return PathResolver.Create(Product.PackageName, this, imagePath, 1);
        }

        private PathResolver ImagePathResolver
        {
            get
            {
                _imagePathResolver = _imagePathResolver ?? CreateImagePathResolver();
                return _imagePathResolver;
            }
        }

        private IMetaBuddyLogger Logger
        {
            get
            {
                _logger = _logger ?? ServiceLocator.Registry.Logger;
                return _logger;
            }            
        }
     
        public void OnEnable()            
        {
            Logger.LogDebug($"Loading UI images from {ImagePathResolver.ProjectRelativeBasePath}");

            _analyseIcon = new CachedTexture
             (
                 ImagePathResolver.ResolvePath("noun_play_5206-32x32.png")
             );

            _buddyIcon = new CachedTexture
             (
                 ImagePathResolver.ResolvePath("noun_Buddy_3361021-32x32.png")
             );

            _summaryView.OnEnable(ImagePathResolver);

            titleContent = new GUIContent(Product.Name, _buddyIcon.Get);

            _errorsView = new ErrorListGUI();
        }

        private void RunAnalysis() 
        {
            _results = null;

            if(_analysisTask == null)
            {
                var runner = ServiceLocator.Registry.AnalysisRunner;
                var configFetcher = ServiceLocator.Registry.ConfigFetcher;

                _config = configFetcher.FetchConfig(AnalysisRunner.HasRunOnce);
                _analysisStartTime = EditorApplication.timeSinceStartup;
                _analysisTask = runner.AnalyseAsync(AnalysisContext.GUI, _config);
            }
        }

        private void PresentResults()
        {
            EditorGUILayout.Space();

            if (_results.Assets.AnalysedFileCount > 0)
            {
                _summaryView.Generate(_results);

                EditorGUILayout.Space();

                _errorsView.Generate
                (
                    _results.Errors,
                    _results.Assets.BasePath,
                    _config.ProjectPath
                );
            }
            else
            {
                _helpBox.Generate
                (
                    "\n" +
                    $"No staged changes for {Product.Name} to check.\n\n" +
                    "Try staging some files for commit with 'git add' before checking again.\n",
                    MessageType.Info
                );
            }
        }

        private static void IconBar()
        {
            EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUIUtil.HelpIcon())
                {
                    Product.OpenDocumentationPage();
                }

                if (GUIUtil.GearIcon())
                {
                    SettingsService.OpenProjectSettings(MetaBuddySettingsProvider.SettingsUIPath);
                }
              
            EditorGUILayout.EndHorizontal();
        }

        private bool LastCriticalErrorGUI()
        {
            var logHistory = ServiceLocator.Registry.LogHistory;

            var lastLog = logHistory.LastLogEntry;

            if ((lastLog != null) && ((lastLog.Flags & LogLevelFlags.Critical) != LogLevelFlags.None))
            {
                _helpBox.Generate(lastLog.Message, MessageType.Error);
                return true;
            }

            return false;
        }

        private void BodyGUI()
        {
            if (_analysisTask != null)
            {
                var checkMsg = "Checking...";
                EditorUtility.DisplayProgressBar(Product.Name, checkMsg, _progress);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(checkMsg);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                if (!LastCriticalErrorGUI())
                {
                    if (_results != null)
                    {
                        PresentResults();
                    }
                }
            }           
        }

        public void Update()
        {
            if (_analysisTask != null)
            {
                var elapsed = (float)(EditorApplication.timeSinceStartup - _analysisStartTime);
                _progress = Math.Min(elapsed / MaxCompletionTime, 1.0f);

                if(_analysisTask.IsCompleted)
                {
                    var task = _analysisTask;
                    _analysisTask = null;
                    _results = ServiceLocator.Registry.AnalysisRunner.WaitForResults(task);
                }

                Repaint();
            }
        }

        public bool HasErrorsToDisplay
        {
            get
            {
                return _analysisTask == null
                    && _results != null
                    && _results.Errors != null
                    && _results.Errors.ErrorCount > 0;
            }
        }

        public void OnGUI()
        {
            wantsMouseMove = HasErrorsToDisplay;

            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
                IconBar();

                EditorGUILayout.Space();
            
                var content = new GUIContent("Check Staged Changes", _analyseIcon.Get);

                if (GUILayout.Button(content, _analyseButtonStyle.Get))
                {
                    RunAnalysis();
                }

                BodyGUI();
          
                GUILayout.FlexibleSpace();

                Footer.FooterGUI.Generate(true);
            
            EditorGUILayout.EndVertical();
        }

        [MenuItem("Window/MetaBuddy")]
        public static void ShowWindow()
        { 
            GetWindow(typeof(MetaBuddyWindow));
        }
    }
}

