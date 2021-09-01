using UnityEngine;
using UnityEditor;
using System;
using ElseForty;


public class ToolsBareEditor
{
    public static void Show(SPData sPData)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

        var c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit", "ToolbarButton"))
        {

            GenericMenu menu = new GenericMenu();
 
            menu.AddItem(new GUIContent("Settings"), false, Settings, sPData);
            menu.AddItem(new GUIContent("Snap To Grid"), false, SnapToGrid, sPData);


            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Modifiers", "ToolbarButton"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Animation"));
            menu.AddItem(new GUIContent("Simple Followers"), false, AddModifier, new object[] { sPData, "SimpleFollowersClass" });
            menu.AddDisabledItem(new GUIContent("Mesh"));
            menu.AddItem(new GUIContent("Plane Mesh"), false, AddModifier, new object[] { sPData, "PlaneMesh" });
            menu.AddItem(new GUIContent("Tube Mesh"), false, AddModifier, new object[] { sPData, "TubeMesh" });
            menu.AddItem(new GUIContent("Slice"), false, AddModifier, new object[] { sPData, "Slice" });


            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Help", "ToolbarButton" ))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Elseforty.com"), false, Website);
            menu.AddItem(new GUIContent("Documentation"), false, Documentation);


            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
    }

    #region Items menu methods

    static void SnapToGrid(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Spline snaped");
        SplinePlusEditorAPI.Snap(sPData);
    }
 
    static void Settings(object obj)
    {
        SPData sPData = (SPData)obj;
        SettingsWindow settingsWindow = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow), true, "Settings", true);
        settingsWindow.Show(sPData);
    }

    static void AddModifier(object objs)
    {
        var o = (object[])objs;
        var modifierName = (string)o[1];
        var sPData = (SPData)o[0];


        var myType = System.Type.GetType("ElseForty." + modifierName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        //sPData.meshModifier =(IMeshModifier) myType;

     //   var myType = Type.GetType(modifierName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        if (myType == null)
        {
            EditorUtility.DisplayDialog("Error", modifierName + " modifier not found!", "Okey");
        }
        else
        {
            sPData.SplinePlus.gameObject.AddComponent(myType);
        }

        SplineCreationClass.Update(sPData);

    }

    static void Documentation()
    {
        Application.OpenURL("https://elseforty.github.io/unity/");
    }

    static void Website()
    {
        Application.OpenURL("https://elseforty.com");
    }
    #endregion
}
