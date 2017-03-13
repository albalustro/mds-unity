using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.DialogueSystem;
using System;
using UnityEditor.SceneManagement;



public class AddMenu : EditorWindow
{
    #region SOUND
    [MenuItem("MDS/Sound/Create")]
    public static void Create()
    {
        Scene currScene = SceneManager.GetActiveScene();

        if(currScene.IsChallenge() == false && currScene.IsEpisode() == false)
        {
            EditorUtility.DisplayDialog("Erro", "Somente Challenge e Episode terão dialogos", "Blz.. entendi..");
            return;
        }

        List<string> soundList = GetSounds(currScene);

        AudioClipHolder audioHolder = GetAudioHolder();

        string game = currScene.GetGameIndex().ToString();
        string world = currScene.GetWorldIndex().ToString();
        string audioPath;

        foreach(var sound in soundList)
        {
            audioPath = "Assets/MDS" + game + "/Dialogue/World" + world + "/" + sound + ".mp3";
            if(File.Exists(audioPath))
                audioHolder.Add(sound, AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath));
            else
                Debug.LogError("Arquivo de audio não encontrado: " + audioPath);
        }


    }

    private static List<string> GetSounds(Scene currScene)
    {
        string sceneName = currScene.name;

        string dialogPath = "Assets/DialogueSystem/Resources/SO/" + sceneName.Substring(0, 4) + ".asset";

        DialogueList list = AssetDatabase.LoadAssetAtPath<DialogueList>(dialogPath);


        string game = currScene.GetGameIndex().ToString();
        string world = currScene.GetWorldIndex().ToString();
        string episode = currScene.GetEpisodeIndex().ToString();
        string challenge = "";
        if(currScene.IsChallenge())
        {
            challenge = currScene.GetChallengeIndex().ToString();
        }
        return list.dialogueList
                            .Where(i => i.episode == episode && i.minigame == challenge)
                            .Select(s => s.sound)
                            .ToList();
    }

    private static AudioClipHolder GetAudioHolder()
    {
        AudioClipHolder ret = GameObject.FindObjectOfType<AudioClipHolder>();
        if(ret == null)
        {
            GameObject go = new GameObject("_AUDIO HOLDER_");
            ret = go.AddComponent<AudioClipHolder>();
        }
        return ret;
    }
    #endregion

    #region PLAYERPREF
    [MenuItem("MDS/Tools/PlayerPref/Reset Playerprefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region CACHING
    [MenuItem("MDS/Tools/Caching/Clean")]
    public static void ClearCaching()
    {
        Caching.CleanCache();
    }
    #endregion

    #region V/H ALIGN

    [MenuItem("MDS/Align Selection/Horizontal")]
    public static void HorizontalSpacer()
    {

        Transform[] transform = Selection.GetTransforms(SelectionMode.Unfiltered).OrderBy(t => t.position.x).ToArray();

        Vector3 first = transform[0].localPosition;
        Vector3 last = transform[(transform.Length - 1)].localPosition;

        int max = transform.Length - 1;
        float step = (last.x - first.x) / max;

        for(int i = 1; i < max; i++)
        {
            Vector3 cur = transform[i].localPosition;
            cur.x = first.x + i * step;
            transform[i].localPosition = cur;
        }


    }

    [MenuItem("MDS/Align Selection/Vertical")]
    public static void VerticalSpacer()
    {

        Transform[] transform = Selection.GetTransforms(SelectionMode.Unfiltered).OrderBy(t => t.position.y).ToArray();

        Vector3 first = transform[0].localPosition;
        Vector3 last = transform[(transform.Length - 1)].localPosition;

        int max = transform.Length - 1;
        float step = (last.y - first.y) / max;

        for(int i = 0; i < max; i++)
        {
            Vector3 cur = transform[i].localPosition;
            cur.y = first.y + i * step;
            transform[i].localPosition = cur;
        }


    }
    #endregion

    #region RUN F5
    [MenuItem("MDS/Run _F5")]
    public static void RunFromSplash()
    {
        string loadSceneName = EditorSceneManager.GetActiveScene().path;
        PlayerPrefs.SetString("_loadSceneName", loadSceneName);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        
        EditorSceneManager.OpenScene("Assets/Common/Scenes/splash.unity");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
    #endregion

    #region PREFABS ESSENTIALS
    [MenuItem("MDS/Tools/Revert ESSENTIALS to Prefab")]
    static void Revert()
    {

        string scenePath;
        
        for(int g = 1; g < 4; g++)
        {
            scenePath = string.Format("Assets/MDS{0}/Scenes/G{0}Login.unity", g);
            RevertEssentialToPrefabAt(scenePath);

            scenePath = string.Format("Assets/MDS{0}/Scenes/G{0}Room.unity", g);
            RevertEssentialToPrefabAt(scenePath);

            for(int w = 1; w < 5; w++)
            {
                scenePath = string.Format("Assets/MDS{0}/Scenes/G{0}W{1}EpisodeMap.unity", g, w);
                RevertEssentialToPrefabAt(scenePath);

                for(int e = 1; e < 9; e++)
                {
                    scenePath = string.Format("Assets/MDS{0}/Scenes/World{1}/Episode{2}/G{0}W{1}E{2}.unity", g, w, e);
                    RevertEssentialToPrefabAt(scenePath);

                    for(int c = 1; c < 6; c++)
                    {
                        scenePath = string.Format("Assets/MDS{0}/Scenes/World{1}/Episode{2}/G{0}W{1}E{2}C{3}.unity", g, w, e, c);
                        RevertEssentialToPrefabAt(scenePath);
                    }
                }
            }
        }


        var selection = Selection.gameObjects;

        if(selection.Length > 0)
        {
            for(var i = 0; i < selection.Length; i++)
            {
                PrefabUtility.RevertPrefabInstance(selection[i]);
            }
        }
       
    }
 
    private static void RevertEssentialToPrefabAt(string scenePath)
    {
        GameObject essentialsInstance;
        Scene curScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        essentialsInstance = GameObject.Find("Essentials");
        if(essentialsInstance == null)
        {
            Debug.LogError(scenePath + " nao tem Essentials");
            return;
        }
        if (PrefabUtility.RevertPrefabInstance(essentialsInstance)==false)
        {
            Debug.LogError(scenePath + " nao reverteu o Essentials");
            return;
        }
        EditorSceneManager.SaveScene(curScene);
    }
    #endregion


    #region Texture import settings
    [MenuItem("MDS/Textures/Fix import settings")]
    public static void FixTexturesImportSettings()
    {
        var selectedList = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Unfiltered);
        foreach(var item in selectedList)
        {
            Texture2D tex = (Texture2D)item;
            string path = AssetDatabase.GetAssetPath(item.GetInstanceID());
            if (tex.width%4==0 && tex.height%4==0)
                Debug.Log("[OK] -> " + path);
            else
                Debug.LogError("[***] -> " + path);


        }
    }
    #endregion
}

[InitializeOnLoad]
public static class SceneModeTracker
{
    

    static SceneModeTracker()
    {
        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged()
    {
        if(!EditorApplication.isPlayingOrWillChangePlaymode &&
             !EditorApplication.isPlaying)
        {
            string loadSceneName = PlayerPrefs.GetString("_loadSceneName", "");
            if(!string.IsNullOrEmpty(loadSceneName))
            {
                EditorSceneManager.OpenScene(loadSceneName);
                PlayerPrefs.DeleteKey("_loadSceneName");
            }
        }
    }
}