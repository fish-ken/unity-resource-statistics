using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ResourceStatistics
{
    public static class Util
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        
        /// <summary>
        /// Root path of project
        /// </summary>
        public static string ProjectRootPath => Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        /// <summary>
        /// Library path
        /// </summary>
        public static string LibraryPath => $"{ProjectRootPath}/Library";

        /// <summary>
        /// Convert collection string to csv line
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string ToCsvLine(this IEnumerable<string> fields)
        {
            if (fields == null || fields.Any() == false)
                return string.Empty;

            var builder = new StringBuilder();

            foreach (var field in fields)
            {
                var convertedField = field;

                if (convertedField.Contains("\""))
                    convertedField = convertedField.Replace("\"", "\"\"");

                if (convertedField.Contains(",") || convertedField.Contains("\\n"))
                    convertedField = string.Format("\"{0}\"", convertedField);

                builder.Append($"{convertedField},");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert object to json
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            if (value == null)
                return string.Empty;

            return JsonConvert.SerializeObject(value);
        }
    }

    public static class EnumExtension<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Return true when T is enum type
        /// </summary>
        public static bool IsEnum
        {
            get => typeof(T).IsEnum;
        }

        /// <summary>
        /// Get enumeration of T
        /// </summary>
        public static IEnumerable<T> Enumerable
        {
            get
            {
                if (IsEnum == false)
                    return null;

                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }
    }

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
                    baseExportPath = $"{Util.ProjectRootPath}/ResourceStatistics/";

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

            if (path.Contains("Editor/"))
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
        /// Open a StreamWriter for incremental csv writing
        /// </summary>
        public static StreamWriter OpenCsvWriter(AssetType assetType, string name)
        {
            var fileName = $"{assetType.GetExportPath()}/{name}.csv";
            return new StreamWriter(fileName, false, Encoding.UTF8);
        }

        /// <summary>
        /// Export critical log by markdown
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="logMap"></param>
        public static void ExportCriticalLog(AssetType assetType, Dictionary<string, StringBuilder> logMap)
        {
            var fileName = $"{assetType.GetExportPath()}/{assetType}-CriticalLog.md";
            var builder = new StringBuilder();

            foreach (var category in logMap.Keys)
            {
                builder.AppendLine($"## {assetType} - {category}");
                builder.AppendLine(logMap[category].ToString());
                builder.AppendLine("\n");
            }

            File.WriteAllText(fileName, builder.ToString());
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
        public static float GetMemorySize_kB(UnityEngine.Object obj)
        {
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(obj) / 1024f;
        }

        /// <summary>
        /// Return asset`s on memory size (by mb)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetMemorySize_MB(UnityEngine.Object obj)
        {
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(obj) / (1024f * 1024f);
        }

    }
}