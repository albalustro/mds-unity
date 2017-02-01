using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.Core.SceneManagement;

public class OptionsHUDController : MonoBehaviour {

	private GameObject _backBtn;
	private GameObject _fullScreenBtn;
	private GameObject _quitGameBtn;
	private GameObject _restoreMaskBtn;
	private GameObject _tutorialBtn;
	private RectTransform _transformOptionsHUDPanel;
	private bool _optionsHUDPanelOn;

	/*
	 TODO
	  - Acho que falta só o tutorial 
	 */

	void Awake()
	{
		_transformOptionsHUDPanel = GameObject.Find ("OptionsHUDPanel").GetComponent<RectTransform> ();
		_optionsHUDPanelOn = false;
		_backBtn = GameObject.Find ("BackHUDBtn");
		_fullScreenBtn = GameObject.Find ("FullScreenHUDBtn");
		_quitGameBtn = GameObject.Find ("QuitGameHUDBtn");
		//_restoreMaskBtn = GameObject.Find ("RestoreMaskHUDBtn");
		_tutorialBtn = GameObject.Find ("TutorialHUDBtn");

		#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
			_fullScreenBtn.SetActive (false);
			_quitGameBtn.SetActive(false);
		#endif

		Scene scene = SceneManager.GetActiveScene ();
		if (scene.IsRoom ())
		{
			_backBtn.SetActive (false);
		}
		else
		{
			//_restoreMaskBtn.SetActive (false);
			if (scene.IsChallenge())
				_tutorialBtn.SetActive (false);
		}
	}
		
	public void UnMute()
	{
		AudioController.Instance.UnMute ();
	}

	public void Mute()
	{
		AudioController.Instance.Mute ();
	}

	public void QuitGame()
	{
        SceneLoader.Instance.Quit();
	}

	//Tela cheia (WebGL e Desktop)
	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	public void TogglePanel()
	{
		if (_optionsHUDPanelOn)
		{
			LeanTween.move (_transformOptionsHUDPanel, new Vector3 (-35, -10, 0), 0.7f).setEase(LeanTweenType.easeInQuint);
			_optionsHUDPanelOn = false;
		} 
		else
		{
			float w = _transformOptionsHUDPanel.rect.width;
			LeanTween.move (_transformOptionsHUDPanel, new Vector3(-w, -10, 0), 0.7f).setEase(LeanTweenType.easeInQuint);
			_optionsHUDPanelOn = true;
		}
	}

	public void BackHUDBtn()
	{
		//se está na cena de quarto, icone desativado = Feito no awake

		Scene scene = SceneManager.GetActiveScene ();
		if (scene.IsMap())
		{
			//se está na cena de mapa, volta para o quarto
			SceneLoader.Instance.LoadRoomScene ();
		} 
		else if (scene.IsEpisode() || scene.IsChallenge())
		{
			//se está na cena episodio OU desafio, volta para o mapa
			SceneLoader.Instance.LoadMapScene();
		}

	}
}
