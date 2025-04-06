using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tunnel))]
public class TunnelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Tunnel tunnel = (Tunnel)target;
        bool open = tunnel.IsOpen;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tunnel State", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(open ? "Open" : "Closed");
        EditorGUILayout.Space();

        if (GUILayout.Button("Toggle Tunnel"))
        {
            open = !open;
            tunnel.SetOpen(open);
        }
    }
}
