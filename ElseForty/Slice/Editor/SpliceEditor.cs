using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ElseForty;


[CustomEditor(typeof(Slice))]
public class SpliceEditor : Editor
{
    Slice slice;

    private void Awake()
    {
        slice = (Slice)target;
        slice.sPData = slice.gameObject.GetComponent<SplinePlus>().sPData;
    }

 
    // Start is called before the first frame update
    private void OnEnable()
    {
        SplineCreationClass.Update_Spline += slice.UpdateSpline;
    }
   
    private void OnDisable()
    {
        SplineCreationClass.Update_Spline -= slice.UpdateSpline;
    }
 

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();
        var range = EditorGUILayout.Slider("Range",slice.range, 0, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(slice, "Valuechanged");
            Undo.RecordObject(slice.sPData.SplinePlus, "Valuechanged");

            slice.range = range;
            slice.UpdateSpline();
            FocusSceneView();
        }
    }

    void FocusSceneView()
    {
        if (SceneView.sceneViews.Count > 0)
        {
            SceneView sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.Focus();
        }
    }
}
