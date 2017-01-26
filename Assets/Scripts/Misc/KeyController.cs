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
	public GameObject[] challengeCrystals;

	public void OpenKeyPanel(int episodeIndex)
	{
		keyPanel.gameObject.SetActive (true);
		//fade in
		LeanTween.alphaCanvas(keyPanel,1,0.3f).setOnComplete(() =>
		{
			clickBlocker.SetActive(true);
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
		});

	}

}
