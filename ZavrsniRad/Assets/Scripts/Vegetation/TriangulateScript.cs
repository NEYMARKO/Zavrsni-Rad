using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangulateScript : MonoBehaviour
{

    public Terrain terrain;
    private int trianglesInTerrain;
    private float[] steepnessArray;
    private float[] triangleCentroidPositionArray;
    // Start is called before the first frame update
    private void Awake()
    {
        trianglesInTerrain = (int)(terrain.terrainData.size.x - 1) * (int)(terrain.terrainData.size.z - 1) * 2;
        triangleCentroidPositionArray = new float[trianglesInTerrain];
    }
    void Start()
    {
        steepnessArray = Triangulate();
        Debug.Log(steepnessArray.Length);
    }
    
    private float[] Triangulate()
    {
        float[] steepness = new float[trianglesInTerrain];   //no. of tris = (no. of points - 1)^2 * 2- dobiveno na papiru
        for (int j = 0; j < terrain.terrainData.size.z; j++)
        {
            for (int i = 0; i < terrain.terrainData.size.x - 1; i++)
            {
                float widthCentroidTris1 = (3 * i + 1) / (3f * terrain.terrainData.size.x);
                float lengthCentroidTris1 = (3 * j + 1) / (3f * terrain.terrainData.size.z);
                steepness[j * i * 2] = terrain.terrainData.GetSteepness(widthCentroidTris1, lengthCentroidTris1);  //pod isti i se trebaju strpati 2 trokuta

                float widthCentroidTris2 = (3 * i + 2) / (3f * terrain.terrainData.size.x);
                float lengthCentroidTris2 = (3 * j + 2) / (3f * terrain.terrainData.size.z);
                steepness[j * i * 2 + 1] = terrain.terrainData.GetSteepness(widthCentroidTris2, lengthCentroidTris2);

                setCentroidPositionArray(i, j);
            } 
        }
        return steepness;
    }

    private void setCentroidPositionArray(int i, int j)
    {
        triangleCentroidPositionArray[j * i * 2] = (3 * i + 1) / (3f * terrain.terrainData.size.x);
        triangleCentroidPositionArray[j * i * 2 + 1] = (3 * i + 2) / (3f * terrain.terrainData.size.x);
    }
    public float[] getSteepnessArray()
    {
        return steepnessArray;
    }

    public float[] getCentroidPositionArray()
    {
        return triangleCentroidPositionArray;
    }
}
