using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace ResourceStatistics
{
    /// <summary>
    /// ResourceStatistics`s application class
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Scan the asset type
        /// </summary>
        /// <param name="assetType"></param>
        public void Scan(AssetType assetType, string filter = null)
        {
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
                    UnityEngine.Debug.LogError($"[ResourceStatistics.Application] Not implemented asset type {assetType}");
                    break;
            }
        }

        /// <summary>
        /// Scan all texture type asset
        /// </summary>
        private void ScanTexture(string filter = null)
        {
            var defaultCsv = new StringBuilder();
            var platformCsvMap = new Dictionary<string, StringBuilder>();
            var addColumnChecker = new HashSet<string>();

            foreach (var platform in TextureInfo.Platforms)
                platformCsvMap[platform] = new StringBuilder();

            var paths = AssetType.Texture.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                var info = new TextureInfo(importer);

                #region Default

                var defaultField = info.GetDefaultFields();

                // Column
                if (addColumnChecker.Contains("Default") == false)
                {
                    addColumnChecker.Add("Default");
                    defaultCsv.AppendLine(defaultField.Keys.ToCsvLine());
                }

                defaultCsv.AppendLine(defaultField.Values.ToCsvLine());

                #endregion Default

                #region Platform

                foreach (var platform in TextureInfo.Platforms)
                {
                    var platformField = info.GetPlatformFields(platform);

                    // Column
                    if (addColumnChecker.Contains(platform) == false)
                    {
                        addColumnChecker.Add(platform);
                        platformCsvMap[platform].AppendLine(platformField.Keys.ToCsvLine());
                    }

                    platformCsvMap[platform].AppendLine(platformField.Values.ToCsvLine());
                }

                #endregion Platform
            }

            #region Export

            Helper.ExportResult(AssetType.Texture, "Default", defaultCsv);
            foreach (var platform in TextureInfo.Platforms)
                Helper.ExportResult(AssetType.Texture, platform, platformCsvMap[platform]);

            #endregion Export
        }

        /// <summary>
        /// Scan all atlas type asset
        /// </summary>
        private void ScanSpriteAtlas(string filter = null)
        {
            var defaultCsv = new StringBuilder();
            var platformCsvMap = new Dictionary<string, StringBuilder>();
            var addColumnChecker = new HashSet<string>();

            foreach (var platform in SpriteAtlasInfo.Platforms)
                platformCsvMap[platform] = new StringBuilder();

            var paths = AssetType.SpriteAtlas.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                var info = new SpriteAtlasInfo(importer);

                #region Default

                var defaultField = info.GetDefaultFields();

                // Column
                if (addColumnChecker.Contains("Default") == false)
                {
                    addColumnChecker.Add("Default");
                    defaultCsv.AppendLine(defaultField.Keys.ToCsvLine());
                }

                defaultCsv.AppendLine(defaultField.Values.ToCsvLine());

                #endregion Default

                #region Platform

                foreach (var platform in SpriteAtlasInfo.Platforms)
                {
                    var platformField = info.GetPlatformFields(platform);

                    // Column
                    if (addColumnChecker.Contains(platform) == false)
                    {
                        addColumnChecker.Add(platform);
                        platformCsvMap[platform].AppendLine(platformField.Keys.ToCsvLine());
                    }

                    platformCsvMap[platform].AppendLine(platformField.Values.ToCsvLine());
                }

                #endregion Platform
            }

            #region Export

            Helper.ExportResult(AssetType.SpriteAtlas, "Default", defaultCsv);
            foreach (var platform in SpriteAtlasInfo.Platforms)
                Helper.ExportResult(AssetType.SpriteAtlas, platform, platformCsvMap[platform]);

            #endregion Export
        }

        /// <summary>
        /// Scan all animation type asset
        /// </summary>
        private void ScanAnimation(string filter = null)
        {
            var paths = AssetType.Animation.ToAssetPaths(filter);
            var defaultCsv = new StringBuilder();
            var addColumn = false;

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                var info = new AnimationInfo(importer);

                #region Default

                var defaultField = info.GetDefaultFields();

                // Column
                if (addColumn == false)
                {
                    addColumn = true;
                    defaultCsv.AppendLine(defaultField.Keys.ToCsvLine());
                }

                defaultCsv.AppendLine(defaultField.Values.ToCsvLine());

                #endregion Default
            }

            #region Export

            Helper.ExportResult(AssetType.Animation, "Default", defaultCsv);

            #endregion Export
        }

        /// <summary>
        /// Scan all mesh type asset
        /// </summary>
        private void ScanModel(string filter = null)
        {
            var paths = AssetType.Model.ToAssetPaths(filter);
            var defaultCsv = new StringBuilder();
            var addColumn = false;

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                var info = new ModelInfo(importer);

                #region Default

                var defaultField = info.GetDefaultFields();

                // Column
                if (addColumn == false)
                {
                    addColumn = true;
                    defaultCsv.AppendLine(defaultField.Keys.ToCsvLine());
                }

                defaultCsv.AppendLine(defaultField.Values.ToCsvLine());

                #endregion Default
            }

            #region Export

            Helper.ExportResult(AssetType.Model, "Default", defaultCsv);

            #endregion Export
        }

        /// <summary>
        /// Scan all audio clip type asset
        /// </summary>
        private void ScanAudioClip(string filter = null)
        {
            var defaultCsv = new StringBuilder();
            var platformCsvMap = new Dictionary<string, StringBuilder>();
            var addColumnChecker = new HashSet<string>();

            foreach (var platform in AudioClipInfo.Platforms)
                platformCsvMap[platform] = new StringBuilder();

            var paths = AssetType.AudioClip.ToAssetPaths(filter);

            foreach (var path in paths)
            {
                var importer = AssetImporter.GetAtPath(path);

                if (importer == null)
                    continue;

                var info = new AudioClipInfo(importer);

                #region Default

                var defaultField = info.GetDefaultFields();

                // Column
                if (addColumnChecker.Contains("Default") == false)
                {
                    addColumnChecker.Add("Default");
                    defaultCsv.AppendLine(defaultField.Keys.ToCsvLine());
                }

                defaultCsv.AppendLine(defaultField.Values.ToCsvLine());

                #endregion Default

                #region Platform

                foreach (var platform in AudioClipInfo.Platforms)
                {
                    var platformField = info.GetPlatformFields(platform);

                    // Column
                    if (addColumnChecker.Contains(platform) == false)
                    {
                        addColumnChecker.Add(platform);
                        platformCsvMap[platform].AppendLine(platformField.Keys.ToCsvLine());
                    }

                    platformCsvMap[platform].AppendLine(platformField.Values.ToCsvLine());
                }

                #endregion Platform
            }

            #region Export

            Helper.ExportResult(AssetType.AudioClip, "Default", defaultCsv);
            foreach (var platform in AudioClipInfo.Platforms)
                Helper.ExportResult(AssetType.AudioClip, platform, platformCsvMap[platform]);

            #endregion Export
        }
    }
}