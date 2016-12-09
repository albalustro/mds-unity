using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;
using UnityEngine.SceneManagement;

namespace MDS.Actions
{
    public class LoadSceneAction : BaseAction
    {

        public string sceneName { get; set; }

        public override IEnumerator Execute()
        {
            yield return base.Execute();

            DownloadManager.Instance.DownloadScene(sceneName);

        }
    }
}