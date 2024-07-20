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

    [System.Serializable]
    public struct Pixel
    {
        public GameObject objectReference;
        public Color colorVal;
    }

    public List<List<Pixel>> pixelMatrix = new List<List<Pixel>>(); 
    
    void Start()
    {
        Debug.Log(_mainCamera.orthographicSize + ", " + _mainCamera.aspect);
        windowHeight = 2 * _mainCamera.orthographicSize;
        windowWidth = windowHeight * _mainCamera.aspect;
        Debug.Log(windowHeight + ", " + windowWidth);

        float pixelSize = _pixelPrefab.transform.localScale.x;
        float pixelSizeOffset = pixelSize / 2;
        int maxColumns = Mathf.FloorToInt(windowWidth / pixelSize);
        int maxRows = Mathf.FloorToInt(windowHeight / pixelSize);
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
    }


    void Update()
    {
        
    }
}
