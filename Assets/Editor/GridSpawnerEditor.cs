using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSpawner))]
public class GridSpawnerEditor : Editor
{
    Vector3 spawnPosition = Vector3.zero;
    Vector3Int sizes = new Vector3Int(4, 4, 4);
    string prefabName = string.Empty;

    public override void OnInspectorGUI()
    {
        spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
        sizes = EditorGUILayout.Vector3IntField("Grid Sizes", sizes);
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);

        if (GUILayout.Button("Construct Board"))
        {
            var prefab = GetPrefabFromKeyword(prefabName);
            var test = new GameObject("Board Test");
            test.transform.position = spawnPosition;
            GridSpawner.ConstructPlanes(sizes, test, prefab);
        }
        if (GUILayout.Button("Construct Plane"))
        {
            var prefab = GetPrefabFromKeyword(prefabName);
            var test = new GameObject("Plane Test");
            test.transform.position = spawnPosition;
            GridSpawner.ConstructPlane(new Vector2Int(sizes.x, sizes.y), test, prefab);
        }
        if (GUILayout.Button("Construct Column"))
        {
            var prefab = GetPrefabFromKeyword(prefabName);
            var test = new GameObject("Column Test");
            test.transform.position = spawnPosition;
            GridSpawner.ConstructColumn(sizes.y, test, prefab);
        }
    }

    private GameObject GetPrefabFromKeyword(string keyword)
    {
        var prefabGUID = AssetDatabase.FindAssets(keyword);
        var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID[0]);
        return (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
    }
}
