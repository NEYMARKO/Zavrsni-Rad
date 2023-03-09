using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VegetationGenerator : MonoBehaviour
{
    [Header ("Parent object")]
    public Transform terrainObject;

    [Header ("Children objects")]
    private List<Transform> childrenList = new List<Transform>();
    private List<Mesh> childrenMeshesList = new List<Mesh>();
    private List<Vector3> childVertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in terrainObject) 
        {
            Mesh mesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            childrenList.Add(child);
            childrenMeshesList.Add(mesh);
            //Debug.Log(child.gameObject.name);
        }
        Debug.Log("after initial");
        for (int i = 0; i < childrenList.Count; i++)
        {
            //Debug.Log(childrenList[i].gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
