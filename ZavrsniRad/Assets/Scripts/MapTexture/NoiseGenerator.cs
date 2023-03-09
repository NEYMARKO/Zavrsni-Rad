using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    /// <summary>
    /// Purpose of this class is to map values returned from PerlinNoise function to
    /// each unit on width x height plane
    /// </summary>
    /// <param name="width"> Width of map on which is noise going to be textured </param>
    /// <param name="height"> Height of map on which is noise going to be textured </param>
    /// <param name="scale"> Used for zooming in and out of map </param>
    /// <returns> Array of perlin values </returns>
    public float[,] makeNoiseMap(int width, int height, float scale)
    {
        float tempX;
        float tempY;
        float perlinValue;
        float[,] noiseMap = new float[width, height];
        /*Debug.Log("Scale: " + scale);
        Debug.Log("WIDTH / SCALE: " + width / (float) scale);
        Debug.Log("DIVISION RESULTS IN FLOAT: " + ((width / scale).GetType() == typeof(float)));*/

        if (scale <= 0) scale = 0.01f;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tempX = x / (width * scale) * 100;      //values that are used to calculate perlin noise value at given pixel
                tempY = y / (height * scale) * 100;      //values that are used to calculate perlin noise value at given pixel
                //Debug.Log("X: " + tempX + " Y: " + tempY);
                perlinValue = Mathf.PerlinNoise(tempX, tempY);
                //Debug.Log("Perlin value: " + perlinValue);
                //Debug.Log("SCALE: " + scale);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
