using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using MDS.Core.SceneManagement;
using MDS.DialogueSystem;
using System.Collections;

public class OptionsHUDController : MonoBehaviour
{
    private GameObject _audioOnHUDBtn;
    private GameObject _backBtn;
    private GameObject _fullScreenBtn;
    private GameObject _quitGameBtn;
    private GameObject _tutorialBtn;                        //Tutorial MDS
    private GameObject _tutorialPlayMoveHUDBtn;             //Painel de Informações Playmove
    private GameObject _placarBtn;                          //Botão para o placar (exclusividade Playmove)
    //  private GameObject _restoreMaskBtn;
    private RectTransform _transformOptionsHUDPanel;
    private bool _optionsHUDPanelOn;

    void Awake()
    {
        _transformOptionsHUDPanel = GameObject.Find("OptionsHUDPanel").GetComponent<RectTransform>();
        _optionsHUDPanelOn = false;
        _backBtn = GameObject.Find("BackHUDBtn");
        _fullScreenBtn = GameObject.Find("FullScreenHUDBtn");
        _quitGameBtn = GameObject.Find("QuitGameHUDBtn");
        //  _restoreMaskBtn = GameObject.Find("RestoreMaskHUDBtn");
        _tutorialBtn = GameObject.Find("TutorialHUDBtn");

        _audioOnHUDBtn = GameObject.Find("AudioOnHUDBtn");

        //PLAYMOVE
        _tutorialPlayMoveHUDBtn = GameObject.Find("TutorialPlayMoveHUDBtn");
        _placarBtn = GameObject.Find("PlacarHUDBtn");
        

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
        _fullScreenBtn.SetActive(false);
#endif

#if UNITY_WEBGL
        _quitGameBtn.SetActive(false);
#endif

#if PLAY_MOVE

#endif

        Scene scene = SceneManager.GetActiveScene();

#if !PLAY_MOVE
        if (scene.IsLogin())
        {
            gameObject.SetActive(false);
            return;
        }

        if(scene.IsRoom())
        {
            _backBtn.SetActive(false);
         //   _restoreMaskBtn.SetActive(true);
        }
        else
        {
        //    _restoreMaskBtn.SetActive(false);
        }

        if(scene.IsRoom() || scene.IsMap())
            _tutorialBtn.SetActive(true);
        else
            _tutorialBtn.SetActive(false);
#else
        gameObject.SetActive(true);
        _placarBtn.SetActive(true);
        _tutorialBtn.SetActive(false);
        _backBtn.SetActive(false);
        _fullScreenBtn.SetActive(false);
        _quitGameBtn.SetActive(false);
        _audioOnHUDBtn.SetActive(false);
#endif
    }

//#if PLAY_MOVE
//    IEnumerator Start()
//    {
//        yield return new WaitForEndOfFrame();
//        TogglePanel();
//    }
//#endif

    public void UnMute()
    {
        AudioController.Instance.UnMute();
    }

    public void Mute()
    {
        AudioController.Instance.Mute();
    }

    public void QuitGame()
    {
        SceneLoader.Instance.Quit();
    }

    //Tela cheia (WebGL e Desktop)
    public void FullScreen()
    {
        //Debug.Log("Clicado no botão fullscreen. Deveria fazer alguma coisa. Status atual do FS: " + Screen.fullScreen);
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void TogglePanel()
    {

        if(_optionsHUDPanelOn)
        {
            LeanTween.move(_transformOptionsHUDPanel, new Vector3(-6, -10, 0), 0.4f).setEase(LeanTweenType.easeInQuart);
            _optionsHUDPanelOn = false;
        }
        else
        {
            float w = _transformOptionsHUDPanel.rect.width;
            LeanTween.move(_transformOptionsHUDPanel, new Vector3(-w, -10, 0), 0.4f).setEase(LeanTweenType.easeInQuart);
            _optionsHUDPanelOn = true;
        }
    }

    private bool _backing = false;
    public void BackHUDBtn()
    {
        if(_backing) return;
        _backing = true;
        //se está na cena de quarto, icone desativado = Feito no awake

        Scene scene = SceneManager.GetActiveScene();
        if(scene.IsMap())
        {
            //se está na cena de mapa, volta para o quarto
            SceneLoader.Instance.LoadRoomScene();
        }
        else if(scene.IsEpisode() || scene.IsChallenge())
        {
            //se está na cena episodio OU desafio, volta para o mapa
            SceneLoader.Instance.LoadMapScene();
        }

    }

    public void OpenTutorial()
    {
        HelpUI help = GameObject.FindObjectOfType<HelpUI>();
        if(help != null)
            help.Open();
    }
}
