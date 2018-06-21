using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cloud))]
public class CloudEditor : Editor
{
    Cloud myTarget;
    int labelWidth = 120;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutNoise = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;
    
    public void OnEnable()
    {
        myTarget = (Cloud)target;
    }

    public override void OnInspectorGUI()
    {
        string[] availableSizes = new string[myTarget.AvailableSizes.Length];
        for (int i = 0; i < myTarget.AvailableSizes.Length; i++) availableSizes[i] = myTarget.AvailableSizes[i].ToString();

        EditorGUILayout.Space();

        foldoutGeneral = EditorGUILayout.Foldout(foldoutGeneral, "General Properties", true, EditorStyles.boldLabel);
        if (foldoutGeneral)
        {
            myTarget.CustomSeed = EditorGUILayout.BeginToggleGroup("Seed", myTarget.CustomSeed);
            {
                EditorGUI.indentLevel++;
                myTarget.Seed = EditorGUILayout.IntField(myTarget.Seed);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomSize = EditorGUILayout.BeginToggleGroup("Size", myTarget.CustomSize);
            {
                EditorGUI.indentLevel++;
                myTarget.Size = EditorGUILayout.IntPopup(myTarget.Size, availableSizes, myTarget.AvailableSizes);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
        }

        foldoutNoise = EditorGUILayout.Foldout(foldoutNoise, "Noise Properties", true, EditorStyles.boldLabel);
        if (foldoutNoise)
        {
            myTarget.CustomFrequency = EditorGUILayout.BeginToggleGroup("Frequency", myTarget.CustomFrequency);
            {
                EditorGUI.indentLevel++;
                myTarget.Frequency = EditorGUILayout.Slider(myTarget.Frequency, myTarget.FrequencyMin, myTarget.FrequencyMax);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomLacunarity = EditorGUILayout.BeginToggleGroup("Lacunarity", myTarget.CustomLacunarity);
            {
                EditorGUI.indentLevel++;
                myTarget.Lacunarity = EditorGUILayout.Slider(myTarget.Lacunarity, myTarget.LacunarityMin, myTarget.LacunarityMax);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomPersistence = EditorGUILayout.BeginToggleGroup("Persistence", myTarget.CustomPersistence);
            {
                EditorGUI.indentLevel++;
                myTarget.Persistence = EditorGUILayout.Slider(myTarget.Persistence, myTarget.PersistenceMin, myTarget.PersistenceMax);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomOctaves = EditorGUILayout.BeginToggleGroup("Octaves", myTarget.CustomOctaves);
            {
                EditorGUI.indentLevel++;
                myTarget.Octaves = EditorGUILayout.IntSlider(myTarget.Octaves, myTarget.OctavesMin, myTarget.OctavesMax);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
        }

        foldoutAppearance = EditorGUILayout.Foldout(foldoutAppearance, "Appearance Properties", true, EditorStyles.boldLabel);
        if (foldoutAppearance)
        {
            myTarget.CustomTint = EditorGUILayout.BeginToggleGroup("Tint", myTarget.CustomTint);
            {
                EditorGUI.indentLevel++;
                myTarget.Tint = EditorGUILayout.ColorField(myTarget.Tint);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomBrightness = EditorGUILayout.BeginToggleGroup("Brightness", myTarget.CustomBrightness);
            {
                EditorGUI.indentLevel++;
                myTarget.Brightness = EditorGUILayout.Slider(myTarget.Brightness, myTarget.BrightnessMin, myTarget.BrightnessMax);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            myTarget.Generate();
        }
        if (GUILayout.Button("Save To File"))
        {
            myTarget.SaveToFile();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}