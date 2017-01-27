using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsHUDController : MonoBehaviour {

	private GameObject _backBtn;
	private GameObject _fullScreenBtn;
	private GameObject _quitGameBtn;
	private GameObject _restoreMaskBtn;
	private GameObject _tutorialBtn;
	private RectTransform _transformOptionsHUDPanel;
	private bool _optionsHUDPanelOn;

	void Awake()
	{
		_transformOptionsHUDPanel = GameObject.Find ("OptionsHUDPanel").GetComponent<RectTransform> ();
		_optionsHUDPanelOn = false;
		_backBtn = GameObject.Find ("BackHUDBtn");
		_fullScreenBtn = GameObject.Find ("FullScreenHUDBtn");
		_quitGameBtn = GameObject.Find ("QuitGameHUDBtn");
		_restoreMaskBtn = GameObject.Find ("RestoreMaskHUDBtn");
		_tutorialBtn = GameObject.Find ("TutorialHUDBtn");

		//Application.platform = RuntimePlatform. 

		#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
			_fullScreenBtn.SetActive (false);
			_quitGameBtn.SetActive(false);
		#endif


	}


	public void UnMute()
	{
		AudioController.Instance.UnMute ();
	}

	public void Mute()
	{
		AudioController.Instance.Mute ();
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
}
