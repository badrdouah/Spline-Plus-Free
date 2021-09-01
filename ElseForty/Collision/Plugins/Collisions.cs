using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElseForty
{
    public class Collisions : MonoBehaviour, IMeshUpdate
    {
        public SPData sPData;
        public EdgeCollider2D col;

        void Awake()
        {
            sPData = GetComponent<SplinePlus>().sPData;
            col = GetComponent<EdgeCollider2D>();
            if (col == null) col = this.gameObject.AddComponent<EdgeCollider2D>();
            sPData.MeshUpdate = this;
        }


     //   private void OnEnable()
     //   {
     //       SplineCreationClass.Update_Spline += UpdateSpline;
     //   }
     //
     //   private void OnDisable()
     //   {
     //       SplineCreationClass.Update_Spline -= UpdateSpline;
     //   }
     //
     //   // Update is called once per frame
     //   public void UpdateSpline()
     //   {
     //  
     //   }

        public void Update()
        {
            var colliderPoints = new Vector2[sPData.Vertices.Count];
            for (int i = 0; i < sPData.Vertices.Count; i++)
            {
                colliderPoints[i] = new Vector2(sPData.Vertices[i].x, sPData.Vertices[i].y);
            }
            col.points = colliderPoints;
        }
    }

}
