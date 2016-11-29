using UnityEditor;

public class CreateDialogueList {

    [MenuItem("Assets/Create/Dialogue List")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<DialogueList>();
    }
}
