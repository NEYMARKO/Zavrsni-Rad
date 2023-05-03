using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using System.Drawing;
using UnityEditor.UI;

public class ProceduralGenerationWindow : EditorWindow
{
    #region declaration
    GameObject objectToSpawn;
    GameObject mapGeneratorObject;
    GameObject textureApplier;
    private Texture2D noiseMapTexture;
    private Texture2D satelliteTexture;
    private Texture2D activeGenerationTexture;
    private string[] options = { "Satellite", "Procedural" };
    private int index;

    private float horizontalScroll = 0f;
    private float verticalScroll = 0f;
    private float surviveFactor = 0.5f;
    private float maxSpawnHeight = 10f;
    private float scale = 1f;
    private float persistence = 1f;
    private float octaves = 1;
    #endregion declaration

    [MenuItem("Window/VegetationGenerator")]

    public static void ShowWindow()
    {
        GetWindow<ProceduralGenerationWindow>("Vegetation Generation");
    }
    private void OnGUI()
    {
        index = EditorGUILayout.Popup(index, options);

        if (index == 1)
            showProceduralHeightMap();
        else showSatellite();

    }

    #region ProceduralHeightMap
    private void showProceduralHeightMap()
    {
        noiseMapTexture = textureAlign(noiseMapTexture);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("NOISE MAP PARAMETERS");
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Vegetation objects", objectToSpawn, typeof(GameObject), true);
        mapGeneratorObject = (GameObject)EditorGUILayout.ObjectField("MapGenerator", mapGeneratorObject, typeof(GameObject), true);
        horizontalScroll = EditorGUILayout.Slider("Horizontal Scroll", horizontalScroll, -100f, 100f);
        verticalScroll = EditorGUILayout.Slider("Vertical Scroll", verticalScroll, -100f, 100f);
        persistence = EditorGUILayout.Slider("Persistence", persistence, 0.1f, 2f);
        octaves = (int)EditorGUILayout.Slider("Octaves", octaves, 1, 5);
        scale = EditorGUILayout.Slider("Scale", scale, 0.1f, 20f);

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("VEGETATION SURVIVAL PARAMETERS");
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        surviveFactor = EditorGUILayout.Slider("Survive Factor", surviveFactor, 0f, 1f);
        maxSpawnHeight = EditorGUILayout.Slider("Maximum spawn height", maxSpawnHeight, 0f, Terrain.activeTerrain.terrainData.size.y);
        showGenerateNoise();
        checkIfCanGenerate(index);

    }

    #endregion ProceduralHeightMap
    private void showSatellite()
    {
        satelliteTexture = textureAlign(satelliteTexture);
        //textureApplier = (GameObject)EditorGUILayout.ObjectField("Texture applier", textureApplier, typeof(GameObject), true);
        checkIfCanGenerate(index);
        surviveFactor = EditorGUILayout.Slider("Survive Factor", surviveFactor, 0f, 1f);
        maxSpawnHeight = EditorGUILayout.Slider("Maximum spawn height", maxSpawnHeight, 0f, Terrain.activeTerrain.terrainData.size.y);
    }

    private Texture2D textureAlign(Texture2D texture)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(300), GUILayout.Height(300));
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        return texture;
    }

    private void checkIfCanGenerate(int index)
    {
        if (index == 1)
        {
            GUI.enabled = noiseMapTexture == null ? false : true;
            activeGenerationTexture = noiseMapTexture;
            showVegetationGeneration(true);
            GUI.enabled = true;
        }
        else
        {
            GUI.enabled = satelliteTexture == null ? false : true;
            activeGenerationTexture = satelliteTexture;
            showVegetationGeneration(false);
            GUI.enabled = true;
        }
    }
    private void showGenerateNoise()
    {
        if (GUILayout.Button("Generate noiseMap"))
        {
            MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
            noiseMapTexture = mapGenerator.generateNoiseMap(scale, verticalScroll, horizontalScroll, persistence, octaves);
        }
    }
    private void showVegetationGeneration(bool useNoiseMap)
    { 
        if (GUILayout.Button("Generate Vegetation"))
        {
            GenerateVegetation(Terrain.activeTerrain, activeGenerationTexture, useNoiseMap);
        }
    }

    #region VegetationGeneration
    private void GenerateVegetation(Terrain terrain, Texture2D spawnTexture, bool useNoiseMap)
    {
        float objectUpOffset = objectToSpawn.gameObject.transform.localScale.y; //pivot point is in the middle - it is calculating distance from pivot point to bottom of an object
        Transform parent = new GameObject("Vegetation").transform;
        int width = (int) terrain.terrainData.size.x;
        int height = (int)terrain.terrainData.size.z;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float textureValue = useNoiseMap == true ? spawnTexture.GetPixel(x, z).grayscale : spawnTexture.GetPixel(x, z).g;

                var pixel = spawnTexture.GetPixel(x, z);
                float red = pixel.r;
                float green = pixel.g;
                float blue = pixel.b;

                //Debug.Log("RGB: (" + red + ", " + green + ", " + blue + ")");


                //bool checkIfGreen = ((blue < surviveFactor * green) && (red < surviveFactor * green));
                bool checkIfGreen = green >= ((red + blue) / 1.25f * (1 + surviveFactor));
                bool spawnSurvive = useNoiseMap == true ? textureValue >= surviveFactor : checkIfGreen;

                if (spawnSurvive && textureValue * terrain.terrainData.GetInterpolatedHeight(x / (float)terrain.terrainData.size.x, z / (float)terrain.terrainData.size.z) < maxSpawnHeight)
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        float spawnX = Random.Range(x, x + 1f);
                        float spawnZ = Random.Range(z, z + 1f);
                        Vector3 position = new Vector3(spawnX, 0, spawnZ);
                        position.y = terrain.terrainData.GetInterpolatedHeight(spawnX / (float)terrain.terrainData.size.x, spawnZ / (float)terrain.terrainData.size.z) + objectUpOffset;
                        GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                        plantToSpawn.transform.SetParent(parent);
                    }
                }
            }
        }
    }
    #endregion VegetationGeneration
}

