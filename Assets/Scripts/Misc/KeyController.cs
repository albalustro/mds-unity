using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MDS.Utilities;

public class KeyController : MonoBehaviour {

	public CanvasGroup keyPanel;
	public Text episodeTitle;
	public GameObject clickBlocker;
	public ChallengeSelectionByKey challengeSelectionByKey;

	void Start()
	{
		keyPanel.transform.GetComponent<Canvas> ().worldCamera = Camera.main;
	}

	public void OpenKeyPanel(int episodeIndex)
	{
		keyPanel.gameObject.SetActive (true);
		//fade in
		LeanTween.alphaCanvas(keyPanel,1,0.3f).setOnComplete(() =>
		{
			clickBlocker.SetActive(true);
			StartCoroutine (challengeSelectionByKey.ConfigureCrystalsOnKey(episodeIndex));
		});
		episodeTitle.text = SceneManager.GetActiveScene().GetEpisodeTitle(episodeIndex);	//seta o titulo do episodio
		//configura os cristais
	}

	public void CloseKeyPanel()
	{
		//fade out
		LeanTween.alphaCanvas(keyPanel,0,0.3f).setOnComplete(() =>
		{
			clickBlocker.SetActive(true);
			keyPanel.gameObject.SetActive (false);
			challengeSelectionByKey.UnSetCrystals();
		});

	}

}
