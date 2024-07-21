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
            myScript.SetNewInitialConfiguration();
        }

        //if (GUILayout.Button("[Test]Count Live Neighbor"))
        //{
        //    myScript.CountLiveNeighbors(Random.Range(0, 500), Random.Range(0, 500));
        //}

        //if (GUILayout.Button("[Test]Print Pixel Matrix"))
        //{
        //    myScript.PrintPixelMatrix();
        //}

        //if (GUILayout.Button("Start Simulation"))
        //{
        //    myScript.StartSimulation();
        //}

        //if (GUILayout.Button("Pause Simulation"))
        //{
        //    myScript.PauseSimulation();
        //}

        if (GUILayout.Button("Reset Simulation"))
        {
            myScript.ResetSimulation();
        }
    }
}
