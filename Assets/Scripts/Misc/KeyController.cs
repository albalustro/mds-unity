using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MDS.Utilities;

public class KeyController : MonoBehaviour {

	public GameObject keyPanel;
	public Text episodeTitle;
	public GameObject[] challengeCrystals;

	public void OpenKeyPanel(int episodeIndex)
	{
		//fade in
		keyPanel.SetActive (true);
		episodeTitle.text = SceneManager.GetActiveScene().GetEpisodeTitle(episodeIndex);
		SetupKey ();
		//seta o titulo do episodio
		//configura os cristais
	}

	public void CloseKeyPanel()
	{
		//fade out
		keyPanel.SetActive (false);
	}

	public void SetupKey()
	{
		
	}

}
