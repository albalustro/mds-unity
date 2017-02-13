using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AncientLightStudios.uTomate.API;
using MDS.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Editor.UTActions
{
    public class RemoveDebugObjFromAllScenes : UTAction
    {
        [MenuItem("Assets/Create/uTomate/MDS/REMOVE ALL Debug Objs from scene", false, 2000)]
        public static void AddAction()
        {
            Create<RemoveDebugObjFromAllScenes>();
        }

        public override IEnumerator Execute(UTContext context)
        {
            Scene curScene = SceneManager.GetActiveScene();

            if(curScene.IsEpisode() || curScene.IsChallenge())
            {
                var objs = GameObject.FindGameObjectWithTag("DEBUG");
                if(objs != null)
                {
                    DestroyImmediate(objs);
                }
            }

            yield return null;
        }
    }
}
