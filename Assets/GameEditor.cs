using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Game))]
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Game myScript = (Game)target;

        if (GUILayout.Button("New Initial Configuration"))
        {
            myScript.SetNewConfiguration();
        }
    }
}
