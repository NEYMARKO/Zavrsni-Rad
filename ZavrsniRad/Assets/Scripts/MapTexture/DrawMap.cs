using UnityEngine;

public class DrawMap : MonoBehaviour
{
    public Renderer planeTextureRenderer;
    public Terrain terrain;

    private int width;
    private int height;
    public Texture2D drawNoiseMap(float[,] noiseMap)
    {
        //dividing by 10 because plane dimensions are by default 10x10
        //if s is applied to the plane, it will be 10 times bigger than terrain
        width = (int) terrain.terrainData.size.x / 10;
        height = (int) terrain.terrainData.size.z / 10;

        Texture2D texture = new Texture2D(width, height);

        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);
        Color[] colors = new Color[width * height]; 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colors);
        texture.Apply();

        planeTextureRenderer.sharedMaterial.mainTexture = texture;
        planeTextureRenderer.transform.localScale = new Vector3(width, 1, height);

        return texture;
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
