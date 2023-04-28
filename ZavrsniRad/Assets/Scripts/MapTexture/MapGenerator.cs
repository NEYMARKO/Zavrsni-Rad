using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private DrawMap drawMap;
    private NoiseGenerator noiseGenerator;
    public Texture2D generateNoiseMap(float scale, float horizontalScroll, float verticalScroll, float persistence, float octaves)
    {
        drawMap = drawMap == null ? GetComponent<DrawMap>() : drawMap;
        noiseGenerator = noiseGenerator == null ? GetComponent<NoiseGenerator>() : noiseGenerator;
        float[,] noiseMap = noiseGenerator.makeNoiseMap(drawMap.getWidth(), drawMap.getHeight(), scale , horizontalScroll, verticalScroll, persistence, octaves);
        Texture2D texture = drawMap.drawNoiseMap(noiseMap);
        return texture;
    }
}
