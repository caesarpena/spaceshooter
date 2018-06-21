using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sun))]
public class SunEditor : Editor
{
    Sun myTarget;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutNoise = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Sun)target;
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

        foldoutAppearance = EditorGUILayout.Foldout(foldoutAppearance, "Appearance Properties", true, EditorStyles.boldLabel);
        if (foldoutAppearance)
        {
            myTarget.CustomColor = EditorGUILayout.BeginToggleGroup("Color", myTarget.CustomColor);
            {
                EditorGUI.indentLevel++;
                myTarget.MainColor = EditorGUILayout.ColorField(myTarget.MainColor);
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