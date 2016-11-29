using UnityEngine;
using System.IO;

public class CreateObjectByJson : MonoBehaviour {

    public DialogueList result;
    public string pathToJson;

    void Start()
    {
        LoadFromJson(pathToJson);
    }

    void LoadFromJson(string path)
    {
        if (result.dialogueList.Count > 0)
            return;
        string json = File.ReadAllText(path);
        json = "{\"Dialogues\":" + json + "}";
        DialogueEntry[] dialogues = FromJson<DialogueEntry>(json);
        for (int i = 0; i < dialogues.Length; i++)
        {
            result.dialogueList.Add(dialogues[i]);
        }
            
    }

    public T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Dialogues;
    }

    private class Wrapper<T>
    {
        public T[] Dialogues;
    }
}
