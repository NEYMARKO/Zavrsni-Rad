using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangulateScript : MonoBehaviour
{

    public Terrain terrain;
    private int trianglesInTerrain;
    private float[] steepnessArray;
    private Vector2[] triangleCentroidPositionArray;
    // Start is called before the first frame update
    private void Awake()
    {
        trianglesInTerrain = (int)(terrain.terrainData.size.x - 1) * (int)(terrain.terrainData.size.z - 1) * 2;
        triangleCentroidPositionArray = new Vector2[trianglesInTerrain];
    }
    void Start()
    {
        steepnessArray = Triangulate();
        //Debug.Log(steepnessArray.Length);
        //ovo srusi editor
        /*foreach (Vector2 v in triangleCentroidPositionArray)
        {
            Debug.Log("v: " + v);
        }*/
        for (int i = 0; i < 1000; i++)
        {
            Debug.Log(triangleCentroidPositionArray[100000 + i]); 
        }
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
        triangleCentroidPositionArray[j * i * 2] = new Vector2((3 * i + 1) / (3f * terrain.terrainData.size.x), (3 * j + 1) / (3f * terrain.terrainData.size.z));
        triangleCentroidPositionArray[j * i * 2 + 1] = new Vector2((3 * i + 2) / (3f * terrain.terrainData.size.x), (3 * j + 2) / (3f * terrain.terrainData.size.z));
    }
    public float[] getSteepnessArray()
    {
        return steepnessArray;
    }

    //podaci se spremaju u Vector2 jer su Tuple (trebaju mi 2 vrijednosti)
    public Vector2[] getCentroidPositionArray()
    {
        return triangleCentroidPositionArray;
    }
}
