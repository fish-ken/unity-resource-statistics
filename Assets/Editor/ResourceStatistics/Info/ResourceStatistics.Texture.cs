using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// Texture import information class
    /// </summary>
    public class TextureInfo : AssetInfo
    {
        /// <summary>
        /// List of platforms to scan
        /// </summary>
        public static readonly List<string> Platforms = new List<string>()
        {
            "Android",
            "iOS",
            "Standalone",

            // Add platform, if you need
        };

        private Texture texture = null;

        private TextureImporter textureImporter = null;

        private Dictionary<string, TextureImporterPlatformSettings> platformMap =
            new Dictionary<string, TextureImporterPlatformSettings>();

        public TextureInfo(AssetImporter importer) : base(importer)
        {
            textureImporter = Importer as TextureImporter;

            Initialize();
        }

        private void Initialize()
        {
            texture = EditorResources.Load<Texture>(textureImporter.assetPath);

            foreach (var platform in Platforms)
                platformMap[platform] = textureImporter.GetPlatformTextureSettings(platform);
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = textureImporter.assetPath,

                // Texture
                ["Texture Type"] = textureImporter.textureType.ToString(),
                ["sRGB (Color Texture)"] = textureImporter.sRGBTexture.ToString(),
                ["Alpha Source"] = textureImporter.alphaSource.ToString(),
                ["Alpha Is Transparency"] = textureImporter.alphaIsTransparency.ToString(),
                ["Read/Write Enabled"] = textureImporter.isReadable.ToString(),
                ["Generate Mip Maps"] = textureImporter.mipmapEnabled.ToString(),
                ["Max size"] = textureImporter.maxTextureSize.ToString(),
                ["Original Width/Height"] = $"{texture.width}x{texture.height}",

                // Size
                ["Original Size(KB)"] = Helper.GetFileSize(FullPath).ToString(),
                ["Imported Size(KB)"] = Helper.GetMemorySize(texture).ToString(),
            };

            if (Editor.EnableLog)
                Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }

        public Dictionary<string, string> GetPlatformFields(string platform)
        {
            if (platformMap.ContainsKey(platform) == false || platformMap[platform] == null)
                return null;

            var platformImporter = platformMap[platform];
            var format = platformImporter.format == TextureImporterFormat.Automatic ? textureImporter.GetAutomaticFormat(platform) : platformImporter.format;

            var fieldMap = new Dictionary<string, string>()
            {
                // Basic
                ["Path/Name"] = textureImporter.assetPath,

                // Platform
                ["Overridden"] = platformImporter.overridden.ToString(),
                ["Max size"] = platformImporter.maxTextureSize.ToString(),
                ["Resize algorithm"] = platformImporter.resizeAlgorithm.ToString(),
                ["Format"] = format.ToString(),
                ["Compressor quality"] = platformImporter.compressionQuality.ToString(),
            };

            fieldMap["Original size"] = $"{texture.width}x{texture.height}";

            if (platform == Platforms[0])
                fieldMap["Android ETC2 Fallback override"] = platformImporter.androidETC2FallbackOverride.ToString();

            if (Editor.EnableLog)
                Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }
    }
}