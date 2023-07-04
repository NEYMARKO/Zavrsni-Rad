using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public float[,] makeNoiseMap(int width, int height, float scale, float horizScroll, float vertScroll, float persistence, float octaves)
    {
        float tempX;
        float tempY;
        float amplitude;
        float frequency;
        float perlinValue;
        float totalPerlinNoiseValue = 0f;
        float[,] noiseMap = new float[width, height];

        float widthCenter = width / 2f;
        float heightCenter = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1f;
                frequency = 1f;
                totalPerlinNoiseValue = 0f;
                for (int octave = 0; octave < octaves; octave++)
                {
                    tempX = (x - widthCenter) / (width * scale * frequency) * 100 + horizScroll;
                    tempY = (y - heightCenter) / (height * scale * frequency) * 100 + vertScroll;
                    perlinValue = Mathf.PerlinNoise(tempX, tempY) * 2 - 1;
                    totalPerlinNoiseValue += perlinValue * amplitude;

                    frequency *= 2f;
                    amplitude *= persistence;

                }
                noiseMap[x, y] = totalPerlinNoiseValue;
            }
        }
        return noiseMap;
    }
}
