using System.Collections;
using System.Collections.Generic;
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
    int maxColumns;
    int maxRows;
    float gridWidth;
    float gridHeight;
    int midx;
    int midy;
    Vector3 bottomLeft;


    [System.Serializable]
    public struct Pixel
    {
        public GameObject objectReference;
        public Color colorVal;
    }

    public List<List<Pixel>> pixelMatrix = new List<List<Pixel>>();

    public int[,] gridConfigurations; // 2D array to store configurations
    int totalConfigurations = 0;

    bool isRunSimulation;

    void Start()
    {
        isRunSimulation = false;

        InitSettings();
        InitScreen();
        ComputeAllInitialGridConfigurations();
        ResetSimulation();
    }

    //void Update()
    //{
    //    if (isRunSimulation)
    //    {
    //        RunSimulation();
    //    }
    //}

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

        gridWidth = maxColumns * pixelSize;
        gridHeight = maxRows * pixelSize;

        Debug.Log(configurationDebugStr);
    }

    private void InitScreen()
    {
        gameObject.transform.localScale = new Vector3(gridWidth, gridHeight, 0.1f);
        bottomLeft = new Vector3(-gridWidth / 2, -gridHeight / 2, gameObject.transform.position.z - 0.5f);
        InstantiatePixels();
    }

    private void InstantiatePixels()
    {
        for (int i = 0; i < maxRows; i++)
        {
            List<Pixel> row = new List<Pixel>();
            for (int j = 0; j < maxColumns; j++)
            {
                Vector3 targetPosition = new Vector3(bottomLeft.x + pixelSizeOffset + (j * pixelSize), bottomLeft.y + pixelSizeOffset + (i * pixelSize), bottomLeft.z);
                GameObject objectPixel = Instantiate(_pixelPrefab, targetPosition, Quaternion.identity);
                objectPixel.transform.SetParent(this.gameObject.transform);
                objectPixel.GetComponent<Renderer>().material = _pixelBlack;

                row.Add(new Pixel
                {
                    objectReference = objectPixel,
                    colorVal = objectPixel.GetComponent<Renderer>().material.color
                });

            }
            pixelMatrix.Add(row);
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

    public void SetNewConfiguration()
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

        string finalstr = randomIndex + " : ";
        for (int i = 0; i < 3; i++)
        {
            string rowstr = "";
            for (int j = 0; j < 3; j++)
            {
                if (targetConfiguration[i][j] == 1)
                {
                    // paint box white;
                    pixelMatrix[midx + 1 - i][midy + j - 1].objectReference.GetComponent<Renderer>().material = _pixelWhite;
                }
                else
                {
                    // paint box black;
                    pixelMatrix[midx + 1 - i][midy + j - 1].objectReference.GetComponent<Renderer>().material = _pixelBlack;
                }
                rowstr += targetConfiguration[i][j] + " ";
            }
            finalstr += rowstr + "  ";
        }
        Debug.Log(finalstr);

        //RunSimulation();
    }

    public void ResetSimulation()
    {
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                pixelMatrix[i][j].objectReference.GetComponent<Renderer>().material = _pixelBlack;
            }
        }
        SetNewConfiguration();
    }

    //public void StartSimulation()
    //{
    //    isRunSimulation = true;
    //}

    //public void PauseSimulation()
    //{
    //    isRunSimulation = false;
    //}

    //public void RunSimulation()
    //{
    //    /*
    //     * Any live cell with fewer than two live neighbours dies, as if by underpopulation.
    //     * Any live cell with two or three live neighbours lives on to the next generation.
    //     * Any live cell with more than three live neighbours dies, as if by overpopulation.
    //     * Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
    //     */

    //    for (int i = 0; i < maxRows; i++)
    //    {
    //        for (int j = 0; j < maxColumns; j++)
    //        {
    //            int liveNeighborCount = CountLiveNeighbors(i, j);
    //            if (liveNeighborCount != 0)
    //            {
    //                Debug.Log(i + ", " + j + " : " + liveNeighborCount);
    //            }
                
    //        }
    //    }
    //}

    //public int CountLiveNeighbors(int x, int y)
    //{
    //    int count = 0;

    //    // Array of relative positions of the 8 neighboring cells
    //    List<List<int>> directions = new List<List<int>>
    //    {
    //        new List<int> { 1, -1 }, new List<int> { 1, 0 }, new List<int> { 1, 1 },
    //        new List<int> { 0, -1 },                        new List<int> { 0, 1 },
    //        new List<int> { -1, -1 }, new List<int> { -1, 0 }, new List<int> { -1, 1 }
    //    };

    //    string debugStr = "(" + x + ", " + y + ") ";

    //    for (int i = 0; i < 8; i++)
    //    {
    //        int newX = x + directions[i][0];
    //        int newY = y + directions[i][1];

    //        debugStr = "(" + newX + ", " + newY + ") ";

    //        if (isWithinBounds(newX, newY))
    //        {
    //            debugStr += " is within bounds. ";
    //            if (pixelMatrix[newX][newY].objectReference != null)
    //            {
    //                debugStr += " Object ref is not null. ";
    //                Renderer renderer = pixelMatrix[newX][newY].objectReference.GetComponent<Renderer>();
    //                if (renderer.material == _pixelWhite)
    //                {
    //                    debugStr += " material is white.";
    //                    count++;
    //                }
    //            }
    //        }
    //    }

    //    return count;
    //}

    //bool isWithinBounds(int x, int y)
    //{
    //    if (x >= 0 && y >= 0 && x < maxRows && y < maxColumns) return true;
    //    return false;
    //}

}