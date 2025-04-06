using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapGenerator mapGenerator = (MapGenerator)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.GenerateMap();
        }
        if (GUILayout.Button("Clear Map"))
        {
            mapGenerator.ClearMap();
        }
    }
}
