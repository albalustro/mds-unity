using UnityEngine;
using System.IO;

/// <summary>
/// Classe responsável pela 'desserialização' do json e preenchimento do array de diálogos
/// </summary>
public class LoadDataFromJson {

    public DialogueEntry[] LoadFromJson(string pathToJson)
    {
        string json = File.ReadAllText(pathToJson);
        json = "{\"Dialogues\":" + json + "}";
        DialogueEntry[] dialogues = FromJson<DialogueEntry>(json);
        return dialogues;
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
