using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// ResourceStatistics`s application class
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Critical log container
        /// </summary>
        private static Dictionary<string, StringBuilder> criticalLogMap = new Dictionary<string, StringBuilder>();

        /// <summary>
        /// Record critical log
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        public static void AddCriticalLog(string category, string name)
        {
            if (criticalLogMap.ContainsKey(category) == false)
                criticalLogMap[category] = new StringBuilder();

            criticalLogMap[category].AppendLine(name);
        }

        /// <summary>
        /// Called before scan
        /// </summary>
        private void OnPreScan()
        {
            criticalLogMap.Clear();
        }

        /// <summary>
        /// Called after scan
        /// </summary>
        /// <param name="assetType"></param>
        private void OnPostScan(AssetType assetType)
        {
            ExportCriticalLog(assetType);
        }

        /// <summary>
        /// Scan the asset type
        /// </summary>
        /// <param name="assetType"></param>
        public void Scan(AssetType assetType, string filter = null)
        {
            OnPreScan();

            assetType.ValidateExportPath();

            switch (assetType)
            {
                case AssetType.Texture:
                    ScanTexture(filter);
                    break;

                case AssetType.SpriteAtlas:
                    ScanSpriteAtlas(filter);
                    break;

                case AssetType.Animation:
                    ScanAnimation(filter);
                    break;

                case AssetType.Model:
                    ScanModel(filter);
                    break;

                case AssetType.AudioClip:
                    ScanAudioClip(filter);
                    break;

                default:
                    Debug.LogError($"[ResourceStatistics.Application] Not implemented asset type {assetType}");
                    break;
            }

            OnPostScan(assetType);
        }

        /// <summary>
        /// Scan all texture type asset
        /// </summary>
        private void ScanTexture(string filter = null)
        {
            var addColumnChecker = new HashSet<string>();
            var platformWriterMap = new Dictionary<string, StreamWriter>();

            foreach (var platform in TextureInfo.Platforms)
                platformWriterMap[platform] = Helper.OpenCsvWriter(AssetType.Texture, platform);

            using var defaultWriter = Helper.OpenCsvWriter(AssetType.Texture, "Default");

            var paths = AssetType.Texture.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                try
                {
                    using var info = new TextureInfo(importer);

                    #region Default

                    var defaultField = info.GetDefaultFields();

                    // Column
                    if (addColumnChecker.Contains("Default") == false)
                    {
                        addColumnChecker.Add("Default");
                        defaultWriter.WriteLine(defaultField.Keys.ToCsvLine());
                    }

                    defaultWriter.WriteLine(defaultField.Values.ToCsvLine());

                    #endregion Default

                    #region Platform

                    foreach (var platform in TextureInfo.Platforms)
                    {
                        var platformField = info.GetPlatformFields(platform);

                        // Column
                        if (addColumnChecker.Contains(platform) == false)
                        {
                            addColumnChecker.Add(platform);
                            platformWriterMap[platform].WriteLine(platformField.Keys.ToCsvLine());
                        }

                        platformWriterMap[platform].WriteLine(platformField.Values.ToCsvLine());
                    }

                    #endregion Platform
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }

            foreach (var writer in platformWriterMap.Values)
                writer.Dispose();
        }

        /// <summary>
        /// Scan all atlas type asset
        /// </summary>
        private void ScanSpriteAtlas(string filter = null)
        {
            var addColumnChecker = new HashSet<string>();
            var platformWriterMap = new Dictionary<string, StreamWriter>();

            foreach (var platform in SpriteAtlasInfo.Platforms)
                platformWriterMap[platform] = Helper.OpenCsvWriter(AssetType.SpriteAtlas, platform);

            using var defaultWriter = Helper.OpenCsvWriter(AssetType.SpriteAtlas, "Default");

            var paths = AssetType.SpriteAtlas.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                try
                {
                    using var info = new SpriteAtlasInfo(importer);

                    #region Default

                    var defaultField = info.GetDefaultFields();

                    // Column
                    if (addColumnChecker.Contains("Default") == false)
                    {
                        addColumnChecker.Add("Default");
                        defaultWriter.WriteLine(defaultField.Keys.ToCsvLine());
                    }

                    defaultWriter.WriteLine(defaultField.Values.ToCsvLine());

                    #endregion Default

                    #region Platform

                    foreach (var platform in SpriteAtlasInfo.Platforms)
                    {
                        var platformField = info.GetPlatformFields(platform);

                        // Column
                        if (addColumnChecker.Contains(platform) == false)
                        {
                            addColumnChecker.Add(platform);
                            platformWriterMap[platform].WriteLine(platformField.Keys.ToCsvLine());
                        }

                        platformWriterMap[platform].WriteLine(platformField.Values.ToCsvLine());
                    }

                    #endregion Platform
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }

            foreach (var writer in platformWriterMap.Values)
                writer.Dispose();
        }

        /// <summary>
        /// Scan all animation type asset
        /// </summary>
        private void ScanAnimation(string filter = null)
        {
            var paths = AssetType.Animation.ToAssetPaths(filter);
            var addColumn = false;

            using var defaultWriter = Helper.OpenCsvWriter(AssetType.Animation, "Default");

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                try
                {
                    using var info = new AnimationInfo(importer);

                    #region Default

                    var defaultField = info.GetDefaultFields();
                    var subAssetDefaultFiles = info.GetSubAssetDefaultFields();

                    // Main
                    if (addColumn == false)
                    {
                        addColumn = true;
                        defaultWriter.WriteLine(defaultField.Keys.ToCsvLine());
                    }
                    defaultWriter.WriteLine(defaultField.Values.ToCsvLine());

                    // Sub
                    foreach(var subAssetField in subAssetDefaultFiles)
                    {
                        // Column
                        if (addColumn == false)
                        {
                            addColumn = true;
                            defaultWriter.WriteLine(subAssetField.Keys.ToCsvLine());
                        }
                        defaultWriter.WriteLine(subAssetField.Values.ToCsvLine());
                    }

                    #endregion Default
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }
        }

        /// <summary>
        /// Scan all mesh type asset
        /// </summary>
        private void ScanModel(string filter = null)
        {
            var paths = AssetType.Model.ToAssetPaths(filter);
            var addColumn = false;
            var counter = 0;

            using var defaultWriter = Helper.OpenCsvWriter(AssetType.Model, "Default");

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                try
                {
                    using var info = new ModelInfo(importer);

                    #region Default

                    var defaultField = info.GetDefaultFields();

                    // Column
                    if (addColumn == false)
                    {
                        addColumn = true;
                        defaultWriter.WriteLine(defaultField.Keys.ToCsvLine());
                    }

                    defaultWriter.WriteLine(defaultField.Values.ToCsvLine());

                    #endregion Default
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }

                // GameObject은 Resources.UnloadAsset 불가 → 주기적으로 일괄 해제
                if (++counter % 50 == 0)
                    EditorUtility.UnloadUnusedAssetsImmediate(false);
            }

            EditorUtility.UnloadUnusedAssetsImmediate(false);
        }

        /// <summary>
        /// Scan all audio clip type asset
        /// </summary>
        private void ScanAudioClip(string filter = null)
        {
            var addColumnChecker = new HashSet<string>();
            var platformWriterMap = new Dictionary<string, StreamWriter>();

            foreach (var platform in AudioClipInfo.Platforms)
                platformWriterMap[platform] = Helper.OpenCsvWriter(AssetType.AudioClip, platform);

            using var defaultWriter = Helper.OpenCsvWriter(AssetType.AudioClip, "Default");

            var paths = AssetType.AudioClip.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                try
                {
                    using var info = new AudioClipInfo(importer);

                    #region Default

                    var defaultField = info.GetDefaultFields();

                    // Column
                    if (addColumnChecker.Contains("Default") == false)
                    {
                        addColumnChecker.Add("Default");
                        defaultWriter.WriteLine(defaultField.Keys.ToCsvLine());
                    }

                    defaultWriter.WriteLine(defaultField.Values.ToCsvLine());

                    #endregion Default

                    #region Platform

                    foreach (var platform in AudioClipInfo.Platforms)
                    {
                        var platformField = info.GetPlatformFields(platform);

                        // Column
                        if (addColumnChecker.Contains(platform) == false)
                        {
                            addColumnChecker.Add(platform);
                            platformWriterMap[platform].WriteLine(platformField.Keys.ToCsvLine());
                        }

                        platformWriterMap[platform].WriteLine(platformField.Values.ToCsvLine());
                    }

                    #endregion Platform
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }

            foreach (var writer in platformWriterMap.Values)
                writer.Dispose();
        }

        /// <summary>
        /// Export critical log
        /// </summary>
        /// <param name="assetType"></param>
        private void ExportCriticalLog(AssetType assetType)
        {
            if (criticalLogMap.Keys.Count <= 0)
                return;

            Helper.ExportCriticalLog(assetType, criticalLogMap);
        }
    }
}