using UnityEngine;
using UnityEditor;

public class WaypointPlacementTool : EditorWindow
{
    private GameObject pathContainer;
    private bool isPlacing = false;

    [MenuItem("Tools/Waypoint Placement Tool")]
    public static void ShowWindow()
    {
        GetWindow<WaypointPlacementTool>("Waypoint Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Waypoint Placement Tool", EditorStyles.boldLabel);
        
        pathContainer = (GameObject)EditorGUILayout.ObjectField("Path Container", pathContainer, typeof(GameObject), true);
        
        if (pathContainer == null)
        {
            EditorGUILayout.HelpBox("Please assign the 'Path' GameObject.", MessageType.Warning);
            if (GUILayout.Button("Find 'Path' in Scene"))
            {
                pathContainer = GameObject.Find("Path");
            }
        }

        EditorGUILayout.Space();

        GUI.backgroundColor = isPlacing ? Color.green : Color.white;
        if (GUILayout.Button(isPlacing ? "Placement Mode: ON" : "Placement Mode: OFF"))
        {
            isPlacing = !isPlacing;
            if (isPlacing)
            {
                SceneView.duringSceneGui += OnSceneGUI;
                EnsurePipelineHasCollider();
            }
            else
            {
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox("While ON, Shift+Click in Scene View to place a waypoint on the pipeline.", MessageType.Info);
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacing || pathContainer == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CreateWaypoint(hit.point);
                e.Use();
            }
        }
    }

    private void CreateWaypoint(Vector3 position)
    {
        GameObject wp = new GameObject("Waypoint_" + pathContainer.transform.childCount);
        wp.transform.SetParent(pathContainer.transform);
        wp.transform.position = position;
        
        // Add a visual if needed (optional, but helpful)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "Visual";
        visual.transform.SetParent(wp.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * 0.05f;
        visual.GetComponent<Renderer>().sharedMaterial.color = Color.yellow;
        
        Undo.RegisterCreatedObjectUndo(wp, "Create Waypoint");
        Selection.activeGameObject = wp;
        Debug.Log("Placed " + wp.name + " at " + position);
    }

    private void EnsurePipelineHasCollider()
    {
        GameObject pipeline = GameObject.Find("Pipeline");
        if (pipeline != null && pipeline.GetComponent<Collider>() == null)
        {
            pipeline.AddComponent<MeshCollider>();
            Debug.Log("Added MeshCollider to Pipeline for waypoint placement.");
        }
    }
}
