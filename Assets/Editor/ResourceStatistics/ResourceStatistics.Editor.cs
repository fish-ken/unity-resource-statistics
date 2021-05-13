using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// ResourceStatistics editor window class
    /// </summary>
    public class Editor : EditorWindow
    {
        #region static

        public static bool EnableLog { get; private set; }

        public static Application Application { get; private set; } = new Application();

        [MenuItem("Tools/Statistics/Resources Statistics/Open", priority = 55)]
        private static void ShowWindow()
        {
            var window = GetWindow<Editor>();
            window.Show();
        }

        [MenuItem("Tools/Statistics/Resources Statistics/Scan all", priority = 55)]
        private static void ScanAll()
        {
            foreach (var assetType in EnumExtension<AssetType>.Enumerable)
                Application.Scan(assetType);
        }

        #endregion

        private Dictionary<AssetType, bool> toggleType = new Dictionary<AssetType, bool>();
        
        private void OnGUI()
        {
            var assetTypes = EnumExtension<AssetType>.Enumerable;

            EnableLog = EditorGUILayout.Toggle("Enable Log", EnableLog);

            // TODO : Filter 
            
            foreach (var type in assetTypes)
            {
                if (toggleType.ContainsKey(type) == false)
                    toggleType[type] = false;

                toggleType[type] = EditorGUILayout.Toggle(type.ToString(), toggleType[type]);
            }

            if (GUILayout.Button("Scan"))
            {
                foreach (var type in assetTypes)
                {
                    if (toggleType.ContainsKey(type) == false || toggleType[type] == false)
                        continue;

                    Application.Scan(type);
                }
            }
        }
    }
}