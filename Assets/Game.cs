using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject _pixelPrefab;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Material _pixelBlack;
    [SerializeField] private Material _pixelWhite;
    float windowHeight;
    float windowWidth;
    float pixelSize;
    float pixelSizeOffset;
    float screenOffset;
    public int maxColumns;
    public int maxRows;
    float gridWidth;
    float gridHeight;
    int midx;
    int midy;
    Vector3 topLeft;

    public List<GameObject> pixelMatrix = new List<GameObject>();
    public List<Material> newPixelMatrix = new List<Material>();

    public int[,] gridConfigurations; // 2D array to store configurations
    int totalConfigurations = 0;

    bool isRunSimulation;

    List<int> directionOffset = new List<int>();

    public float delay = 2.0f; // Delay time in seconds
    private float timer = 0.0f;

    void Start()
    {
        isRunSimulation = false;

        InitSettings();
        InitScreen();
        ComputeAllInitialGridConfigurations();
        ResetSimulation();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay && isRunSimulation)
        {
            RunSimulation();
            timer = 0.0f;
        }
    }

    private void InitSettings()
    {
        string configurationDebugStr = "";
        configurationDebugStr += "OS (" + _mainCamera.orthographicSize + ") ; ";
        configurationDebugStr += "aspct (" + _mainCamera.aspect + ") ; ";

        windowHeight = 2 * _mainCamera.orthographicSize;
        windowWidth = windowHeight * _mainCamera.aspect;
        configurationDebugStr += "HxW (" + windowHeight + " x " + windowWidth + ") ; ";

        pixelSize = _pixelPrefab.transform.localScale.x;
        pixelSizeOffset = pixelSize / 2;
        maxColumns = Mathf.FloorToInt(windowWidth / pixelSize);
        maxRows = Mathf.FloorToInt(windowHeight / pixelSize);
        configurationDebugStr += "RxC (" + maxRows + " x " + maxColumns + ") ; ";
        configurationDebugStr += "TotalPixels (" + maxColumns * maxRows + ")";

        gridWidth = maxColumns * pixelSize;
        gridHeight = maxRows * pixelSize;

        Debug.Log(configurationDebugStr);

        directionOffset.Add(-maxColumns - 1);
        directionOffset.Add(-maxColumns);
        directionOffset.Add(-maxColumns + 1);
        directionOffset.Add(-1);
        directionOffset.Add(0);
        directionOffset.Add(1);
        directionOffset.Add(maxColumns - 1);
        directionOffset.Add(maxColumns);
        directionOffset.Add(maxColumns + 1);
    }

    private void InitScreen()
    {
        gameObject.transform.localScale = new Vector3(gridWidth, gridHeight, 0.1f);
        topLeft = new Vector3(-gridWidth / 2, gridHeight / 2, gameObject.transform.position.z - 0.5f);
        InstantiatePixels();
    }

    private void InstantiatePixels()
    {
        int pixelCount = 0;
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                Vector3 targetPosition = new Vector3(
                                                topLeft.x + pixelSizeOffset + (j * pixelSize), 
                                                topLeft.y - pixelSizeOffset - (i * pixelSize), 
                                                topLeft.z
                                            );
                GameObject objectPixel = Instantiate(_pixelPrefab, targetPosition, Quaternion.identity);
                objectPixel.transform.SetParent(this.gameObject.transform);
                objectPixel.GetComponent<Renderer>().material = _pixelBlack;

                pixelMatrix.Add(objectPixel);
                pixelCount++;
            }
        }

        midx = maxRows / 2;
        midy = maxColumns / 2;
    }

    private void ComputeAllInitialGridConfigurations()
    {
        List<int[,]> configurations = GenerateConfigurations();
        totalConfigurations = configurations.Count;
        gridConfigurations = new int[configurations.Count, 9];

        for (int i = 0; i < configurations.Count; i++)
        {
            int[,] grid = configurations[i];
            for (int j = 0; j < 9; j++)
            {
                gridConfigurations[i, j] = grid[j / 3, j % 3];
            }
        }
    }

    List<int[,]> GenerateConfigurations()
    {
        List<int[,]> configurations = new List<int[,]>();

        // Total number of combinations (2^9)
        int totalCombinations = 1 << 9;

        for (int i = 1; i < totalCombinations; ++i) // Start from 1 to skip the all-black configuration
        {
            int[,] grid = new int[3, 3];

            for (int j = 0; j < 9; ++j)
            {
                grid[j / 3, j % 3] = (i >> j) & 1;
            }

            configurations.Add(grid);
        }

        return configurations;
    }

    public void ResetSimulation()
    {
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                int idx = i * maxColumns + j;
                pixelMatrix[idx].GetComponent<Renderer>().material = _pixelBlack;
            }
        }
    }

    public void SetNewInitialConfiguration()
    {
        //int randomIndex = Random.Range(0, totalConfigurations + 1);
        int randomIndex = Random.Range(0, 500);
        List<List<int>> targetConfiguration = new List<List<int>>();

        for (int row = 0; row < 3; row++)
        {
            List<int> tmp = new List<int>();
            for (int col = 0; col < 3; col++)
            {
                tmp.Add(gridConfigurations[randomIndex, row * 3 + col]);
            }
            targetConfiguration.Add(tmp);
        }

        // can be used to compare output with the one on wikipedia page of Conway's game of life
        //ValidateSimulationLogic();

        string finalstr = randomIndex + " : ";
        int dirIndex = 0;
        for (int i = 0; i < 3; i++)
        {
            string rowstr = "";
            for (int j = 0; j < 3; j++)
            {
                int targetPixelIndex = i * maxColumns + j;
                int mid = midx * maxColumns + midy;

                if (targetConfiguration[i][j] == 1)
                {
                    pixelMatrix[mid + directionOffset[dirIndex]].GetComponent<Renderer>().material = _pixelWhite;
                }
                else
                {
                    pixelMatrix[mid + directionOffset[dirIndex]].GetComponent<Renderer>().material = _pixelBlack;
                }
                dirIndex++;
                rowstr += targetConfiguration[i][j] + " ";
            }
            finalstr += rowstr + "  ";
        }
        Debug.Log(finalstr);
    }

    public void StartSimulation()
    {
        isRunSimulation = true;
    }

    public void PauseSimulation()
    {
        isRunSimulation = false;
    }

    public void RunSimulation()
    {
        /*
         * Any live cell with fewer than two live neighbours dies, as if by underpopulation.
         * Any live cell with two or three live neighbours lives on to the next generation.
         * Any live cell with more than three live neighbours dies, as if by overpopulation.
         * Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
         */

        newPixelMatrix.Clear();

        int liveCellCnt = 0;
        int deadCellCnt = 0;

        // iterate to calculate next generation
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                int liveNeighborCount = CountLiveNeighbors(i, j);
                int idx = i * maxColumns + j;

                if (pixelMatrix[idx].GetComponent<Renderer>().material.name == _pixelWhite.name + " (Instance)")
                {
                    liveCellCnt++;
                    // live cell - 3 conditions
                    if (liveNeighborCount < 2 || liveNeighborCount > 3)
                    {
                        // underpopulation and overpopulation
                        newPixelMatrix.Add(_pixelBlack);
                    }
                    else if (liveNeighborCount == 2 || liveNeighborCount == 3)
                    {
                        // next generation
                        newPixelMatrix.Add(_pixelWhite);
                    }
                }
                else
                {
                    deadCellCnt++;
                    // dead cell
                    if (liveNeighborCount == 3)
                    {
                        // reproduction
                        newPixelMatrix.Add(_pixelWhite);
                    }
                    else
                    {
                        newPixelMatrix.Add(_pixelBlack);
                    }
                }
            }
        }

        Debug.Log(liveCellCnt + ", " + deadCellCnt);

        // iterate to swap next generation
        for(int i=0; i<maxRows; i++)
        {
            for(int j=0; j<maxColumns; j++)
            {
                int idx = i * maxColumns + j;
                pixelMatrix[idx].GetComponent<Renderer>().material = newPixelMatrix[idx];
            }
        }
    }

    public int CountLiveNeighbors(int x, int y)
    {
        int count = 0;
        int idx = x * maxColumns + y;
        
        for (int i=0; i<9; i++)
        {
            int targetIndex = idx + directionOffset[i];
            if (targetIndex >= 0 && targetIndex < pixelMatrix.Count && i != 4)
            {
                if (pixelMatrix[targetIndex] != null)
                {
                    Renderer renderer = pixelMatrix[targetIndex].GetComponent<Renderer>();
                    if (renderer.material.name == _pixelWhite.name + " (Instance)")
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    //private void ValidateSimulationLogic()
    //{
    //    // Add first configuration
    //    List<int> tmp = new List<int> { 0, 1, 0 };
    //    targetConfiguration.Add(tmp);
    //    Debug.Log(targetConfiguration[0][0]);
    //    Debug.Log(targetConfiguration[0][1]);
    //    Debug.Log(targetConfiguration[0][2]);

    //    // Add second configuration
    //    tmp = new List<int> { 1, 1, 0 };
    //    targetConfiguration.Add(tmp);
    //    Debug.Log(targetConfiguration[1][0]);
    //    Debug.Log(targetConfiguration[1][1]);
    //    Debug.Log(targetConfiguration[1][2]);

    //    // Add third configuration
    //    tmp = new List<int> { 0, 1, 1 };
    //    targetConfiguration.Add(tmp);
    //    Debug.Log(targetConfiguration[2][0]);
    //    Debug.Log(targetConfiguration[2][1]);
    //    Debug.Log(targetConfiguration[2][2]);

    //    // Format and log the entire configuration
    //    string fstr = "";
    //    for (int i = 0; i < targetConfiguration.Count; i++)
    //    {
    //        string rstr = "";
    //        for (int j = 0; j < targetConfiguration[i].Count; j++)
    //        {
    //            rstr += targetConfiguration[i][j] + " ";
    //        }
    //        fstr += rstr.TrimEnd() + " ";
    //    }
    //}

}