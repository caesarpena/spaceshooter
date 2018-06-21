using UnityEngine;
using UnityEditor;

using Stellar_Sprites;

[CustomEditor(typeof(Station))]
public class StationEditor : Editor
{
    Station myTarget;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutNoise = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Station)target;
    }

    public override void OnInspectorGUI()
    {
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

            myTarget.CustomStationType = EditorGUILayout.BeginToggleGroup("Type", myTarget.CustomStationType);
            {
                EditorGUI.indentLevel++;
                myTarget.StationType = (SS_StationType)EditorGUILayout.EnumPopup(myTarget.StationType);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            if (myTarget.StationType == SS_StationType.Pod)
            {
                myTarget.CustomPods = EditorGUILayout.BeginToggleGroup("Pods", myTarget.CustomPods);
                {
                    int[] podCountOptions = new int[] { 2, 4, 6, 8 };
                    string[] podCountOptionsString = new string[] { "2", "4", "6", "8" };

                    EditorGUI.indentLevel++;
                    myTarget.NumberOfPods = EditorGUILayout.IntPopup(myTarget.NumberOfPods, podCountOptionsString, podCountOptions);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
            }
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