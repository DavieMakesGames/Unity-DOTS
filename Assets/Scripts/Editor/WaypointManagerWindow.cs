using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform WaypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("WaypointRoot"));

        if (WaypointRoot == null) EditorGUILayout.HelpBox("WaypointRoot is null in WaypointManagerWindow.cs", MessageType.Warning);
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }
        obj.ApplyModifiedProperties();
    }
    void DrawButtons()
    {        
        if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
        else
        {
            if (GUILayout.Button("Create Waypoint"))
            {
                CreateWaypoint();
            }
        }
    }
    void CreateWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);

        // Create new waypoint at the position of the last selected waypoint.
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        if(WaypointRoot.childCount > 1)
        {
            newWaypoint.PreviousWaypoint = WaypointRoot.GetChild(WaypointRoot.childCount - 2).GetComponent<Waypoint>();
            newWaypoint.PreviousWaypoint.NextWaypoint = newWaypoint;
            newWaypoint.transform.position = newWaypoint.PreviousWaypoint.transform.position;
            newWaypoint.transform.forward = newWaypoint.PreviousWaypoint.transform.forward;
        }

        Selection.activeGameObject = newWaypoint.gameObject;
    }
    void CreateWaypointBefore()
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if(selectedWaypoint.PreviousWaypoint != null)
        {
            newWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
            selectedWaypoint.PreviousWaypoint.NextWaypoint = newWaypoint;
        }

        newWaypoint.NextWaypoint = selectedWaypoint;
        selectedWaypoint.PreviousWaypoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeGameObject = newWaypoint.gameObject;
    }
    void CreateWaypointAfter()
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        newWaypoint.PreviousWaypoint = selectedWaypoint;

        if (selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = newWaypoint;
            newWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
        }

        selectedWaypoint.NextWaypoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeGameObject = newWaypoint.gameObject;
    }
    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if(selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
        }
        if(selectedWaypoint.PreviousWaypoint != null)
        {
            selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
            Selection.activeGameObject = selectedWaypoint.PreviousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }
}
