using System;
using System.Collections.Generic;
using UnityEditor;

namespace ResourceStatistics
{
    public abstract class AssetInfo : IDisposable
    {
        protected AssetImporter Importer { get; private set; }

        protected string FullPath
        {
            get
            {
                if (Importer == null)
                    return null;

                return $"{Util.ProjectRootPath}/{Importer.assetPath}";
            }
        }

        protected AssetInfo(AssetImporter importer)
        {
            Importer = importer;
        }

        public virtual void Dispose() { }

        protected void Log(Dictionary<string, string> fieldMap)
        {
            if (Editor.EnableLog == false)
                return;

            if (fieldMap == null)
            {
                Debug.LogError($"Error - {nameof(Log)} : fieldMap is null");
                return;
            }

            Debug.Log(fieldMap.ToJson());
        }
    }
}