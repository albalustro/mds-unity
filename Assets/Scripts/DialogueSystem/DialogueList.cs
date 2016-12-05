using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "NewFile", menuName = "Dialogue System/Create List", order = 1)]
public class DialogueList : ScriptableObject
{

    public List<DialogueEntry> dialogueList;
}