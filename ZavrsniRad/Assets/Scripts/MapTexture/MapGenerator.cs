using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //dimensions of map should be equal to dimensions of terrain
    public float scale;

    private DrawMap drawMap;
    private NoiseGenerator noiseGenerator;
    private void Start()
    {
        drawMap = GetComponent<DrawMap>();
        noiseGenerator = GetComponent<NoiseGenerator>();
    } 
    public void generateNoiseMap()
    {
        float[,] noiseMap = noiseGenerator.makeNoiseMap(drawMap.getWidth(), drawMap.getHeight(), scale);
        drawMap.drawNoiseMap(noiseMap);

    }
}
