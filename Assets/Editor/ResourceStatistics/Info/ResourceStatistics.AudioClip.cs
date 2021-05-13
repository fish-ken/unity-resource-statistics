using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// Mesh audio information class
    /// </summary>
    public class AudioClipInfo : AssetInfo
    {
        public static readonly List<string> Platforms = new List<string>()
        {
            "Android",
            "iOS",
            "Standalone",

            // Add platform, if you need
        };

        private AudioImporter audioClipImporter = null;

        private AudioClip audioClip = null;

        private Dictionary<string, AudioImporterSampleSettings> platformMap =
            new Dictionary<string, AudioImporterSampleSettings>();

        public AudioClipInfo(AssetImporter importer) : base(importer)
        {
            Initialize();
        }

        private void Initialize()
        {
            audioClipImporter = Importer as AudioImporter;
            audioClip = EditorResources.Load<AudioClip>(Importer.assetPath);

            foreach (var platform in Platforms)
            {
                platformMap[platform] = audioClipImporter.GetOverrideSampleSettings(platform);
            }
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            var defaultSetting = audioClipImporter.defaultSampleSettings;

            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,

                // Common
                ["Force To Mono"] = audioClipImporter.forceToMono.ToString(),
                ["Load In Background"] = audioClipImporter.loadInBackground.ToString(),
                ["Ambisonic"] = audioClipImporter.ambisonic.ToString(),

                // Default platform
                ["Load Type"] = defaultSetting.loadType.ToString(),
                ["Preload Audio Data"] = audioClipImporter.preloadAudioData.ToString(),
                ["Compression Format"] = defaultSetting.compressionFormat.ToString(),
                ["Quality"] = GetQualityString(defaultSetting),
                ["Sample Rate Setting"] = GetSampleRateString(defaultSetting),

                // Extra
                // [""] = defaultSetting.
            };

            if (Editor.EnableLog)
                Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }

        public Dictionary<string, string> GetPlatformFields(string platform)
        {
            if (platformMap.ContainsKey(platform) == false)
                return null;

            var platformImporter = platformMap[platform];

            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,

                // Common
                ["Force To Mono"] = audioClipImporter.forceToMono.ToString(),
                ["Load In Background"] = audioClipImporter.loadInBackground.ToString(),
                ["Ambisonic"] = audioClipImporter.ambisonic.ToString(),

                // Platform
                ["Load Type"] = platformImporter.loadType.ToString(),
                ["Preload Audio Data"] = audioClipImporter.preloadAudioData.ToString(),
                ["Compression Format"] = platformImporter.compressionFormat.ToString(),
                ["Quality"] = GetQualityString(platformImporter),
                ["Sample Rate Setting"] = GetSampleRateString(platformImporter),
            };

            if (Editor.EnableLog)
                Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }

        private static string GetSampleRateString(AudioImporterSampleSettings importer)
        {
            return importer.sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate
                ? $"{importer.sampleRateSetting} ({importer.sampleRateOverride}"
                : importer.sampleRateSetting.ToString();
        }

        private static string GetQualityString(AudioImporterSampleSettings importer)
        {
            var format = importer.compressionFormat;

            return format == AudioCompressionFormat.Vorbis || format == AudioCompressionFormat.AAC
                ? "100"
                : $"{((int)importer.quality * 100)}";
        }
    }
}