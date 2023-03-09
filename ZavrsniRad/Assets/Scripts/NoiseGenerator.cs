using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public float[,] MakeNoiseMap(int width, int height, float scale)
    {
        float tempX;
        float tempY;
        float perlinValue;
        float[,] noiseMap = new float[width, height];

        if (scale <= 0) scale = 0.01f;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tempX = x / scale;      //values that are used to calculate perlin noise value at given pixel
                tempY = y / scale;      //values that are used to calculate perlin noise value at given pixel
                perlinValue = Mathf.PerlinNoise(tempX, tempY);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
