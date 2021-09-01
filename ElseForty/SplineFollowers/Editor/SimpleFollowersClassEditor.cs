﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ElseForty;

[CustomEditor(typeof(SimpleFollowersClass))]
public class SimpleFollowersClassEditor : Editor
{
    public ReorderableList FollowersList;
    SimpleFollowersClass SimpleFollowersClass;

    private void OnEnable()
    {
        SimpleFollowersClass = (SimpleFollowersClass)target;
        SimpleFollowersClass.SPData = SimpleFollowersClass.gameObject.GetComponent<SplinePlus>().sPData;

        SplineCreationClass.Update_Spline += Update_Spline;
        SplinePlusAPI.Branch_Deleted += Branch_Deleted;
    }
    private void OnDisable()
    {
        SplineCreationClass.Update_Spline -= Update_Spline;
        SplinePlusAPI.Branch_Deleted -= Branch_Deleted;
    }

    void Update_Spline()
    {
        SimpleFollowerAnim.Follow(SimpleFollowersClass.SPData, SimpleFollowersClass.Followers);
    }

    void Branch_Deleted(int branchKey)
    {
        SimpleFollowerAnim.Follow(SimpleFollowersClass.SPData, SimpleFollowersClass.Followers);
    }

    public override void OnInspectorGUI()
    {
        if (FollowersList == null) Init();
        FollowersList.DoList(EditorGUILayout.GetControlRect());
        GUILayout.Space(FollowersList.GetHeight());
    }

    public void Init()
    {
        SerializedProperty FollowersSP = serializedObject.FindProperty("Followers");
        FollowersList = new ReorderableList(SimpleFollowersClass.Followers, typeof(Follower), true, false, true, true);

        FollowersList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty follower = FollowersSP.GetArrayElementAtIndex(index);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 40, rect.height),
               follower.FindPropertyRelative("Animation"), new GUIContent(""));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SimpleFollowerAnim.Follow(SimpleFollowersClass.SPData, SimpleFollowersClass.Followers);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(new Rect(rect.x + 50, rect.y, rect.width - 130, rect.height),
               follower.FindPropertyRelative("FollowerGO"), typeof(GameObject), new GUIContent(""));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SimpleFollowerAnim.Follow(SimpleFollowersClass.SPData, SimpleFollowersClass.Followers);
            }

            if (GUI.Button(new Rect(rect.x + rect.width - 70, rect.y, 70, rect.height), "Settings"))
            {
                FollowerWindow wind = (FollowerWindow)EditorWindow.GetWindow(typeof(FollowerWindow), true, "Simple Follower Settings", true);
                wind.Show(SimpleFollowersClass.SPData, follower, SimpleFollowersClass.Followers[index]);
            }
        };
        FollowersList.onAddCallback = (ReorderableList list) =>
        {
            var t = FollowersSP.arraySize;
            FollowersSP.InsertArrayElementAtIndex(t);

            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("FollowerGO").objectReferenceValue = null;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Animation").enumValueIndex = (int)Switch.On;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("FlipDirection").boolValue = false;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("IsForward").boolValue = true;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Distance").floatValue = 0;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Speed").floatValue = 2.5f;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("TimeToReachFullSpeed").floatValue = 0;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("_FollowerAnimation").enumValueIndex = (int)FollowerAnimation.Auto;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("LockRotation").enumValueIndex = (int)Switch.Off;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Position").vector3Value = Vector3.zero;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Rotation").vector3Value = Vector3.zero;


            serializedObject.ApplyModifiedProperties();
        };

        FollowersList.onRemoveCallback = (ReorderableList list) =>
        {
            var cachedIndex = FollowersList.index;
            FollowersSP.DeleteArrayElementAtIndex(cachedIndex);
            FollowersList.index = cachedIndex - 1;
            serializedObject.ApplyModifiedProperties();

        };
    }
}
