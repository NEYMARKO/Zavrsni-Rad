using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public float[,] makeNoiseMap(int width, int height, float scale, float horizScroll, float vertScroll, float density)
    {
        float tempX;
        float tempY;
        float perlinValue;
        float[,] noiseMap = new float[width, height];

        float widthCenter = width / 2f;
        float heightCenter = height / 2f;
        
        if (scale <= 0) scale = 0.01f;
        density = Mathf.Clamp01(density);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tempX = (x - widthCenter) / (width * scale) * 100 * density + horizScroll;  
                tempY = (y - heightCenter) / (height * scale) * 100 * density + vertScroll;
                perlinValue = Mathf.PerlinNoise(tempX, tempY);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
