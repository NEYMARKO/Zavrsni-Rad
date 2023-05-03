using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureApplier : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] Texture2D texture;

    private int terrainWidth;
    private int terrainHeight;
    private Texture2D resizeTexture(Texture2D texture)
    {
        terrainWidth = (int)terrain.terrainData.size.x;
        terrainHeight = (int)terrain.terrainData.size.z;

        Texture2D terrainTexture = new Texture2D(terrainWidth, terrainHeight);

        Color[] colors = new Color[terrainWidth * terrainHeight];

        for (int y = 0; y < terrainHeight; y++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                colors[y * terrainWidth + x] = Color.Lerp(Color.black, Color.white, terrainTexture.GetPixel(x, y).g / 255f);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}
