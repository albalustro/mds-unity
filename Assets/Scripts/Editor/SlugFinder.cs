using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlugFinder : EditorWindow {

    static List<string> _sceneSlugs;

    [MenuItem("MDS/Scene Slug Finder")]
    static void Init()
    {
        SlugFinder window = (SlugFinder)GetWindow(typeof(SlugFinder));

        _sceneSlugs = new List<string>();
        string sceneName = SceneManager.GetActiveScene().name;

        string dialogPath = "Assets/Dialogue System/Resources/SO/" + sceneName.Substring(0, 4) + ".asset";

        DialogueList list = AssetDatabase.LoadAssetAtPath<DialogueList>(dialogPath);

        string game = sceneName.Substring(1, 1);
        string world = sceneName.Substring(3, 1);
        string episode = sceneName.Substring(5, 1);
        string challenge = sceneName.Substring(7, 1);

        _sceneSlugs = list.dialogueList.Where(i => i.episode == episode && i.minigame == challenge).Select(s => s.slug).ToList();

        window.Show();
    }

    void OnGUI()
    {

        GUILayout.BeginVertical();

        if(_sceneSlugs == null || _sceneSlugs.Count == 0)
        {
            GUILayout.Label("No slug found in this scene", EditorStyles.boldLabel);
        }
        else
        {
            foreach(var slug in _sceneSlugs)
            {
                GUILayout.Label(slug);

            }
        }
        GUILayout.EndVertical();
    }

   
 

}
