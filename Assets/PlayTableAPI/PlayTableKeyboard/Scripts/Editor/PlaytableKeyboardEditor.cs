using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Playmove
{
    public class PlaytableKeyboardEditor : Editor
    {
        [MenuItem("Assets/Create/PlayTableAPI/Create Keyboard Skin")]
        public static void CreateSkin()
        {
            string currentFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
            CreateSkin("KeyboardDefaultSkin", currentFolder);
        }

        public static PKKeyboardSkin CreateSkin(string name, string path)
        {
            return ScriptableUtilities.CreateAsset<PKKeyboardSkin>(name, path);
        }
    }
}