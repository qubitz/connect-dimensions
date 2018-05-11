using System;
using UnityEngine;
using UnityEditor;

public class GridSpawner : MonoBehaviour
{
    public static GameObject[][][] ConstructPlanes(Vector3Int count, GameObject parent, GameObject original)
    {
        var planes = new GameObject[count.z][][];
        var planePos = new Vector3(0, 0, -(count.z - 1) / 2.0f);

        for (int i = 0; i < count.z; i++)
        {
            var plane = new GameObject("Plane");
            planes[i] = ConstructPlane(new Vector2Int(count.x, count.y), plane, original);
            var planess = parent.GetComponentsInChildren<Transform>();
            plane.name += " " + i;
            plane.transform.localPosition = planePos;
            plane.SetParentRelative(parent);
            planess = parent.GetComponentsInChildren<Transform>();
            planePos.z++;
        }

        return planes;
    }

    public static GameObject[][] ConstructPlane(Vector2Int count, GameObject parent, GameObject original)
    {
        var columns = new GameObject[count.x][];
        var colPos = new Vector3(-(count.x - 1) / 2.0f, 0, 0);

        for (int i = 0; i < count.x; i++)
        {
            var column = new GameObject("Column");
            columns[i] = ConstructColumn(count.y, column, original);
            column.name += " " + i;
            column.transform.localPosition = colPos;
            column.SetParentRelative(parent);
            colPos.x++;
        }

        return columns;
    }

    public static GameObject[] ConstructColumn(int count, GameObject parent, GameObject original)
    {
        var zones = new GameObject[count];
        var zonePos = new Vector3(0, -(count - 1) / 2.0f, 0);

        for (int i = 0; i < count; i++)
        {
            var zone = Instantiate(original, parent.transform);
            zone.name = "Token Zone " + i;  // add identifier
            zone.transform.localPosition = zonePos;  // place relative
            zones[i] = zone;  // assign to array
            zonePos.y++;  // move up
        }

        return zones;
    }
}
