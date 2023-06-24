using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private DrawMap drawMap;
    private NoiseGenerator noiseGenerator;
    public Texture2D generateNoiseMap(Terrain terrain, float scale, float horizontalScroll, float verticalScroll, float persistence, float octaves)
    {
        drawMap = drawMap == null ? GetComponent<DrawMap>() : drawMap;
        noiseGenerator = noiseGenerator == null ? GetComponent<NoiseGenerator>() : noiseGenerator;
        float[,] noiseMap = noiseGenerator.makeNoiseMap((int)terrain.terrainData.size.x, (int)terrain.terrainData.size.z, scale, horizontalScroll, verticalScroll, persistence, octaves);
        Texture2D texture = drawMap.drawNoiseMap(noiseMap);
        
        return texture;
    }
}
