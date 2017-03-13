using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;

namespace MDS
{
    public class Intro : MDSBehaviour
    {
        public GameObject textVersion;
        public bool byPassOBB;

        IEnumerator Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Log("Inicio");
            SceneLoader.Instance.LoadOBB();
#endif
            float originalFadeTime = FadeTransition.Instance.FadeTime;
            FadeTransition.Instance.FadeTime = 1f;
            FadeTransition.Instance.BeginFade(FadeDirection.In);
            yield return new WaitForSeconds(3f);

#if UNITY_WEBGL
            //Color originalFadeColor = FadeTransition.Instance.FadeColor;
            //FadeTransition.Instance.FadeColor = Color.white;
            //Camera.main.backgroundColor = Color.white;
            yield return new WaitForSeconds(FadeTransition.Instance.BeginFade(FadeDirection.Out));
            FadeTransition.Instance.BeginFade(FadeDirection.In);
            yield return SceneLoader.Instance.DownloadInitialAssetbundles();
            //FadeTransition.Instance.FadeColor = originalFadeColor;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            // FadeTransition.Instance.BeginFade(FadeDirection.In);
            // yield return new WaitForSeconds(3f);
            if (byPassOBB==false)
                yield return SceneLoader.Instance.WaitOBB();
#endif

            textVersion.SetActive(false);

            FadeTransition.Instance.FadeTime = originalFadeTime;
            SceneLoader.Instance.LoadLogin();
        }

    }

}