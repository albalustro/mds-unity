using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MDS.Utilities;

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
	[SerializeField]
	private GameObject closeMaskButton;
	[SerializeField]
	private GameObject hudCanvas;
	private string currentGameIndex;

	protected override void Awake ()
	{
		base.Awake ();
		hudCanvas.SetActive (true);
	}

	void Start()
	{
		_charSelectionMask.SetActive (true);
		Scene scene = SceneManager.GetActiveScene ();
		if (scene.IsRoom ())
		{
			nextButton.SetActive (true);
			closeMaskButton.SetActive (true);
		}
        currentGameIndex = SceneManager.GetActiveScene().GetGameIndex().ToString();
		AudioController.Instance.PlayTheme (_theme);
	}
		
	public void ChangeScene(int scene)
	{
		closeMaskButton.SetActive (false);
		nextButton.SetActive (false);
		backButton.SetActive (false);
		_charSelectionMask.SetActive (false);
		_worldSelectionMask.SetActive (false);
		switch (scene)
		{
		case 1:
			SceneManager.LoadScene ("G" + currentGameIndex + "W1EpisodeMap", LoadSceneMode.Single);
			break;
		case 2:
			SceneManager.LoadScene ("G" + currentGameIndex + "W2EpisodeMap", LoadSceneMode.Single);
			break;
		case 3:
			SceneManager.LoadScene ("G" + currentGameIndex + "W3EpisodeMap", LoadSceneMode.Single);
			break;
		case 4:
			SceneManager.LoadScene ("G" + currentGameIndex + "W4EpisodeMap", LoadSceneMode.Single);
			break;
		default:
			LogWarning ("Cena de mundo nao encontrado.");
			break;
		}
	}

    public void NextButton()
    {
        nextButton.SetActive(false);
        _hotlinks.SetActive(true);
        backButton.SetActive(true);
        _charSelectionMask.SetActive(false);
        _worldSelectionMask.SetActive(true);
    }

    public void BackButton()
    {
        backButton.SetActive(false);
        _hotlinks.SetActive(false);
        nextButton.SetActive(true);
        _worldSelectionMask.SetActive(false);
        _charSelectionMask.SetActive(true);
    }

    public void CloseMaskButton()
    {
        _hotlinks.SetActive(true);
        backButton.SetActive(false);
        nextButton.SetActive(false);
        closeMaskButton.SetActive(false);
        _charSelectionMask.SetActive(false);
        _worldSelectionMask.SetActive(false);
    }

    public void RestartMask()
    {
        backButton.SetActive(true);
        closeMaskButton.SetActive(true);
        _worldSelectionMask.SetActive(true);

    }
}
