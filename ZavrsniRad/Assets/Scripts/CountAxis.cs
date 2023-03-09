using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountAxis : MonoBehaviour
{
    [Header("Parent object")]
    public Terrain terrainObject;

    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        for (int x = 0; x < terrainObject.terrainData.size.x; x++)
        {
            for (int z = 0; z < terrainObject.terrainData.size.z; z++)
            {
                count++;
            }
        }
        Debug.Log("Count: " + count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
