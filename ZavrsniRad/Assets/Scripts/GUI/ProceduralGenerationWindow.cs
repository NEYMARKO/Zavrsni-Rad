using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

public class ProceduralGenerationWindow : EditorWindow
{
    GameObject objectToSpawn;
    GameObject mapGeneratorObject;
    private Texture2D noiseMapTexture;

    private float horizontalScroll = 0f;
    private float verticalScroll = 0f;
    private float surviveFactor = 0.5f;
    private float scale = 1f;
    private float persistence = 1f;
    //private float frequency = 1f;
    //private float amplitude = 1f;
    private float octaves = 1;

    [MenuItem("Window/VegetationGenerator")]

    public static void ShowWindow()
    {
        GetWindow<ProceduralGenerationWindow>("Vegetation Generation");
    }
    private void OnGUI()
    {
        noiseMapTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false, GUILayout.Width(400), GUILayout.Height(400));
        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Vegetation objects", objectToSpawn, typeof(GameObject), true);
        mapGeneratorObject = (GameObject)EditorGUILayout.ObjectField("MapGenerator", mapGeneratorObject, typeof(GameObject), true);


        horizontalScroll = EditorGUILayout.Slider("Horizontal Scroll", horizontalScroll, -100f, 100f);
        verticalScroll = EditorGUILayout.Slider("Vertical Scroll", verticalScroll, -100f, 100f);
        surviveFactor = EditorGUILayout.Slider("Survive Factor", surviveFactor, 0f, 1f);
        //frequency = EditorGUILayout.Slider("Frequency", frequency, 0.001f, 1f);
        //amplitude = EditorGUILayout.Slider("Amplitude", amplitude, 0.1f, 2f);
        persistence = EditorGUILayout.Slider("Persistence", persistence, 0.1f, 2f);
        octaves = (int)EditorGUILayout.Slider("Octaves", octaves, 1, 5);
        scale = EditorGUILayout.Slider("Scale", scale, 0.1f, 20f);

        if (GUILayout.Button("Generate noiseMap"))
        {
            MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
            noiseMapTexture = mapGenerator.generateNoiseMap(scale, verticalScroll, horizontalScroll, persistence, octaves);
        }

        if (GUILayout.Button("Generate Vegetation"))
        {
            GenerateVegetation(Terrain.activeTerrain, noiseMapTexture);
        }
    }
    private void GenerateVegetation(Terrain terrain, Texture2D noiseMapTexture)
    {
        float objectUpOffset = objectToSpawn.gameObject.transform.localScale.y; //pivot point is in the middle - it is calculating distance from pivot point to bottom of an object
        Transform parent = new GameObject("Vegetation").transform;
        //Debug.Log("TERRAIN DIMENSIONS: " + terrain.terrainData.size.x + " x " + terrain.terrainData.size.z);
        for (int x = 0; x < terrain.terrainData.size.x; x++)
        {
            for (int z = 0; z < terrain.terrainData.size.z; z++)
            {
                float noiseMapValue = noiseMapTexture.GetPixel(x, z).g;

                if (noiseMapValue > surviveFactor)
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        float spawnX = Random.Range(x, x + 2f);
                        float spawnZ = Random.Range(z, z + 2f);
                        Vector3 position = new Vector3(spawnX, 0, spawnZ);
                        position.y = terrain.terrainData.GetInterpolatedHeight(spawnX / (float)terrain.terrainData.size.x, spawnZ / (float)terrain.terrainData.size.z) + objectUpOffset;
                        //Vector3 position = new Vector3(x + i / 10, 0, z + i / 10);
                        //position.y = terrain.terrainData.GetInterpolatedHeight(x / (float)terrain.terrainData.size.x, z / (float)terrain.terrainData.size.z) + objectUpOffset;
                        GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                        plantToSpawn.transform.SetParent(parent);
                    }
                }
            }
        }
    }

}

