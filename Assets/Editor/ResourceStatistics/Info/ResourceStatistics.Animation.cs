using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// Sprite atlas import information class
    /// </summary>
    public class AnimationInfo : AssetInfo
    {
        private AnimationClip animationClip = null;
        private List<AnimationClip> subAnimationClips = new List<AnimationClip>();

        public AnimationInfo(AssetImporter importer) : base(importer)
        {
            Initialize();
        }

        private void Initialize()
        {
            animationClip = EditorResources.Load<AnimationClip>(Importer.assetPath);

            subAnimationClips.Clear();

            var assets = AssetDatabase.LoadAllAssetsAtPath(Importer.assetPath);
            var clips = new List<AnimationClip>();
            foreach (var asset in assets)
            {
                if (asset is AnimationClip clip)
                {
                    subAnimationClips.Add(clip);
                }
            }
        }

        public override void Dispose()
        {
            foreach (var clip in subAnimationClips)
                if (clip != null) Resources.UnloadAsset(clip);
            if (animationClip != null) Resources.UnloadAsset(animationClip);
        }

        private Dictionary<string, string> GetAnimationClipFileds(string assetPath, AnimationClip clip)
        {
            if (animationClip == null)
            {
                Debug.Log(assetPath);
            }

            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = assetPath,

                // Animation 
                ["Framerate"] = ((int)animationClip.frameRate).ToString(),
                ["Length"] = animationClip.length.ToString(),

                // Size
                ["Original Size(kB)"] = Helper.GetFileSize(FullPath).ToString(),
                ["Imported Size(kB)"] = Helper.GetMemorySize_kB(animationClip).ToString(),
                ["Imported Size(MB)"] = Helper.GetMemorySize_MB(animationClip).ToString(),
            };

            if (Editor.EnableLog)
                Debug.Log(animationClip.ToJson());

            return fieldMap;
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            return GetAnimationClipFileds(Importer.assetPath, animationClip);
        }

        public List<Dictionary<string, string>> GetSubAssetDefaultFields()
        {
            var result = new List<Dictionary<string, string>>(subAnimationClips.Count);

            foreach(var subAnimationClip in subAnimationClips)
            {
                var fieldMap = GetAnimationClipFileds($"{Importer.assetPath}|{subAnimationClip.name}", subAnimationClip);
                result.Add(fieldMap);
            }

            return result;
        }
    }
}