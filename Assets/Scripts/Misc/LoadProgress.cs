using System.Collections;
using System.Collections.Generic;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadProgress : MDSBehaviour {



    [SerializeField]
    private Sprite[] _backgroundsSprites;

    [SerializeField]
    private Sprite[] _progressBarSprites;

    private Image _bgImage;
    private Image _progressBarBG;
    private Image _progressBarFill;
    private Animator _animator;
    private Text _text;
    private Text _textSize;

    private GameObject _bgGO;
    private GameObject _animGO;
    private GameObject _fillGO;
    private GameObject _fillBGGO;
    private GameObject _textGO;
    private GameObject _textSizeGO;

    protected override void Awake()
    {
        base.Awake();

        _bgGO = transform.FindChild("BG").gameObject;
        _animGO = transform.FindChild("Anim").gameObject;
        _fillGO = transform.FindChild("Fill").gameObject;
        _fillBGGO = transform.FindChild("FillBG").gameObject;
        _textGO = transform.FindChild("Text").gameObject;
        _textSizeGO = transform.FindChild("SizeText").gameObject;

        _bgImage = _bgGO.GetComponent<Image>();
        _animator = _animGO.GetComponent<Animator>();
        _progressBarBG = _fillBGGO.GetComponent<Image>();
        _progressBarFill = _fillGO.GetComponent<Image>();
        _text = _textGO.GetComponent<Text>();
        _textSize = _textSizeGO.GetComponent<Text>();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;


    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        Vector3 pos = Camera.main.transform.position + Vector3.forward;
        transform.position = pos;
    }

    public void ShowLoadProgress(bool show)
    {
        _bgGO.SetActive(show);
        _animGO.SetActive(show);
        _fillBGGO.SetActive(show);
        _fillGO.SetActive(show);
        _textGO.SetActive(show);
        _textSizeGO.SetActive(show);

        _progressBarFill.fillAmount = 0f;
        _text.text = "";
        _textSize.text = "0 / 0";

        Scene curScene = SceneManager.GetActiveScene();

        int wIndex = 4;
        if(!curScene.name.Equals("splash"))
            wIndex = curScene.GetWorldIndex() - 1;            
        

        
        _bgImage.sprite = _backgroundsSprites[wIndex];
        _progressBarBG.sprite = _progressBarSprites[wIndex];
        _progressBarFill.sprite = _progressBarSprites[wIndex];
        StartCoroutine(LazySetAnim(wIndex));
    }

    private IEnumerator LazySetAnim(int wIndex)
    {
        yield return new WaitForEndOfFrame();
        if (_animator.isActiveAndEnabled)
            _animator.SetInteger("world", wIndex);
    }

    public void UpdateProgressBar(float percent)
    {
        Debug.Log(percent);
        _progressBarFill.fillAmount = percent;
        _textSize.text = string.Format("{0:0.00}%", percent*100f);
        if(percent < .1f)
            _text.text = "Preparando-se para dormir...";
        else
        if(percent < .2f)
            _text.text = "... Escovando os dentes antes de dormir ...";
        else
        if(percent < .3f)
            _text.text = "... Arrumando a cama ...";
        else
        if(percent < .4f)
            _text.text = "... Contando carneirinho ...";
        else
        if(percent < .5)
            _text.text = "... Dormindo ...";
        else
        if(percent < .6f)
            _text.text = "... Entrando no Mundo dos Sonhos ...";
        else
        if(percent < .7f)
            _text.text = "... Catalogando novos sonhos ...";
        else
        if(percent < .8f)
            _text.text = "... Guardando sonhos antigos ...";
        else
        if(percent < .9f)
            _text.text = "O sono está ficando profundo...";
        else
        //if(percent < 0.95f)
            _text.text = "Quase tudo pronto! Prepare-se!";
        
    }

}
