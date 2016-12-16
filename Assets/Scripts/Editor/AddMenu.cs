using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddMenu : EditorWindow
{

    [MenuItem("MDS/Sound/Create")]
    public static void Create()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        string dialogPath = "Assets/Dialogue System/Resources/SO/" + sceneName.Substring(0, 4) + ".asset";

        DialogueList list = AssetDatabase.LoadAssetAtPath<DialogueList>(dialogPath);


        string game = sceneName.Substring(1, 1);
        string world = sceneName.Substring(3, 1);
        string episode = sceneName.Substring(5, 1);
        string challenge = sceneName.Substring(7, 1);

        List<string> sounds = list.dialogueList.Where(i => i.episode == episode && i.minigame == challenge).Select(s=>s.sound).ToList();

        string audioPath;
        foreach(var sound in sounds)
        {
            audioPath = "Assets/MDS " + game + "/Dialogue/World " + world + "/" + sound + ".mp3";
            if (File.Exists(audioPath))
            {
                Debug.Log(audioPath);
                GameObject go = new GameObject(sound);
                AudioSource a = go.AddComponent<AudioSource>();
                a.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);
            }
            else
            {
                Debug.LogError("Arquivo de audio não encontrado: " + audioPath);
            }
        }

       
    }

    [MenuItem("MDS/PlayerPref/Reset Playerprefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("MDS/Caching/Clean")]
    public static void ClearCaching()
    {
        Caching.CleanCache();
    }

    [MenuItem("MDS/Align Selection/Horizontal")]
    public static void HorizontalSpacer()
    {

        Transform[] transform = Selection.GetTransforms(SelectionMode.Unfiltered).OrderBy(t => t.position.x).ToArray() ;

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

		Transform[] transform = Selection.GetTransforms(SelectionMode.Unfiltered).OrderBy(t=>t.position.y).ToArray();

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
}
