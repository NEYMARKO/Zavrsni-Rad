using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class ProceduralGenerationWindow : EditorWindow
{
    #region declaration
    GameObject objectToSpawn;
    GameObject mapGeneratorObject;
    private List<MeshFilter> childrenMeshFilters;
    private Texture2D noiseMapTexture;
    private Texture2D satelliteTexture;
    private Texture2D activeGenerationTexture;
    private string[] options = { "Satellite", "Procedural" };
    private int index;

    private float horizontalScroll = 0f;
    private float verticalScroll = 0f;
    private float surviveFactor = 0.5f;
    private float maxSpawnHeight = 10f;
    private float maxSteepness = 30f;
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
            noiseMapTexture = mapGenerator.generateNoiseMap(Terrain.activeTerrain, scale, verticalScroll, horizontalScroll, persistence, octaves);
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
        maxSteepness = EditorGUILayout.Slider("Maximum steepness angle", maxSteepness, 0f, 60f);
    }
    #region VegetationGeneration



    private pixelInfo[,] scanSatelliteImage(Texture2D satelliteTexture)
    {
        int satelliteTextureWidth = satelliteTexture.width;
        int satelliteTextureHeight = satelliteTexture.height;
        pixelInfo[,] greenPositions = new pixelInfo[satelliteTextureWidth, satelliteTextureHeight];

        int counter = 0;
        for (int z = 0; z < satelliteTextureHeight; z++)
        {
            for (int x = 0; x < satelliteTextureWidth; x++)
            {
                UnityEngine.Color pixel = satelliteTexture.GetPixel(x, z);
                float value = greenColorChecker(pixel);
                if (value > 0)
                {
                    greenPositions[x, z] = new pixelInfo(1, value);
                    counter++;
                }
                else
                {
                    greenPositions[x, z] = new pixelInfo(0, value);
                }
            }
        }
        return greenPositions;
    }
    private void GenerateVegetation(Terrain terrain, Texture2D spawnTexture, bool useNoiseMap)
    {
        float objectUpOffset = objectToSpawn.gameObject.transform.localScale.y; //pivot point is in the middle - it is calculating distance from pivot point to bottom of an object
        Transform parent = new GameObject("Vegetation").transform;

        if (childrenMeshFilters == null)
        {
            childrenMeshFilters = new List<MeshFilter>();
        }

        if (!useNoiseMap)
        {
            generateUsingScan(terrain, objectUpOffset, parent, satelliteTexture.width, satelliteTexture.height, useNoiseMap);
        }

        else 
        {
            int terrainWidth = (int)terrain.terrainData.size.x;
            int terrainHeight = (int)terrain.terrainData.size.z;

            for (int z = 0; z < terrainHeight; z++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {

                    float textureValue = spawnTexture.GetPixel(x, z).grayscale;

                    bool spawnSurvive = textureValue >= (1 - surviveFactor);

                    float spawnHeight = terrain.terrainData.GetInterpolatedHeight(x / (float)terrain.terrainData.size.x, z / (float)terrain.terrainData.size.z);
                    float angle = calculateSteepness(terrain, x, z, maxSteepness, useNoiseMap, null);
                    if (spawnSurvive && spawnHeight <= maxSpawnHeight && angle <= maxSteepness)
                    {
                        for (int i = 1; i <= 20; i++)
                        {
                            float spawnX = UnityEngine.Random.Range(x, x + 1f);
                            float spawnZ = UnityEngine.Random.Range(z, z + 1f);
                            spawnHeight = terrain.terrainData.GetInterpolatedHeight(spawnX / (float)terrain.terrainData.size.x, spawnZ / (float)terrain.terrainData.size.z) + objectUpOffset;
                            Vector3 position = new Vector3(spawnX, spawnHeight, spawnZ);
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
        childrenMeshFilters.Clear();
    }
    #endregion VegetationGeneration

    private void generateUsingScan(Terrain terrain, float objectUpOffset, Transform parent, float textureWidth, float textureHeight, bool useNoiseMap)
    {
        pixelInfo[,] greenSurface = scanSatelliteImage(satelliteTexture);

        for (int j = 0; j < textureHeight; j++)
        {
            for (int i = 0; i < textureWidth; i++)
            {
                float greenColorValue = greenSurface[i, j].colorValue;
                float height = terrain.terrainData.GetInterpolatedHeight(i / textureWidth, j / textureHeight);

                if (greenSurface[i, j].spawnValue == 1 && greenColorValue <= surviveFactor && height <= maxSpawnHeight)
                    //1 - greenSurface[i, j].colorValue => only most dense will remain (they have smallest hsv value)
                {
                    float angle = calculateSteepness(terrain, i, j, maxSteepness, useNoiseMap, satelliteTexture);
                    if (angle <= maxSteepness)
                    {
                        Vector3 position = new Vector3(i / textureWidth * terrain.terrainData.size.x, height + objectUpOffset, j / textureHeight * terrain.terrainData.size.z);
                        GameObject plantToSpawn = Instantiate(objectToSpawn, position, Quaternion.identity);
                        childrenMeshFilters.Add(plantToSpawn.GetComponent<MeshFilter>());
                        plantToSpawn.transform.SetParent(parent);
                    }
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
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, true, true);

        GameObject combinedObject = new GameObject("Combined Vegetation");
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>().sharedMaterial = objectToSpawn.GetComponent<MeshRenderer>().sharedMaterial;
    }
    private float greenColorChecker(UnityEngine.Color color)
    {
        float hue, saturation, value;
        UnityEngine.Color.RGBToHSV(color, out hue, out saturation, out value);
        if (hue >= 72/360f && hue <= 180/360f)
        {
            if (saturation >= 0.05f && value >= 0.05f)
            {
                return value;
            }
        }
        return 0;
    }

    private float calculateSteepness(Terrain terrain, int x, int z, float maxSteepness, bool useNoiseMap, Texture2D satelliteTexture)
    {
        float xPositionCheck = useNoiseMap == true ? x / terrain.terrainData.size.x : x / (float)satelliteTexture.width;
        float zPositionCheck = useNoiseMap == true ? z / terrain.terrainData.size.z : z / (float)satelliteTexture.height;

        float angle = terrain.terrainData.GetSteepness(xPositionCheck, zPositionCheck);

        return angle;
    }
    struct pixelInfo
    {
        public int spawnValue;
        public float colorValue;

        public pixelInfo(int spawnValue, float colorValue)
        {
            this.spawnValue = spawnValue;
            this.colorValue = colorValue;
        }
    }
}

