using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorUI : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Noise Map"))
        {
            mapGenerator.generateNoiseMap();
        }
    }
}
