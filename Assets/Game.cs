using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject _pixelPrefab;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Material _pixelBlack;
    [SerializeField] private Material _pixelWhite;
    float windowHeight;
    float windowWidth;
    float pixelSizeOffset;
    float screenOffset;
    int maxColumns;
    int maxRows;

    [System.Serializable]
    public struct Pixel
    {
        public GameObject objectReference;
        public Color colorVal;
    }

    public List<List<Pixel>> pixelMatrix = new List<List<Pixel>>();

    public int[,] gridConfigurations; // 2D array to store configurations
    int totalConfigurations = 0;

    void Start()
    {
        Debug.Log(_mainCamera.orthographicSize + ", " + _mainCamera.aspect);
        windowHeight = 2 * _mainCamera.orthographicSize;
        windowWidth = windowHeight * _mainCamera.aspect;
        Debug.Log(windowHeight + ", " + windowWidth);

        float pixelSize = _pixelPrefab.transform.localScale.x;
        float pixelSizeOffset = pixelSize / 2;
        maxColumns = Mathf.FloorToInt(windowWidth / pixelSize);
        maxRows = Mathf.FloorToInt(windowHeight / pixelSize);
        Debug.Log(maxRows + ", " + maxColumns);
        Debug.Log(maxColumns * pixelSize + ", " + maxRows * pixelSize);

        float gridWidth = maxColumns * pixelSize;
        float gridHeight = maxRows * pixelSize;

        gameObject.transform.localScale = new Vector3(gridWidth, gridHeight, 0.1f);

        Vector3 bottomLeft = new Vector3(-gridWidth / 2, -gridHeight / 2, gameObject.transform.position.z - 0.5f);

        for (int i = 0; i < maxRows; i++)
        {
            List<Pixel> row = new List<Pixel>();
            for (int j = 0; j < maxColumns; j++)
            {
                Vector3 targetPosition = new Vector3(bottomLeft.x + pixelSizeOffset + (j*pixelSize), bottomLeft.y + pixelSizeOffset + (i * pixelSize), bottomLeft.z);
                GameObject objectPixel = Instantiate(_pixelPrefab, targetPosition, Quaternion.identity);
                objectPixel.transform.SetParent(this.gameObject.transform);
                objectPixel.GetComponent<Renderer>().material = _pixelBlack;

                row.Add(new Pixel
                {
                    objectReference = objectPixel,
                    colorVal = objectPixel.GetComponent<Renderer>().material.color
                }) ;

            }
            pixelMatrix.Add(row);
        }

        int midx = maxRows / 2;
        int midy = maxColumns / 2;

        pixelMatrix[midx][midy].objectReference.GetComponent<Renderer>().material = _pixelWhite;

        ComputeAllInitialGridConfigurations();
    }

    public void ComputeAllInitialGridConfigurations()
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
        int randomIndex = Random.Range(0, totalConfigurations + 1);
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

        int midx = maxRows / 2;
        int midy = maxColumns / 2;
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
    }
}