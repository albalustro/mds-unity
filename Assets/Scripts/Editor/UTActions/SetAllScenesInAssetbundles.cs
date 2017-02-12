using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AncientLightStudios.uTomate.API;
using MDS.Utilities;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace MDS.Editor.UTActions
{
    public class ClearAllAssetbundles : UTAction
    {
        [MenuItem("Assets/Create/uTomate/MDS/CLEAR ALL Scenes Assetbundles", false, 2000)]
        public static void AddAction()
        {
            Create<ClearAllAssetbundles>();
        }

        public override IEnumerator Execute(UTContext context)
        {
            string[] abNames = AssetDatabase.GetAllAssetBundleNames();

            for(int i = 0; i < abNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(abNames[i], true);
            }

            yield return null;
        }
    }

    public class SetAllScenesInAssetbundles : UTAction
    {

        [MenuItem("Assets/Create/uTomate/MDS/Fix ALL Scenes Assetbundles", false, 2000)]
        public static void AddAction()
        {
            Create<SetAllScenesInAssetbundles>();
        }

        public override IEnumerator Execute(UTContext context)
        {
            Scene curScene = SceneManager.GetActiveScene();

            if (curScene.IsEpisode() || curScene.IsChallenge())
            {
                string assetBundleName = string.Format("g{0}w{1}e{2}", curScene.GetGameIndex(),
                                                                       curScene.GetWorldIndex(),
                                                                       curScene.GetEpisodeIndex());

                
                AssetImporter.GetAtPath(curScene.path).SetAssetBundleNameAndVariant(assetBundleName, "");
            }

            yield return null;
        }
    }
}
