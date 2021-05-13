using UnityEditor;

namespace ResourceStatistics
{
    public abstract class AssetInfo
    {
        protected AssetImporter Importer { get; private set; }

        protected string FullPath
        {
            get
            {
                if (Importer == null)
                    return null;

                return $"{PathExtension.ProjectRootPath}/{Importer.assetPath}";
            }
        }

        protected AssetInfo(AssetImporter importer)
        {
            Importer = importer;
        }
    }
}