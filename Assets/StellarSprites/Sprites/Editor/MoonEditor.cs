using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Moon))]
public class MoonEditor : Editor
{
    Moon myTarget;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutNoise = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Moon)target;
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
            myTarget.CustomColors = EditorGUILayout.BeginToggleGroup("Colors", myTarget.CustomColors);
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < myTarget.Colors.Length; i++)
                {
                    myTarget.Colors[i] = EditorGUILayout.ColorField(myTarget.Colors[i]);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

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
            myTarget.CustomColors = EditorGUILayout.BeginToggleGroup("Colors", myTarget.CustomColors);
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < myTarget.Colors.Length; i++)
                {
                    myTarget.Colors[i] = EditorGUILayout.ColorField(myTarget.Colors[i]);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomRoughness = EditorGUILayout.BeginToggleGroup("Roughness", myTarget.CustomRoughness);
            {
                EditorGUI.indentLevel++;
                myTarget.Roughness = EditorGUILayout.Slider(myTarget.Roughness, 0f, 1f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomLighting = EditorGUILayout.BeginToggleGroup("Lighting", myTarget.CustomLighting);
            {
                EditorGUI.indentLevel++;
                myTarget.LightAngle = EditorGUILayout.Slider(myTarget.LightAngle, 0f, 359f);
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