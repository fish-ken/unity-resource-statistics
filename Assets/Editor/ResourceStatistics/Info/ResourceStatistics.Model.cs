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
            }
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
                ["Original Size(KB)"] = Helper.GetFileSize(FullPath).ToString(),
                ["Imported Size(KB)"] = Helper.GetMemorySize(gameObject).ToString(),
            };

            if (Editor.EnableLog)
                Debug.Log(fieldMap.ToJson());

            return fieldMap;
        }
    }
}