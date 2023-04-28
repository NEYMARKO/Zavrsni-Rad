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

    private DrawMap drawMap;
    private NoiseGenerator noiseGenerator;
    private void Awake()
    {
        drawMap = GetComponent<DrawMap>();

        noiseGenerator = GetComponent<NoiseGenerator>();
    } 
    public void initialize()
    {
        drawMap = GetComponent<DrawMap>();

        noiseGenerator = GetComponent<NoiseGenerator>();
    }
    public Texture2D generateNoiseMap(float scale, float horizontalScroll, float verticalScroll)
    {
        drawMap = GetComponent<DrawMap>();
        noiseGenerator = GetComponent<NoiseGenerator>();
        float[,] noiseMap = noiseGenerator.makeNoiseMap(drawMap.getWidth(), drawMap.getHeight(), scale , horizontalScroll, verticalScroll, density);
        Texture2D texture = drawMap.drawNoiseMap(noiseMap);
        return texture;
    }
}
