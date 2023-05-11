using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Jobs;

public class ProceduralGenerationWindow : EditorWindow
{
    #region declaration
    GameObject objectToSpawn;
    GameObject mapGeneratorObject;
    private List<MeshFilter> childrenMeshFilters;
    private Texture2D noiseMapTexture;
    private Texture2D satelliteTexture;
    private Texture2D bigSatelliteTexture;
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
        showSpawnSurviveParammeters();
        showGenerateNoise();
        checkIfCanGenerate(index);

    }

    #endregion ProceduralHeightMap
    private void showSatellite()
    {
        satelliteTexture = textureAlign(satelliteTexture);
        checkIfCanGenerate(index);
        showSpawnSurviveParammeters();
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

    private void showSpawnSurviveParammeters()
    {
        surviveFactor = EditorGUILayout.Slider("Survive Factor", surviveFactor, 0f, 1f);
        maxSpawnHeight = EditorGUILayout.Slider("Maximum spawn height", maxSpawnHeight, 0f, Terrain.activeTerrain.terrainData.size.y);
    }
    #region VegetationGeneration



    private int[,] scanSatelliteImage(Texture2D satelliteTexture)
    {
        int satelliteTextureWidth = satelliteTexture.width;
        int satelliteTextureHeight = satelliteTexture.height;
        int[,] greenPositions = new int[satelliteTextureWidth, satelliteTextureHeight];

        int counter = 0;
        for (int z = 0; z < satelliteTextureHeight; z++)
        {
            for (int x = 0; x < satelliteTextureWidth; x++)
            {
                UnityEngine.Color pixel = satelliteTexture.GetPixel(x, z);
                if (greenColorChecker(pixel))
                {
                    greenPositions[x, z] = 1;
                    counter++;
                }
                else
                {
                    greenPositions[x, z] = 0;
                }
            }
        }
        Debug.Log("COUNTER: " + counter);
        return greenPositions;
    }
    private void GenerateVegetation(Terrain terrain, Texture2D spawnTexture, bool useNoiseMap)
    {
        /*Debug.Log("TEXURE DIMENSIONS: " + satelliteTexture.width + " " + satelliteTexture.height);
        Debug.Log("SATELLITE TEXTURE / TERRAIN: " + satelliteTexture.width / terrain.terrainData.size.x);
        Debug.Log("ROUNDED with (int): " + (int) (satelliteTexture.width / terrain.terrainData.size.x));*/

        float objectUpOffset = objectToSpawn.gameObject.transform.localScale.y; //pivot point is in the middle - it is calculating distance from pivot point to bottom of an object
        Transform parent = new GameObject("Vegetation").transform;
        if (!useNoiseMap)
        {
            generateUsingScan(terrain, objectUpOffset, parent, satelliteTexture.width, satelliteTexture.height);
        }

        else 
        {
            int satelliteTextureWidth = satelliteTexture.width;

            int width = (int)terrain.terrainData.size.x;
            int height = (int)terrain.terrainData.size.z;

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {

                    Vector2 pixelPosition = new Vector2(x, z);

                    UnityEngine.Color trueColor = calculateTrueColor(pixelPosition, (int)(satelliteTextureWidth / terrain.terrainData.size.x), spawnTexture);

                    float textureValue = useNoiseMap == true ? spawnTexture.GetPixel(x, z).grayscale : spawnTexture.GetPixel(x, z).g;

                    bool checkIfGreen = greenColorChecker(trueColor);
                    bool spawnSurvive = useNoiseMap == true ? textureValue >= surviveFactor : checkIfGreen;

                    if ((spawnSurvive && textureValue * terrain.terrainData.GetInterpolatedHeight(x / (float)terrain.terrainData.size.x, z / (float)terrain.terrainData.size.z) < maxSpawnHeight) || checkIfGreen)
                    {
                        for (int i = 1; i <= 20; i++)
                        {
                            float spawnX = UnityEngine.Random.Range(x, x + 1f);
                            float spawnZ = UnityEngine.Random.Range(z, z + 1f);
                            Vector3 position = new Vector3(spawnX, 0, spawnZ);
                            position.y = terrain.terrainData.GetInterpolatedHeight(spawnX / (float)terrain.terrainData.size.x, spawnZ / (float)terrain.terrainData.size.z) + objectUpOffset;
                            GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                            childrenMeshFilters.Add(plantToSpawn.GetComponent<MeshFilter>());
                            plantToSpawn.transform.SetParent(parent);
                        }
                    }
                }
            }
        }
        combineMeshFilters(parent.gameObject);
        DestroyImmediate(parent.gameObject);

    }
    #endregion VegetationGeneration

    private void generateUsingScan(Terrain terrain, float objectUpOffset, Transform parent, float textureWidth, float textureHeight)
    {
        int[,] greenSurface = scanSatelliteImage(satelliteTexture);

        for (int j = 0; j < textureHeight; j++)
        {
            for (int i = 0; i < textureWidth; i++)
            {
                if (greenSurface[i, j] == 1)
                {
                    Vector3 position = new Vector3(i/textureWidth * terrain.terrainData.size.x, 0, j/textureHeight * terrain.terrainData.size.z);
                    position.y = terrain.terrainData.GetInterpolatedHeight(i / (float) textureWidth * terrain.terrainData.size.x, j / (float)textureHeight * terrain.terrainData.size.z) + objectUpOffset;
                    GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                    childrenMeshFilters.Add(plantToSpawn.GetComponent<MeshFilter>());
                    plantToSpawn.transform.SetParent(parent);
                }
            }
        }
    }
    private void combineMeshFilters(GameObject parentObject)
    {
        MeshFilter[] meshFilters = parentObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.SetActive(false);
            //DestroyImmediate(meshFilters[i].gameObject);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, true, true);

        GameObject combinedObject = new GameObject("Combined Vegetation");
        //combinedObject.transform.SetParent(parentObject.transform);
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>().sharedMaterial = objectToSpawn.GetComponent<MeshRenderer>().sharedMaterial;
    }
    private bool greenColorChecker(UnityEngine.Color color)
    {
        float hue, saturation, value;
        UnityEngine.Color.RGBToHSV(color, out hue, out saturation, out value);
        if (hue >= 100/360f && hue <= 160/360f)
        {
            if (saturation >= 0.35f && value >= 0.05f)
            {
                return true;
            }
        }
        return false;
    }

    private UnityEngine.Color calculateTrueColor(Vector2 pixelPosition, int factor, Texture2D spawnTexture)
    {
        int x = (int) pixelPosition.x;
        int z = (int) pixelPosition.y;

        float red = 0;
        float green = 0;
        float blue = 0;
        float alpha = 0;
        int counter = 0;

        for (int i = z * factor; i < (z + 1) * factor; i++)
        {
            for (int j = x * factor; j < (x + 1) * factor; j++)
            {
                red += spawnTexture.GetPixel(i, j).r;
                green += spawnTexture.GetPixel(i, j).g;
                blue += spawnTexture.GetPixel(i, j).b;
                alpha += spawnTexture.GetPixel(i, j).a;
                counter++;
            }
        }
        return new UnityEngine.Color(red/counter, green/counter, blue/counter, alpha/counter);
    }
}

