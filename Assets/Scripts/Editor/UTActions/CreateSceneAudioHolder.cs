using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AncientLightStudios.uTomate.API;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.DialogueSystem;
using System.IO;
using System.Linq;



public class CreateSceneAudioHolder : UTAction {

    public override IEnumerator Execute(UTContext context)
    {
        Scene currScene = SceneManager.GetActiveScene();

        if(currScene.IsChallenge() == false && currScene.IsEpisode() == false)
        {
            // EditorUtility.DisplayDialog("Erro", "Somente Challenge e Episode terão dialogos", "Blz.. entendi..");
            yield break;
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

    [MenuItem("Assets/Create/uTomate/MDS/CreateSoundClipHolder", false, 2000)]
    public static void AddAction()
    {
        Create<CreateSceneAudioHolder>();
    }

}
