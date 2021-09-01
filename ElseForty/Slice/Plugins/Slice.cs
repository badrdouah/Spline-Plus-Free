using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElseForty
{
    public class Slice : MonoBehaviour
    {
        public float range=0f;
        public SPData sPData;

        void Awake()
        {
            sPData = GetComponent<SplinePlus>().sPData;
            range = 0f;
          //  SplineCreationClass.Update(sPData,false);
          //  UpdateSpline();


        }

        private void OnEnable()
        {
            SplineCreationClass.Update_Spline += UpdateSpline;
        }

        private void OnDisable()
        {
            SplineCreationClass.Update_Spline -= UpdateSpline;
        }

        public void UpdateSpline()
        {
   
            var progress = Mathf.Lerp(0, sPData.Length, range);

           // if (progress == 0)
           // {
           //     sPData.Vertices.Clear();
           //     if (sPData.MeshModifier != null) sPData.MeshModifier.DrawMesh();
           //     return;
           // }





            int index = 0;

            for (int i = sPData.VerticesDistance.Count - 2; i >= 0; i--)
            {
                if (progress >= sPData.VerticesDistance[i])
                {
                    index = i;
                    break;
                }
            }

            List<Vector3> v = new List<Vector3>();
            for (int i = 0; i <= index; i++)
            {
                v.Add(sPData.VerticesCached[i]);
            }

            var a = index;
            var b = a + 1;

            var vertexA = sPData.VerticesCached[a];
            var vertexB = sPData.VerticesCached[b];

            var t = Mathf.InverseLerp(sPData.VerticesDistance[a], sPData.VerticesDistance[b], progress);

            var vert = Vector3.Lerp(vertexA, vertexB, t);
            v.Add(vert);

            sPData.Vertices = v;



            if (sPData.MeshModifier != null) sPData.MeshModifier.DrawMesh();
        }
    }

}
