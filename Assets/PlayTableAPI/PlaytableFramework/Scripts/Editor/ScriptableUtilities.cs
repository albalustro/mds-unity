using UnityEngine;
using UnityEditor;
using System.IO;

    namespace Playmove { public class ScriptableUtilities : Editor
    {
        public static T CreateAsset<T>(string assetName, string pathToSave) where T : ScriptableObject
        {
            Selection.activeObject = null;

            if (pathToSave[pathToSave.Length - 1] != '/')
                pathToSave += "/";

            // If we already have a saved object in the assets folder
            T asset = (T)AssetDatabase.LoadAssetAtPath(pathToSave + assetName + ".asset", typeof(T));
            if (asset != null)
            {
                return asset;
            }

            asset = CreateInstance<T>();
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(pathToSave + assetName + ".asset");
            Debug.Log(assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            EditorUtility.SetDirty(asset);

            return asset;
        }
    }
}