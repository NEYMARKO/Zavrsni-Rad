using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //dimensions of map should be equal to dimensions of terrain
    [Header ("Parammeters")]
    public float scale;
    public float frequency;
    public float density;

    [Header ("Scrolling")]
    public float verticalScroll;
    public float horizontalScroll;

    //private DrawMap drawMap;
    //private NoiseGenerator noiseGenerator;

    public DrawMap drawMap;
    public NoiseGenerator noiseGenerator;
    private void Start()
    {
        //drawMap = GetComponent<DrawMap>();
        //noiseGenerator = GetComponent<NoiseGenerator>();
    } 
    public void generateNoiseMap()
    {
        float[,] noiseMap = noiseGenerator.makeNoiseMap(drawMap.getWidth(), drawMap.getHeight(), scale , horizontalScroll, verticalScroll, density);
        drawMap.drawNoiseMap(noiseMap);

    }
}
