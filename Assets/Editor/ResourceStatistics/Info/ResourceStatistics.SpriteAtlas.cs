using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.U2D;
using UnityEngine.U2D;

namespace ResourceStatistics
{
    /// <summary>
    /// Sprite atlas import information class
    /// </summary>
    public class SpriteAtlasInfo : AssetInfo
    {
        /// <summary>
        /// List of platforms to scan
        /// </summary>
        public static readonly List<string> Platforms = new List<string>()
        {
            "Android",
            "iPhone",            // instead of 'iOS'
            "Standalone",

            // Add platform, if you need
        };

        private SpriteAtlas spriteAtlas = null;

        private Dictionary<string, TextureImporterPlatformSettings> platformMap =
            new Dictionary<string, TextureImporterPlatformSettings>();

        public SpriteAtlasInfo(AssetImporter importer) : base(importer)
        {
            Initialize();
        }

        private void Initialize()
        {
            spriteAtlas = EditorResources.Load<SpriteAtlas>(Importer.assetPath);

            foreach (var platform in Platforms)
                platformMap[platform] = spriteAtlas.GetPlatformSettings(platform);
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            var defaultSetting = spriteAtlas.GetTextureSettings();
            var packingSetting = spriteAtlas.GetPackingSettings();
            var platformSetting = spriteAtlas.GetPlatformSettings("Default");

            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,
                ["Atlas Type"] = spriteAtlas.isVariant ? "Variant" : "Master",

                // Packing
                ["Allow Rotation"] = packingSetting.enableRotation.ToString(),
                ["Tight Packing"] = packingSetting.enableTightPacking.ToString(),
                ["Padding"] = packingSetting.padding.ToString(),

                // Texture
                ["Read/Write Enabled"] = defaultSetting.readable.ToString(),
                ["Generate Mip Maps"] = defaultSetting.generateMipMaps.ToString(),
                ["sRGB (Color Texture)"] = defaultSetting.sRGB.ToString(),
                ["Max size"] = platformSetting.maxTextureSize.ToString(),
            };

            if (Editor.EnableLog)
                UnityEngine.Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }

        public Dictionary<string, string> GetPlatformFields(string platform)
        {
            if (platformMap.ContainsKey(platform) == false || platformMap[platform] == null)
                return null;

            var platformImporter = platformMap[platform];
            var format = platformImporter.format == TextureImporterFormat.Automatic ? spriteAtlas.GetPlatformSettings(platform).format : platformImporter.format;

            var fieldMap = new Dictionary<string, string>()
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,

                // Platform
                ["Overridden"] = platformImporter.overridden.ToString(),
                ["Max size"] = platformImporter.maxTextureSize.ToString(),
                ["Format"] = format.ToString(),
                ["Compressor quality"] = platformImporter.compressionQuality.ToString(),
            };

            if (Editor.EnableLog)
                UnityEngine.Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }
    }
}