using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointEditor 
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        if((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = new Color(1, 1, 1, .5f);
        }

        Gizmos.DrawSphere(waypoint.transform.position, .25f);

        if (waypoint.NextWaypoint != null)
        {
            Gizmos.DrawLine(waypoint.transform.position, waypoint.NextWaypoint.transform.position);
            if (waypoint.Width > 0)
            {
                Vector3 offset = waypoint.transform.right * waypoint.Width / 2;
                Vector3 offsetTo = waypoint.NextWaypoint.transform.right * waypoint.NextWaypoint.Width / 2;
                Gizmos.color = Color.white;
                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.NextWaypoint.transform.position + offsetTo);
            }
        }
        if (waypoint.PreviousWaypoint != null)
        {
            if((gizmoType & GizmoType.Selected) != 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(waypoint.transform.position, waypoint.PreviousWaypoint.transform.position);
            }
            if (waypoint.Width > 0)
            {
                Vector3 offset = waypoint.transform.right * -waypoint.Width / 2;
                Vector3 offsetTo = waypoint.PreviousWaypoint.transform.right * -waypoint.PreviousWaypoint.Width / 2;
                Gizmos.color = Color.white;
                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.PreviousWaypoint.transform.position + offsetTo);
            }
        }

        

    }
}
