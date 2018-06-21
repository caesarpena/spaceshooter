using UnityEngine;
using UnityEditor;

using Stellar_Sprites;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet myTarget;
    int labelWidth = 120;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutNoise = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Planet)target;
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

            myTarget.CustomPlanetType = EditorGUILayout.BeginToggleGroup("Type", myTarget.CustomPlanetType);
            {
                EditorGUI.indentLevel++;
                myTarget.PlanetType = (SS_PlanetType)EditorGUILayout.EnumPopup(myTarget.PlanetType);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomColors = EditorGUILayout.BeginToggleGroup("Colors", myTarget.CustomColors);
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < myTarget.Colors.Length; i++)
                    myTarget.Colors[i] = EditorGUILayout.ColorField(myTarget.Colors[i]);
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
            myTarget.CustomAtmosphere = EditorGUILayout.BeginToggleGroup("Atmosphere", myTarget.CustomAtmosphere);
            {
                EditorGUI.indentLevel++;
                myTarget.Atmosphere = EditorGUILayout.ToggleLeft("Enable Atmosphere", myTarget.Atmosphere);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomOceans = EditorGUILayout.BeginToggleGroup("Oceans", myTarget.CustomOceans);
            {
                EditorGUI.indentLevel++;
                myTarget.Oceans = EditorGUILayout.ToggleLeft("Enable Oceans", myTarget.Oceans);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomClouds = EditorGUILayout.BeginToggleGroup("Clouds", myTarget.CustomClouds);
            {
                EditorGUI.indentLevel++;
                myTarget.Clouds = EditorGUILayout.BeginToggleGroup("Enable Clouds", myTarget.Clouds);
                {
                    EditorGUI.indentLevel++;
                    myTarget.CloudsRandomize = EditorGUILayout.BeginToggleGroup("Set Parameters", myTarget.CloudsRandomize);
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Density:", GUILayout.Width(labelWidth));
                            myTarget.CloudsDensity = EditorGUILayout.Slider(myTarget.CloudsDensity, 0f, 1f);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Transparency:", GUILayout.Width(labelWidth));
                            myTarget.CloudsTransparency = EditorGUILayout.Slider(myTarget.CloudsTransparency, 0f, 1f);
                        }
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndToggleGroup();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomCity = EditorGUILayout.BeginToggleGroup("City", myTarget.CustomCity);
            {
                EditorGUI.indentLevel++;
                myTarget.City = EditorGUILayout.BeginToggleGroup("Enable Cities", myTarget.City);
                {
                    EditorGUI.indentLevel++;
                    myTarget.CityRandomoize = EditorGUILayout.BeginToggleGroup("Set Parameters", myTarget.CityRandomoize);
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Density:", GUILayout.Width(labelWidth));
                            myTarget.CityDensity = EditorGUILayout.Slider(myTarget.CityDensity, myTarget.CityDensityMin, myTarget.CityDensityMax);
                        }
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndToggleGroup();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomRing = EditorGUILayout.BeginToggleGroup("Ring", myTarget.CustomRing);
            {
                EditorGUI.indentLevel++;
                myTarget.Ring = EditorGUILayout.BeginToggleGroup("Enable Ring", myTarget.Ring);
                {
                    EditorGUI.indentLevel++;
                    myTarget.RingRandomize = EditorGUILayout.BeginToggleGroup("Set Parameters", myTarget.RingRandomize);
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Detail:", GUILayout.Width(labelWidth));
                            myTarget.RingDetail = EditorGUILayout.Slider(myTarget.RingDetail, myTarget.RingDetailMin, myTarget.RingDetailMax);
                        }
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndToggleGroup();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
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

    //void StartGroup(ref bool property, string label)
    //{
    //    property = EditorGUILayout.BeginToggleGroup(label, property);
    //    EditorGUI.indentLevel++;
    //}

    //void EndGroup()
    //{
    //    EditorGUI.indentLevel--;
    //    EditorGUILayout.EndToggleGroup();
    //}
}