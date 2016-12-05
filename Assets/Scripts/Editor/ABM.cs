using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ABM : EditorWindow {

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Window/Asset Bundles Manager")]
    static void Init()
    {
        ABM window = (ABM)GetWindow(typeof(ABM));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.BeginVertical();
        var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach(var ab in assetBundleNames)
        {
            var bundles = AssetDatabase.GetAssetPathsFromAssetBundle(ab);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label(ab, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Filter"))
            {
                
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach(var b in bundles)
            {
                IndentedLabel(b, 1, () =>
                {
                    if(GUILayout.Button("Remove"))
                    {
                        AssetImporter.GetAtPath(b).SetAssetBundleNameAndVariant(string.Empty, string.Empty);
                    }
                    GUILayout.FlexibleSpace();
                });

            }
            var dependencias = AssetDatabase.GetAssetBundleDependencies(ab, true);
            if(dependencias.Length == 0)
            {
                IndentedLabel("-- Empty --", 2);
            }
            else
            {
                foreach(var d in dependencias)
                {

                    IndentedLabel(d, 2);
                }
            }
        }

        GUILayout.EndVertical();
    }

    private void IndentedLabel(string label, int indentLevel, Action func = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 20);
        GUILayout.Label(label);
        if(func != null)
            func();
        GUILayout.EndHorizontal();
    }
 

}
