using UnityEngine;
using UnityEditor;

using Stellar_Sprites;

[CustomEditor(typeof(Ship))]
public class ShipEditor : Editor
{
    Ship myTarget;

    private static bool foldoutDefault = false;
    bool foldoutGeneral = foldoutDefault;
    bool foldoutAppearance = foldoutDefault;
    bool foldoutPhysics = foldoutDefault;

    public void OnEnable()
    {
        myTarget = (Ship)target;
    }

    public override void OnInspectorGUI()
    {
        string[] availableLengths = new string[myTarget.AvailableBodyLengths.Length];
        for (int i = 0; i < myTarget.AvailableBodyLengths.Length; i++) availableLengths[i] = myTarget.AvailableBodyLengths[i].ToString();

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


            myTarget.CustomShipType = EditorGUILayout.BeginToggleGroup("Type", myTarget.CustomShipType);
            {
                EditorGUI.indentLevel++;
                myTarget.ShipType = (SS_ShipType)EditorGUILayout.EnumPopup(myTarget.ShipType);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
        }

        foldoutAppearance = EditorGUILayout.Foldout(foldoutAppearance, "Appearance Properties", true, EditorStyles.boldLabel);
        if (foldoutAppearance)
        {
            myTarget.CustomColorBody = EditorGUILayout.BeginToggleGroup("Main Color", myTarget.CustomColorBody);
            {
                EditorGUI.indentLevel++;
                myTarget.Colors[0] = EditorGUILayout.ColorField(myTarget.Colors[0]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomColorHighlight = EditorGUILayout.BeginToggleGroup("Highlight Color", myTarget.CustomColorHighlight);
            {
                EditorGUI.indentLevel++;
                myTarget.Colors[1] = EditorGUILayout.ColorField(myTarget.Colors[1]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomColorEngine = EditorGUILayout.BeginToggleGroup("Engine Color", myTarget.CustomColorEngine);
            {
                EditorGUI.indentLevel++;
                myTarget.Colors[2] = EditorGUILayout.ColorField(myTarget.Colors[2]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomColorDetail = EditorGUILayout.BeginToggleGroup("Highlight Noise", myTarget.CustomColorDetail);
            {
                EditorGUI.indentLevel++;
                myTarget.ColorDetail = EditorGUILayout.Slider(myTarget.ColorDetail, 0.075f, 0.15f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomBodyLength = EditorGUILayout.BeginToggleGroup("Body Length", myTarget.CustomBodyLength);
            {
                EditorGUI.indentLevel++;
                myTarget.BodyLength = EditorGUILayout.IntPopup(myTarget.BodyLength, availableLengths, myTarget.AvailableBodyLengths);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomBodyDetail = EditorGUILayout.BeginToggleGroup("Body Noise", myTarget.CustomBodyDetail);
            {
                EditorGUI.indentLevel++;
                myTarget.BodyDetail = EditorGUILayout.Slider(myTarget.BodyDetail, 0.01f, 0.1f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();

            myTarget.CustomWingDetail = EditorGUILayout.BeginToggleGroup("Wing Noise", myTarget.CustomWingDetail);
            {
                EditorGUI.indentLevel++;
                myTarget.WingDetail = EditorGUILayout.Slider(myTarget.WingDetail, 0.01f, 0.1f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndToggleGroup();
        }

        foldoutPhysics = EditorGUILayout.Foldout(foldoutPhysics, "Physics Properties", true, EditorStyles.boldLabel);
        if (foldoutPhysics)
        {
            GUILayout.BeginHorizontal();
            {
                myTarget.PolygonCollider = EditorGUILayout.ToggleLeft("Polygon Collider", myTarget.PolygonCollider);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                myTarget.EngineExhaust = EditorGUILayout.ToggleLeft("Engine Points", myTarget.EngineExhaust);
            }
            GUILayout.EndHorizontal();
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