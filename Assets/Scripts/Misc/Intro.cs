using System.Collections;
using System.Collections.Generic;
using MDS.Core.SceneManagement;
using UnityEngine;

namespace MDS
{
    public class Intro : MDSBehaviour
    {

        IEnumerator Start()
        {
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
#else
            FadeTransition.Instance.BeginFade(FadeDirection.In);
            yield return new WaitForSeconds(3f);
#endif

            FadeTransition.Instance.FadeTime = originalFadeTime;
            SceneLoader.Instance.LoadLogin();
        }

    }

}