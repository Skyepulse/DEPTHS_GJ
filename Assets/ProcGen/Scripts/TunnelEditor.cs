using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tunnel))]
public class TunnelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Tunnel tunnel = (Tunnel)target;
        Tunnel.State state = tunnel.curState;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tunnel State", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current State: " + state.ToString());
        EditorGUILayout.Space();

        if (GUILayout.Button("Toggle Tunnel"))
        {
            state = (Tunnel.State)(((int)state + 1) % 3);
            tunnel.SetState(state);
        }
    }
}
