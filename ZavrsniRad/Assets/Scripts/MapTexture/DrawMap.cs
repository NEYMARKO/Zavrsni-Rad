using UnityEngine;

public class DrawMap : MonoBehaviour
{
    public Renderer planeTextureRenderer;
    public Terrain terrain;

    private int width;
    private int height;
    public Texture2D drawNoiseMap(float[,] noiseMap)
    {
        width = (int) terrain.terrainData.size.x;
        height = (int) terrain.terrainData.size.z;

        int textureWidth = 1024;
        int textureHeight = 1024;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Color[] colors = new Color[textureWidth * textureHeight]; 

        for (int y = 0; y < textureHeight; y++)
        {
            int currentPickHeight = (int ) (y / (float)textureHeight * height);
            for (int x = 0; x < textureWidth; x++)
            {
                int currentPickWidth = (int)(x / (float)textureWidth * width);
                colors[y * 1024 + x] = Color.Lerp(Color.black, Color.white, noiseMap[currentPickHeight, currentPickWidth]);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();

        planeTextureRenderer.sharedMaterial.mainTexture = texture;
        planeTextureRenderer.transform.localScale = new Vector3(textureWidth, 1, textureHeight);

        return texture;
    }
}