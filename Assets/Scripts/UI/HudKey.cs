using System.Collections;
using System.Collections.Generic;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class HudKey : MonoBehaviour {

    public GameObject[] crystals;

    public CanvasGroup canvasGroup;

    void Start () {
        Scene curScene = SceneManager.GetActiveScene();
        if (curScene.IsChallenge() || curScene.IsEpisode())
        {
            canvasGroup.alpha = 1;
            int w = curScene.GetWorldIndex()-1;
            int e = curScene.GetEpisodeIndex() - 1;
            for(int i = 0; i < 5; i++)
            {
                ConceptTypes ct = ConceptTypes.CONCEPT_NOT_PLAYED;
                try
                {
                    ct = UserProfile.Instance.conceptMap.worlds[w].episodes[e].challenges[i].concept;
                }
                catch (System.Exception ex)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Game", "MDS" + curScene.GetGameIndex().ToString());
                    dic.Add("Plat", Application.platform.ToString());
                    dic.Add("User", UserProfile.Instance.login);
                    dic.Add("Conn", UserProfile.Instance.loginInfo.status.code.ToString());
                    dic.Add("Error", "Missing concept in conceptmap");
                    dic.Add("ErrorType", "HudKey");
                    Analytics.CustomEvent("Error", dic);
                    Analytics.FlushEvents();
                }
                bool active = ct == ConceptTypes.CONCEPT_GREEN;
                crystals[i].SetActive(active);
            }
        }
        else
        {
            canvasGroup.alpha = 0;
        }
        
	}
	

}
