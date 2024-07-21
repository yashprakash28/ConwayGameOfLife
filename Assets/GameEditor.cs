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

        if (GUILayout.Button("Start Simulation"))
        {
            myScript.StartSimulation();
        }

        if (GUILayout.Button("Pause Simulation"))
        {
            myScript.PauseSimulation();
        }

        if (GUILayout.Button("Reset Simulation"))
        {
            myScript.ResetSimulation();
        }

        if (GUILayout.Button("[Test]Count Live Neighbor"))
        {
            //myScript.CountLiveNeighbors(Random.Range(0, myScript.maxRows), Random.Range(0, myScript.maxColumns));
            int middleRow = myScript.maxRows / 2;
            int middleCol = myScript.maxColumns / 2;

            myScript.CountLiveNeighbors(middleRow, middleCol);
            myScript.CountLiveNeighbors(middleRow - 1, middleCol - 1);
            myScript.CountLiveNeighbors(middleRow - 1, middleCol);
            myScript.CountLiveNeighbors(middleRow - 1, middleCol + 1);
            myScript.CountLiveNeighbors(middleRow, middleCol - 1);
            myScript.CountLiveNeighbors(middleRow, middleCol + 1);
            myScript.CountLiveNeighbors(middleRow + 1, middleCol - 1);
            myScript.CountLiveNeighbors(middleRow + 1, middleCol);
            myScript.CountLiveNeighbors(middleRow + 1, middleCol + 1);
        }

        if (GUILayout.Button("[Test]Run Simulation"))
        {
            myScript.RunSimulation();
        }
    }
}
