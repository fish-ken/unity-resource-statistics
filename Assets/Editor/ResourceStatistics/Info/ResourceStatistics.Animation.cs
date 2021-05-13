using System.Collections.Generic;
using System.IO;
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

        public AnimationInfo(AssetImporter importer) : base(importer)
        {
            Initialize();
        }

        private void Initialize()
        {
            animationClip = EditorResources.Load<AnimationClip>(Importer.assetPath);
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,
                
                // Animation 
                ["Framerate"] = ((int)animationClip.frameRate).ToString(),
                ["Length"] = animationClip.length.ToString(),
                
                // Size
                ["Original Size(KB)"] = Helper.GetFileSize(FullPath).ToString(),
                ["Imported Size(KB)"] = Helper.GetMemorySize(animationClip).ToString(),
            };

            if (Editor.EnableLog)
                Debug.Log(animationClip.ToJson());

            return fieldMap;
        }
    }
}