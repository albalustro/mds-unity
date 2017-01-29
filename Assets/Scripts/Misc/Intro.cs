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
            //yield return new WaitForSeconds(FadeTransition.Instance.BeginFade(FadeDirection.Out));
            FadeTransition.Instance.FadeTime = originalFadeTime;
            SceneLoader.Instance.LoadLogin();
        }

    }

}