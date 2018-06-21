using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Asteroid))]
public class AsteroidEditor : Editor
{
    Asteroid myTarget;
    int labelWidth = 120;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Asteroid)target;
    }

    public override void OnInspectorGUI()
    {
        string[] availableSizes = new string[myTarget.AvailableSizes.Length];
        for (int i = 0; i < myTarget.AvailableSizes.Length; i++) availableSizes[i] = myTarget.AvailableSizes[i].ToString();

        string[] availableMineralColors = new string[myTarget.AvailableMineralColors.Length];
        for (int i = 0; i < myTarget.AvailableMineralColors.Length; i++) availableMineralColors[i] = myTarget.AvailableMineralColors[i].ToString();

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

        foldoutAppearance = EditorGUILayout.Foldout(foldoutAppearance, "Appearance Properties", true, EditorStyles.boldLabel);
        if (foldoutAppearance)
        {
            myTarget.CustomColors = EditorGUILayout.BeginToggleGroup("Colors", myTarget.CustomColors);
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < myTarget.Colors.Length; i++)
                    myTarget.Colors[i] = EditorGUILayout.ColorField(myTarget.Colors[i]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomMinerals = EditorGUILayout.BeginToggleGroup("Minerals", myTarget.CustomMinerals);
            {
                EditorGUI.indentLevel++;
                myTarget.Minerals = EditorGUILayout.BeginToggleGroup("Enable Minerals", myTarget.Minerals);
                {
                    EditorGUI.indentLevel++;
                    myTarget.MineralRandomize = EditorGUILayout.BeginToggleGroup("Set Parameters", myTarget.MineralRandomize);
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Color:", GUILayout.Width(labelWidth));
                            myTarget.MineralColor = EditorGUILayout.ColorField(myTarget.MineralColor);
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
}