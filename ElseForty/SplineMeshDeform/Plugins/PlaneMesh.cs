using System.Collections.Generic;
using UnityEngine;

namespace ElseForty
{
    public class PlaneMesh : MonoBehaviour, IMeshModifier
    {
        public delegate void OnUpdate();
        public static event OnUpdate Update_Mesh;

        public Vector2[] uvs;
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector4> tangents = new List<Vector4>();
        public int[] triangles;

        public GameObject MeshHolder;
        public MeshFilter Mesh;
        public MeshRenderer MeshRenderer;
        public Material Material;
        public SPData SPData;
        public Switch FlipFaces = Switch.Off;
        public Switch TwoSided = Switch.Off;

        public float Width = 1f;
        public float UVRotation;

        public string AssetName = "";
        int j;

        public void Awake()
        {
            SPData = GetComponent<SplinePlus>().sPData;
            MeshRenderer = MeshHolder.GetComponent<MeshRenderer>();
            Mesh = MeshHolder.GetComponent<MeshFilter>();
 
            SPData.MeshModifier = this;
        }

        private void OnEnable()
        {
            SPData.MeshModifier = this;
            SplineCreationClass.Update_Spline += Update_Spline;
        }

        void OnDisable()
        {
            SPData.MeshModifier = null;
            SplineCreationClass.Update_Spline -= Update_Spline;
        }

        void Update_Spline()
        {
            DrawMesh();
        }

        private void OnDestroy()
        {
            Destroy(MeshHolder);
        }


        public void DrawMesh()
        {
            if (SPData.Vertices.Count < 2) return;
            vertices.Clear();
            normals.Clear();
            tangents.Clear();


            for (int i = 0; i < SPData.Vertices.Count; i++)//vertices normals tangents
            {
                Vertices(i);
                Normals(i);
                Tangents(i);
            }

            Triangles();
            Uvs();

            var finalMesh = CreateMesh();

            if (TwoSided == Switch.On) finalMesh = FacesSettings.TwoSided(finalMesh);
            if (FlipFaces == Switch.On) finalMesh = FacesSettings.FlipFaces(finalMesh);
            Mesh.sharedMesh = finalMesh;

            MeshRenderer.sharedMaterial = Material;
           if (SPData.MeshUpdate!=null) SPData.MeshUpdate.Update();
        }

        public void Vertices(int i)
        {
            var vertex1 = (SPData.Vertices[i] + Vector3.Cross(SPData.Tangents[i], SPData.Normals[i]) * Width);
            var vertex2 = (SPData.Vertices[i] + Vector3.Cross(SPData.Tangents[i], SPData.Normals[i]) * -Width);


            vertices.Add(transform.InverseTransformPoint(vertex1));
            vertices.Add(transform.InverseTransformPoint(vertex2));
        }

        public void Normals(int i)
        {
            normals.Add(SPData.Normals[i]);
            normals.Add(SPData.Normals[i]);
        }

        public void Tangents(int i)
        {
            tangents.Add(SPData.Tangents[i]);
            tangents.Add(SPData.Tangents[i]);
        }

        public void Uvs()
        {
            uvs = new Vector2[vertices.Count];
            float h = SPData.Length / (Width * 2);
            for (int n = 0, i = 0; i < SPData.Vertices.Count; i++, n = n + 2)
            {
                var x = Mathf.InverseLerp(0, SPData.Length, SPData.VerticesDistance[i]) * h;


                uvs[n] = new Vector2(x, 0);
                uvs[n + 1] = new Vector2(x, 1);

                uvs[n] = SplinePlusAPI.Vector2_Rotate_Around_Pivot(uvs[n], UVRotation);
                uvs[n + 1] = SplinePlusAPI.Vector2_Rotate_Around_Pivot(uvs[n + 1], UVRotation);
            }
        }

        public void Triangles()
        {
            triangles = new int[(vertices.Count - 2) * 3];
            for (int i = 0; i < vertices.Count - 2; i = i + 2)
            {
                triangles[j] = i;
                triangles[j + 1] = i + 2;
                triangles[j + 2] = i + 1;

                triangles[j + 3] = i + 1;
                triangles[j + 4] = i + 2;
                triangles[j + 5] = i + 3;

                j = j + 6;
                if (i == (vertices.Count - 2) || j >= triangles.Length) j = 0;
            }
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();

            MeshRenderer.sharedMaterial = Material;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles;
            mesh.normals = normals.ToArray();
            mesh.tangents = tangents.ToArray();
            mesh.uv = uvs;
            mesh.RecalculateBounds();

            return mesh;
        }


        public void PlanarUvs(Mesh mesh)
        {
            var newUvs = new Vector2[mesh.uv.Length];
            for (int i = 0; i < mesh.uv.Length; i++)
            {
                var a = Mathf.InverseLerp(mesh.bounds.max.x, mesh.bounds.min.x, mesh.vertices[i].x);
                var b = Mathf.InverseLerp(mesh.bounds.max.z, mesh.bounds.min.z, mesh.vertices[i].z);
                newUvs[i] = new Vector2(a, b);
            }
            mesh.uv = newUvs;
        }
    }

}