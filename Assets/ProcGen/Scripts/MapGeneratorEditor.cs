using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
#endif
#if UNITY_EDITOR
public class MapGeneratorEditor : Editor
#else
public class MapGeneratorEditor
#endif
{
    #if UNITY_EDITOR
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
    #endif
}
