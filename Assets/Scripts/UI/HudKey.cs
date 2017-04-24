using System.Collections;
using System.Collections.Generic;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                bool active = UserProfile.Instance.conceptMap.worlds[w].episodes[e].challenges[i].concept == ConceptTypes.CONCEPT_GREEN;
                crystals[i].SetActive(active);
            }
        }
        else
        {
            canvasGroup.alpha = 0;
        }
        
	}
	

}
