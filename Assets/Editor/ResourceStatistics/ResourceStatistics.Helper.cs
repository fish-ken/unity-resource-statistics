using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// Asset type to be analysed
    /// /// </summary>
    public enum AssetType
    {
        Texture,
        SpriteAtlas,
        Animation,
        Model,
        AudioClip,
    }

    /// <summary>
    /// Utility for ResourceStatistics
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Export path cache
        /// </summary>
        private static string baseExportPath = null;

        /// <summary>
        /// Export path property
        /// </summary>
        private static string BaseExportPath
        {
            get
            {
                if (baseExportPath.IsNullOrEmpty())
                    baseExportPath = $"{PathExtension.ProjectRootPath}/ResourceStatistics/";

                return baseExportPath;
            }
        }

        /// <summary>
        /// Cache for ToFilterKeyword
        /// </summary>
        private const string FilterFormat = "t:{0}";

        /// <summary>
        /// Cache for ToFilterKeyword
        /// </summary>
        private const string FilterNameFormat = "t:{0} {1}";

        /// <summary>
        /// Current platform
        /// </summary>
        public static RuntimePlatform Platform => UnityEngine.Application.platform;

        /// <summary>
        /// Return filter parameter for AssetDataBase.FindAssets(...)
        /// </summary>
        private static string ToFilterKeyword(this AssetType assetType, string name = null)
        {
            if (name == null)
                return string.Format(FilterFormat, assetType);
            else
                return string.Format(FilterNameFormat, assetType, name);
        }

        /// <summary>
        /// Check if the path is ignored
        /// </summary>
        private static bool IsIgnorePath(string path)
        {
            if (path.StartsWith("Packages/"))
                return true;
            
            return false;
        }

        /// <summary>
        /// Return the path appropriate for the assettype
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToAssetPaths(this AssetType assetType, string name = null)
        {
            var guids = AssetDatabase.FindAssets(assetType.ToFilterKeyword(name));
            return guids.Select(AssetDatabase.GUIDToAssetPath).Where(path => IsIgnorePath(path) == false);
        }

        /// <summary>
        /// Return export path for asset type
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static string GetExportPath(this AssetType assetType)
        {
            return $"{BaseExportPath}/{assetType}";
        }

        /// <summary>
        /// Create directory, if directory is not exist
        /// </summary>
        /// <param name="assetType"></param>
        public static void ValidateExportPath(this AssetType assetType)
        {
            var path = assetType.GetExportPath();

            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Export statistic result
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="???"></param>
        public static void ExportResult(AssetType assetType, string name, StringBuilder stringBuilder)
        {
            var fileName = $"{assetType.GetExportPath()}/{name}.csv";
            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        /// <summary>
        /// Return file size (by KB)
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static float GetFileSize(string fullPath)
        {
            return (new FileInfo(fullPath).Length / 1024f);
        }

        /// <summary>
        /// Return asset`s on memory size (by KB)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetMemorySize(Object obj)
        {
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(obj);
        }
    }
}