using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ElseForty;

[CustomEditor(typeof(Collisions))]
public class CollisionsEditor : Editor
{
    Collisions collisions;
    private void Awake()
    {
        collisions = (Collisions)target;
        collisions.sPData = collisions.gameObject.GetComponent<SplinePlus>().sPData;
        collisions.sPData.MeshUpdate =  collisions;

        collisions.col = collisions.gameObject.GetComponent<EdgeCollider2D>();
        if (collisions.col == null) collisions.col = collisions.gameObject.AddComponent<EdgeCollider2D>();
    }


        // Start is called before the first frame update
      private void OnEnable()
      {
            collisions.sPData.MeshUpdate = collisions;
            //  SplineCreationClass.Update_Spline += collisions.UpdateSpline;
      }

  //  private void OnDisable()
  //  {
  //      SplineCreationClass.Update_Spline -= collisions.UpdateSpline;
  //  }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("trigger"))
        {
            collisions.sPData.MeshModifier.DrawMesh();
        }
        // base.OnInspectorGUI();
    }

}
