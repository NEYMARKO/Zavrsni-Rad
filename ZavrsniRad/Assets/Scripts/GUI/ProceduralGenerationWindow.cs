using UnityEngine;
using UnityEditor;

public class ProceduralGenerationWindow : EditorWindow
{
    GameObject objectToSpawn;
    GameObject mapGeneratorObject;
    private Texture2D noiseMapTexture;

    private float horizontalScroll = 0f;
    private float verticalScroll = 0f;
    private float scale = 1f;

    [MenuItem("Window/VegetationGenerator")]
 
    public static void ShowWindow()
    {
        GetWindow<ProceduralGenerationWindow>("Vegetation Generation");
    }
    private void OnGUI()
    {
        noiseMapTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false, GUILayout.Width(300), GUILayout.Height(300));
        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Vegetation objects", objectToSpawn, typeof(GameObject), true);
        mapGeneratorObject = (GameObject)EditorGUILayout.ObjectField("MapGenerator", mapGeneratorObject, typeof(GameObject), true);


        horizontalScroll = EditorGUILayout.Slider("Horizontal Scroll", horizontalScroll, -100f, 100f);
        verticalScroll = EditorGUILayout.Slider("Vertical Scroll", verticalScroll, -100f, 100f);
        scale = EditorGUILayout.Slider("Scale", scale, 0.1f, 10f);

        if (GUILayout.Button("Generate noiseMap"))
        {
            MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
            noiseMapTexture = mapGenerator.generateNoiseMap(scale, verticalScroll, horizontalScroll);
        }

        if (GUILayout.Button("Generate Vegetation"))
        {
            Debug.Log("ACTIVE TERRAIN: ", Terrain.activeTerrain.terrainData);
            GenerateVegetation(Terrain.activeTerrain, noiseMapTexture);
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
                    position.y = terrain.terrainData.GetInterpolatedHeight(x / (float) terrain.terrainData.size.x, z / (float) terrain.terrainData.size.y);
                    GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                    plantToSpawn.transform.SetParent(parent);
                }
            }
        }
    }
}

