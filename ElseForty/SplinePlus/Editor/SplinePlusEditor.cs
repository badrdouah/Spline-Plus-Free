using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using ElseForty;


[CustomEditor(typeof(SplinePlus))]
public class SplinePlusEditor : Editor
{
    public SPData SPData;

    public static GUIContent Banner;
    public static GUIContent Plus;
    public static GUIContent Minus;
    public static GUIContent Delete;
    public static GUIContent Return;
    public static GUIContent Ok;

    public int ToolBarSelection = 0;
    EditorApplication.CallbackFunction value;

    [MenuItem("Tools/Spline plus", false, 0)]
    static void CreateSplinePlus()
    {
        var sPData = SplinePlusAPI.SplinePlus_Create(Vector3.zero);
        Selection.activeGameObject = sPData.SplinePlus.gameObject;
    }

    public void OnEnable()
    {
        var SplinePlus = (SplinePlus)target;
        SPData = SplinePlus.sPData;

        if (Banner == null) Banner = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Banner.png")));
        if (Plus == null) Plus = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Plus.png")));
        if (Minus == null) Minus = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Minus.png")));
        if (Delete == null) Delete = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Delete.png")));
        if (Return == null) Return = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Return.png")));
        if (Ok == null) Ok = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Ok.png")));


 

 
        System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
    value = (EditorApplication.CallbackFunction) info.GetValue(null);
    value += Shortcuts;
        info.SetValue(null,value);

        EditorUtility.SetDirty(SPData.SplinePlus);
    }

public void OnDisable()
{
    System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler",
    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
    value -= Shortcuts;
    info.SetValue(null, value);
}

public static string FindAssetPath(string assetName)
    {
        string[] res = System.IO.Directory.GetFiles(Application.dataPath, assetName, SearchOption.AllDirectories);
        if (res.Length == 0)
        {
            Debug.LogError("Asset " + assetName + " not found!!");
            return null;
        }

        var path = Regex.Split(res[0], "Assets", RegexOptions.None);
        return ("Assets" + path[1]);
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        var rect = EditorGUILayout.BeginHorizontal();
        var x = rect.x + (rect.width - Banner.image.width) * 0.5f;
        GUI.Label(new Rect(x, rect.y, Banner.image.width, Banner.image.height), Banner);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(Banner.image.height);
        ToolsBareEditor.Show(SPData);

        //force update when undo redo or transform changed
        if (Event.current.commandName == "UndoRedoPerformed" || SPData.SplinePlus.gameObject.transform.hasChanged) //
        {
            if (SPData.SplinePlus.gameObject.transform.hasChanged) SPData.SplinePlus.gameObject.transform.hasChanged = false;
            SPData.Update();
        }

        EditorGUILayout.Space();
        DebugArea();

        Node();
        EditorGUILayout.Space();
        Spline();
    }

    protected virtual void OnSceneGUI()
    {
        Selection.activeGameObject = SPData.SplinePlus.gameObject;

        if (SPData.Projection.ContinuosUpdate == Switch.On) SplineCreationClass.ProjectSpline(SPData);

        SceneViewDisplay.Display(SPData);
        Shortcuts();
  
}

void FocusSceneView()
{
    if (SceneView.sceneViews.Count > 0)
    {
        SceneView sceneView = (SceneView)SceneView.sceneViews[0];
        sceneView.Focus();
    }
}

    void DebugArea()
    {
        GUI.color = Color.green;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

   
        GUI.color = Color.white;

        var style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.green;

        var mode = "";
        if (SPData.User_Action == User_Actions.None) mode = "Selection";
        else if (SPData.User_Action == User_Actions.CoordinatesEdit) mode = "Node Coordinates Edit ('esc' To exit) ";
        else if (SPData.User_Action == User_Actions.NormalEdit) mode = "Node Normal Edit ('esc' To exit)";
       // else if (SPData.User_Action == User_Actions.SpeedEdit) mode = "Node Speed Edit ('esc' To exit)";
        else if (SPData.User_Action == User_Actions.Add) mode = "Node Adding ('esc' To exit)";

        var selectedNode = SPData.Node_Selected();
        if (selectedNode != null)
        {
 
            EditorGUILayout.LabelField("Mode: " + mode, style, GUILayout.ExpandWidth(true), GUILayout.Height(9));
            EditorGUILayout.LabelField("Node: " + SPData._NodeIndex.ToString(), style, GUILayout.ExpandWidth(true), GUILayout.Height(9));
        }
        else
        {
            EditorGUILayout.LabelField("Mode: " + mode, style, GUILayout.ExpandWidth(true), GUILayout.Height(9));
            EditorGUILayout.LabelField("Node: NULL", style, GUILayout.ExpandWidth(true), GUILayout.Height(9));
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void Node()
{
    var selectedNode = SPData.Node_Selected();
    var centerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 18 };

    if (selectedNode == null)
    {
        EditorGUILayout.LabelField("No Node Selected!", centerStyle, GUILayout.ExpandWidth(true));
        return;
    }
    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

    EditorGUILayout.LabelField("Node", centerStyle, GUILayout.ExpandWidth(true));
    centerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
    EditorGUILayout.Space();

    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Add"))
    {
        SceneViewDisplay.Node_Add(SPData);
        FocusSceneView();
    }

    if (GUILayout.Button("Remove"))
    {
        SceneViewDisplay.Node_Delete(SPData);
        FocusSceneView();
    }
    EditorGUILayout.EndHorizontal();
 
    EditorGUILayout.LabelField("Pop Ups:", centerStyle, GUILayout.ExpandWidth(true));

    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Normal"))
    {
        SceneViewDisplay.Node_Normal(SPData);
        FocusSceneView();
    }
 
    if (GUILayout.Button("Coordinates"))
    {
        SceneViewDisplay.Node_Coordinates(SPData);
        FocusSceneView();
    }
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.LabelField("Shape:", centerStyle, GUILayout.ExpandWidth(true));

    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Flip Handles"))
    {
        SceneViewDisplay.Node_FlipHandles(SPData);
        FocusSceneView();
    }

    if (GUILayout.Button("Hide/Unhide Handles"))
    {
        SceneViewDisplay.Node_Hide_Unhide_Handles(SPData);
        FocusSceneView();
    }
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    EditorGUI.BeginChangeCheck();
    var newNodeType = EditorGUILayout.EnumPopup("Node Type", selectedNode._Type);

    if (EditorGUI.EndChangeCheck())
    {
        selectedNode._Type = (NodeType)newNodeType;
        switch (newNodeType)
        {

            case NodeType.Smooth:
                SceneViewDisplay.Node_Type_Smooth(SPData);
                break;
            case NodeType.Free:
                SceneViewDisplay.Node_Type_Free(SPData);
                break;
            case NodeType.Broken:
                SceneViewDisplay.Node_Type_Broken(SPData);
                break;
        }

        FocusSceneView();
    }

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.EndVertical();
}

void Spline()
{
 
    var centerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 18 };
 

    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    EditorGUILayout.LabelField("Spline", centerStyle, GUILayout.ExpandWidth(true));
    centerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
    EditorGUILayout.Space();

    EditorGUILayout.BeginHorizontal();
 

    if (GUILayout.Button("Clear"))
    {
        SceneViewDisplay.Spline_Clear(SPData);
        FocusSceneView();
    }
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.LabelField("Actions:", centerStyle, GUILayout.ExpandWidth(true));

    EditorGUILayout.BeginHorizontal();
 
    if (GUILayout.Button("Reverse"))
    {
        SceneViewDisplay.Spline_Reverse(SPData);
        FocusSceneView();
    }


    if (SPData.Close)
    {
        if (GUILayout.Button("Open"))
        {

            SceneViewDisplay.Spline_Open(SPData);
            FocusSceneView();
        }
    }
    else
    {
        if (GUILayout.Button("Close"))
        {

            SceneViewDisplay.Spline_Close(SPData);
            FocusSceneView();
        }
    }

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.EndVertical();
}

void Shortcuts()
    {
        var e = Event.current;
        if (e == null) return;

        // Set mode back to selection
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            Undo.RecordObject(SPData.SplinePlus, "Selection initialised");
            SPData.User_Action = User_Actions.None;
            FocusSceneView();
        }
 
        //focus on path point
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C)
        {
            var selectedNode = SPData.Node_Selected();
            SceneView.lastActiveSceneView.LookAt(selectedNode.Point.position);
            FocusSceneView();
        }

        //Delete selected node
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace)
        {
            var selectedNode = SPData.Node_Selected();
            Undo.RecordObject(SPData.SplinePlus, "node deleted");
            SplinePlusAPI.Node_Remove(SPData, selectedNode);
            FocusSceneView();
        }

        //Hide node handles
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.H)
        {
            Undo.RecordObject(SPData.SplinePlus, "node hide unhide handles");
            SPData.SplineSettings.Show_SecondaryHandles = !SPData.SplineSettings.Show_SecondaryHandles;
            FocusSceneView();
        }

        //Reverse sPData
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
        {
            Undo.RecordObject(SPData.SplinePlus, "Reverse sPData");
            SplinePlusAPI.Spline_Reverse(SPData);
            SPData.Update( );
            FocusSceneView();
        }

        //Flip node handles
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.X)
        {
            Undo.RecordObject(SPData.SplinePlus, "Flip handles sPData");
            SplinePlusAPI.Node_FlipHandles(SPData,   SPData._NodeIndex);
            SPData.Update();
            FocusSceneView();
        }
    }
}


