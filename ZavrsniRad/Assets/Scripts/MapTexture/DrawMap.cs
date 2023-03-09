using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMap : MonoBehaviour
{

    /// <summary>
    /// This class is used for texturing plane using values from PerlinNoise function
    /// Each unit will be colored as black/white or something in between of those 2 colors based on value from
    /// noiseMap
    /// </summary>
    public Renderer planeTextureRenderer;
    public Terrain terrain;

    private int width;
    private int height;
    public void drawNoiseMap(float[,] noiseMap)
    {
        //dividing by 10 because plane dimensions are by default 10x10
        //if localScale is applied to the plane, it will be 10 times bigger than terrain
        width = (int) terrain.terrainData.size.x / 10;
        height = (int) terrain.terrainData.size.z / 10;

        Texture2D texture = new Texture2D(width, height);

        Color[] colors = new Color[width * height]; // 1D array because SetPixels accepts 1D array 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();

        planeTextureRenderer.sharedMaterial.mainTexture = texture;      //used so material can be previewed in scene mode without having to enter game mode
        planeTextureRenderer.transform.localScale = new Vector3(width, 1, height);  //scaling plane object by width in x-axis and height in z-axis
        //Debug.Log(planeTextureRenderer.bounds.size.x);
    }

    public int getWidth()
    {
        return width;
    }
    public int getHeight()
    {
        return height;
    }
}
