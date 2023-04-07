using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorUIWindow : EditorWindow
{

    GameObject objectToSpawn;
    MapGenerator mapGenerator;
    Terrain terrain;
    private Texture2D noiseMapTexture;
    private float density = 0.1f;

    [MenuItem("Window/MapGeneratorUI")]
    public static void ShowWindow()
    {
        GetWindow<MapGeneratorUIWindow>("Vegetation Generator");
    }
    void onGUI()
    {
        /*if (GUILayout.Button("Generate Noise Map"))
        {
            mapGenerator.generateNoiseMap();
        }*/
        GUILayout.Label("THIS IS LABEL");
        noiseMapTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false);
        if (GUILayout.Button("Generate noiseMap"))
        {
            noiseMapTexture = mapGenerator.generateNoiseMap();
        }

        density = EditorGUILayout.Slider("Density", density, 0, 1);

        if (GUILayout.Button("Generate Vegetation"))
        {
            GenerateVegetation(terrain, noiseMapTexture);
        }
    }
    private void GenerateVegetation(Terrain terrain, Texture2D noiseMapTexture)
    {
        Transform parent = new GameObject("Vegetation").transform;
        for (int x = 0; x < terrain.terrainData.size.x; x++)
        {
            for (int z = 0; z < terrain.terrainData.size.y; z++)
            {
                float noiseMapValue = noiseMapTexture.GetPixel(x, z).g;
                if (noiseMapValue > 0)
                {
                    Vector3 position = new Vector3(x, 0, z);
                    position.y = terrain.terrainData.GetInterpolatedHeight(x / (float)terrain.terrainData.size.x, z / (float)terrain.terrainData.size.y);
                    GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                    plantToSpawn.transform.SetParent(parent);
                }
            }
        }
    }
}