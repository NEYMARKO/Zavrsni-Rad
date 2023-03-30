using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorUI : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator) target;

        if (DrawDefaultInspector())
        {
            mapGenerator.generateNoiseMap();
        }

        /*if (GUILayout.Button("Generate Noise Map"))
        {
            mapGenerator.generateNoiseMap();
        }*/
    }
}
