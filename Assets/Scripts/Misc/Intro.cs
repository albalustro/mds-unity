using System.Collections;
using MDS.Core.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MDS
{
    public class Intro : MDSBehaviour
    {
        public GameObject textVersion;
        public Text debugText;
		public GameObject reporter;

        IEnumerator Start()
        {
			textVersion.GetComponent<Text>().text ="Version "+Application.version;

            float originalFadeTime = FadeTransition.Instance.FadeTime;
            FadeTransition.Instance.FadeTime = 1f;
            FadeTransition.Instance.BeginFade(FadeDirection.In);
            yield return new WaitForSeconds(3f);

            if(debugText == null)
            {
                Debug.Log("debug lost reference");
                var go = GameObject.Find("DebugText");
                if(go != null)
                {
                    debugText = go.GetComponent<Text>();
                }
            }

            if(Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.C))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("Buffer limpo");
                debugText.text += "[Buffer limpo]";
            }



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
            yield return new WaitForSeconds(3f);
            //yield return SceneLoader.Instance.WaitOBB();
#endif
            textVersion.SetActive(false);

            FadeTransition.Instance.FadeTime = originalFadeTime;
            SceneLoader.Instance.LoadLogin();
        }


        private int _inc = 0;
        public void IncDebugButton()
        {
            _inc++;
            if(_inc == 7)
            {
                //ConnectionManager.Instance.SetHomolgConfig();
                //debugText.text += "[Homolog Connection] ";
                //PlayerPrefs.DeleteAll();
                //Debug.Log("Buffer limpo");
                //debugText.text += "[Buffer limpo]";
				reporter.SetActive(true);
            }
            //else
            //    if (_inc <7)
            //        debugText.text = _inc.ToString();
        }
    }

}