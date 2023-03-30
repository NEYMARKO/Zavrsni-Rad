using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangulateScript : MonoBehaviour
{

    public Terrain terrain;
    private float[] steepnessArray;
    
    // Start is called before the first frame update
    void Start()
    {
        steepnessArray = Triangulate();
        Debug.Log(steepnessArray.Length);
    }
    
    private float[] Triangulate()
    {
        float[] steepness = new float[(int)(terrain.terrainData.size.x - 1) * (int)(terrain.terrainData.size.z - 1) * 2];   //no. of tris = (no. of points - 1)^2 * 2- dobiveno na papiru
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
            } 
        }
        return steepness;
    }
    /*private void Triangulate()
    {
        int x = 0;
        int width = (int)terrain.terrainData.size.x; 
        for (int z = 0; z < terrain.terrainData.size.x * terrain.terrainData.size.z; z += 3) 
        {
            if (z % width == 0)
            {
                x++;
            }
            for (int i = 0; i < 3; i++)
            {
                Vector3 position = new Vector3(x, 0, z);
                position.y = terrain.terrainData.GetInterpolatedHeight(x / terrain.terrainData.size.x, z / terrain.terrainData.size.z);
            }
        }
    }*/
}
