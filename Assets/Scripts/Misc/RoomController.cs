using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomController : MDSBehaviour {

	[SerializeField]
	private AudioClip _theme;
	[SerializeField]
	private GameObject _hotlinks;
	[SerializeField]
	private GameObject _charSelectionMask;
	[SerializeField]
	private GameObject _worldSelectionMask;
	[SerializeField]
	private GameObject nextButton;
	[SerializeField]
	private GameObject backButton;

	void Start () {
		AudioController.Instance.PlayTheme (_theme);
	}

	public void ChangeScene(int scene)
	{
		switch (scene)
		{
		case 1:
			Log ("Carregando mundo " + scene);
			break;
		case 2:
			Log ("Carregando mundo " + scene);
			break;
		case 3:
			Log ("Carregando mundo " + scene);
			break;
		case 4:
			Log ("Carregando mundo " + scene);
			break;
		default:
			LogWarning ("Cena de mundo nao encontrado.");
			break;
		}
	}

	public void NextButton()
	{
		if (_charSelectionMask.activeSelf)
		{
			_charSelectionMask.SetActive (false);
			_worldSelectionMask.SetActive (true);
			nextButton.SetActive (false);
			_hotlinks.SetActive (true);
			backButton.SetActive (true);
		} 
	}

	public void BackButton()
	{
		if (_worldSelectionMask.activeSelf)
		{
			_worldSelectionMask.SetActive (false);
			_hotlinks.SetActive (false);
			backButton.SetActive (false);
			_charSelectionMask.SetActive (true);
			nextButton.SetActive (true);
		}
	}
}
