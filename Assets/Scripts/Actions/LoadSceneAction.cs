using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;
using UnityEngine.SceneManagement;

namespace MDS.Actions
{
    public class LoadSceneAction : IAction
    {

        public string sceneName { get; set; }

        public IEnumerator Execute(Action callback = null)
        {
            DownloadManager.Instance.DownloadScene(sceneName);
            yield return null;
        }
    }
}