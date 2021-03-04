using System.IO;
using MetaBuddyLib.Config;
using MetaBuddy.Settings;
using UnityEditor;
using UnityEngine;
using MetaBuddy.App;
using MetaBuddy.UI.Styles;
using MetaBuddy.UI.Settings.Styles;
using MetaBuddy.UI.Footer.Styles;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace MetaBuddy.UI.Settings
{
    public class MetaBuddySettingsProvider : SettingsProvider
    {
        public static readonly string SettingsUIPath = $"Project/{Product.Name}";

        private readonly StyleCache _settingsContainerStyle = new StyleCache(new SettingsContainer());
        private readonly StyleCache _footerContainerStyle = new StyleCache(new FooterContainer());

        private SerializedObject _serializedSettings;

        private SerializedProperty _suppressBannerProperty;
        private SerializedProperty _ignoreFilesInDotGitIgnoreProperty;
        private SerializedProperty _verboseLoggingProperty;
        private SerializedProperty _runOnEditorStartProperty;
        private SerializedProperty _logIgnoredFilesProperty;

        private class Labels
        {
            public static GUIContent SuppressBanner = new GUIContent("Hide banner in Console and log files");
            public static GUIContent IgnoreFilesFromDotGitIgnore = new GUIContent("Ignore files from .gitignore");
            public static GUIContent RunOnEditorStart = new GUIContent("Run analysis when Unity Editor starts");
            public static GUIContent VerboseLogging = new GUIContent("Verbose logging");
            public static GUIContent LogIgnoredFiles = new GUIContent("Log ignored files to the console");
            public static GUIContent ProductTitle = new GUIContent(Product.Title, "Visit the MetaBuddy home page.");
        }

        private MetaBuddySettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _serializedSettings = new SerializedObject(LoadOrCreate());

            _suppressBannerProperty = _serializedSettings.FindProperty(MetaBuddySettings.SuppressBannerPropertyName);
            _ignoreFilesInDotGitIgnoreProperty = _serializedSettings.FindProperty(MetaBuddySettings.IgnoreFilesInDotGitIgnorePropertyName);
            _verboseLoggingProperty = _serializedSettings.FindProperty(MetaBuddySettings.VerboseLoggingPropertyName);
            _runOnEditorStartProperty = _serializedSettings.FindProperty(MetaBuddySettings.RunOnEditorStartPropertyName);
            _logIgnoredFilesProperty = _serializedSettings.FindProperty(MetaBuddySettings.LogIgnoredFilesPropertyName);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical(_settingsContainerStyle.Get);

            var saveLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            EditorGUILayout.PropertyField
            (
                _suppressBannerProperty,
                Labels.SuppressBanner
            );

            EditorGUILayout.PropertyField
            (
                _ignoreFilesInDotGitIgnoreProperty,
                Labels.IgnoreFilesFromDotGitIgnore
            );

            EditorGUILayout.PropertyField
            (
                _verboseLoggingProperty,
                Labels.VerboseLogging
            );

            EditorGUILayout.PropertyField
            (
                _runOnEditorStartProperty,
                Labels.RunOnEditorStart
            );

            EditorGUILayout.PropertyField
            (
                _logIgnoredFilesProperty,
                Labels.LogIgnoredFiles
            );

            EditorGUIUtility.labelWidth = saveLabelWidth;

            EditorGUILayout.EndVertical();

            if(_serializedSettings.hasModifiedProperties)
            {
                _serializedSettings.ApplyModifiedPropertiesWithoutUndo();
                Save();                
            }       
        }

        public override void OnTitleBarGUI()
        {
            if (GUIUtil.HelpIcon())
            {
                Product.OpenDocumentationPage();
            }
        }

        public override void OnFooterBarGUI()
        {
            EditorGUILayout.BeginVertical(_footerContainerStyle.Get);
                Footer.FooterGUI.Generate(false);
            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            var settings = _serializedSettings.targetObject as MetaBuddySettings;
            var config = settings.ToConfig();
            ConfigSerialization.Save(config, ConfigModel.DefaultConfigFilename);
        }

        private static MetaBuddySettings LoadOrCreate()
        {
            if (!TryLoadConfig(out ConfigModel config))
            {
                config = DefaultConfigFactory.Create();
            }

            return MetaBuddySettings.CreateFromConfig(config);
        }

        private static bool TryLoadConfig(out ConfigModel config)
        {
            try
            {
                config = ConfigSerialization.Load(ConfigModel.DefaultConfigFilename);
                return config != null;
            }
            catch (FileNotFoundException)
            {
            }

            config = null;
            return false;
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new MetaBuddySettingsProvider
            (
                SettingsUIPath,
                SettingsScope.Project
            )
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Labels>()
            };

            return provider;
        }
    }
}