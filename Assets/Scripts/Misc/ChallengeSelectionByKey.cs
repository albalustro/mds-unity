using System;
using System.Collections.Generic;
using UnityEngine;
using MDS.Utilities;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using MDS.Core.SceneManagement;

public class ChallengeSelectionByKey : MDSBehaviour
{
    [System.Serializable]
    public class ChallengeSelection
    {
		public GameObject go;
        public Vector3 uncompletedChallengeLocalPosition;
		public Vector3 completedChallengeLocalPosition;
		public Button button;

        
#if UNITY_EDITOR
        [FullInspector.InspectorButton]
        void CaptureCompletedPosition()
        {
			completedChallengeLocalPosition = go.transform.localPosition;
        }

        [FullInspector.InspectorButton]
        void CaptureUNCompletedPosition()
        {
			uncompletedChallengeLocalPosition = go.transform.localPosition;
        }

		[FullInspector.InspectorButton]
		void CaptureButtonComponent()
		{
			button = go.GetComponent<Button>();
		}
#endif

    }

    private bool _loadingScene = false;

    public IEnumerator ConfigureCrystalsOnKey(int episodeIndex)
	{
		int w, e;

		Scene scene = SceneManager.GetActiveScene ();
		w = scene.GetWorldIndex() - 1;

        bool directAccess = UserProfile.Instance.conceptMap.worlds[w].episodes[episodeIndex - 1].CheckDirectAccessToChallenge();

        foreach(var item in challengeCrystalList)
        {
            if(directAccess)
            {
                item.Value.button.onClick.AddListener(() => loadChallengeOnClick(item.Key + 1, episodeIndex));
                item.Value.button.interactable = true;
            }
            else
                item.Value.button.interactable = false;

            if(UserProfile.Instance.conceptMap.worlds[w].episodes[episodeIndex - 1].CheckChallengeComplete(item.Key))
            {
				//coloca na posição correta (aberto ou fechado)
				item.Value.go.GetComponent<RectTransform>().localPosition = item.Value.completedChallengeLocalPosition;
			} 
			else
			{
				//coloca na posição correta (aberto ou fechado)
				item.Value.go.GetComponent<RectTransform>().localPosition = item.Value.uncompletedChallengeLocalPosition;
			}
			//ativa o objeto
			item.Value.go.SetActive (true);
			//faz o fade in e a escala com tween e delay para o proximo
			LeanTween.alpha (item.Value.go, 0, 0.8f);
			LeanTween.scale (item.Value.go, Vector3.one, 0.8f).setEase (LeanTweenType.easeOutElastic);
			yield return new WaitForSeconds (0.3f);
		}
	}

    private void loadChallengeOnClick(int challengeIndex, int episodeIndex)
    {
        if(_loadingScene) return;
        _loadingScene = true;
        SceneLoader.Instance.LoadChallenge(challengeIndex, episodeIndex);
    }

    [FullInspector.ShowInInspector]
    public Dictionary<int, ChallengeSelection> challengeCrystalList;
		
	public void UnSetCrystals()
	{
		foreach (var item in challengeCrystalList)
		{
			item.Value.go.SetActive (false);
			LeanTween.alpha (item.Value.go, 0, 0.5f);
			LeanTween.scale(item.Value.go, Vector3.zero, 0f);
		}
	}
}
