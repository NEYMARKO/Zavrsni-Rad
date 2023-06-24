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

        Texture2D texture = new Texture2D(width, height);

        Color[] colors = new Color[width * height]; 

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[y, x]);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();

        planeTextureRenderer.sharedMaterial.mainTexture = texture;
        planeTextureRenderer.transform.localScale = new Vector3(width, 1, height);

        return texture;
    }
}
