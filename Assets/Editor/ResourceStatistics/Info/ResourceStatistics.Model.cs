using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace ResourceStatistics
{
    /// <summary>
    /// Mesh import information class
    /// </summary>
    public class ModelInfo : AssetInfo
    {
        private ModelImporter modelImporter = null;

        private GameObject gameObject = null;

        private int vertexCount = 0;

        private int triangleCount = 0;

        private long importedMemoryBytes = 0;

        public ModelInfo(AssetImporter importer) : base(importer)
        {
            Initialize();
        }

        private void Initialize()
        {
            modelImporter = (ModelImporter)Importer;
            gameObject = EditorResources.Load<GameObject>(Importer.assetPath);

            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh == null)
                    continue;

                vertexCount += meshFilter.sharedMesh.vertexCount;
                triangleCount += meshFilter.sharedMesh.triangles.Length;
                importedMemoryBytes += UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(meshFilter.sharedMesh);
            }
        }

        public override void Dispose()
        {
            gameObject = null;
        }

        public Dictionary<string, string> GetDefaultFields()
        {
            var fieldMap = new Dictionary<string, string>
            {
                // Basic
                ["Path/Name"] = Importer.assetPath,

                // Mesh
                ["Vertex count"] = vertexCount.ToString(),
                ["Triangle count"] = triangleCount.ToString(),

                // Extra 
                ["Read/Write Enabled"] = modelImporter.isReadable.ToString(),
                ["Mesh Compression"] = modelImporter.meshCompression.ToString(),
                ["Optimize Mesh/Vertex"] = modelImporter.optimizeMeshVertices.ToString(),
                ["Optimize Mesh/Polygon"] = modelImporter.optimizeMeshPolygons.ToString(),
                ["Import Normals"] = modelImporter.importNormals.ToString(),
                ["Import Tangents"] = modelImporter.importTangents.ToString(),
                ["Original Size(kB)"] = Helper.GetFileSize(FullPath).ToString(),
                ["Imported Size(kB)"] = (importedMemoryBytes / 1024f).ToString(),
                ["Imported Size(MB)"] = (importedMemoryBytes / (1024f * 1024f)).ToString(),
            };

            #region Critical logs

            if (modelImporter.isReadable)
                Application.AddCriticalLog("Write Enabled", Importer.assetPath);

            #endregion

            Log(fieldMap);
            return fieldMap;
        }
    }
}