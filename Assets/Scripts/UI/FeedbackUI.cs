using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : Singleton<FeedbackUI> {

    [SerializeField]
    GameObject _okBtn;
    [SerializeField]
    GameObject _closeBtn;
    [SerializeField]
    GameObject _simBtn;
    [SerializeField]
    GameObject _naoBtn;

    [SerializeField]
    GameObject _canvas;

    [SerializeField]
    Text _textField;

    Action _okFB, _closeFB, _simFB, _naoFB;

    private void Start()
    {
        _canvas.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public FeedbackUI SetText(string text)
    {
        _textField.text = text;
        return this;
    }

    public FeedbackUI SetButtons(bool ok, bool close, bool sim, bool nao)
    {
        _okBtn.SetActive(ok);
        _closeBtn.SetActive(close);
        _simBtn.SetActive(sim);
        _naoBtn.SetActive(nao);
        return this;
    }

    public FeedbackUI SetOKFeedback(Action okFeedback)
    {
        _okFB = okFeedback;
        return this;
    }

    public FeedbackUI SetCloseFeedback(Action closeFeedback)
    {
        _closeFB = closeFeedback;
        return this;
    }

    public FeedbackUI SetSimFeedback(Action simFeedback)
    {
        _simFB = simFeedback;
        return this;
    }

    public FeedbackUI SetNaoFeedback(Action naoFeedback)
    {
        _naoFB = naoFeedback;
        return this;
    }

    public FeedbackUI ClearAllButtonsFeedback()
    {
        _simFB = null;
        _naoFB = null;
        _okFB = null;
        _closeFB = null;
        return this;
    }

    public void Show()
    {
        _canvas.SetActive(true);
    }

    internal void Show(string text)
    {
        SetButtons(true, false, false, false);
        _textField.text = text;
        _canvas.SetActive(true);
    }

    public void Close()
    {
        _canvas.SetActive(false);
        _textField.text = "";
        ClearAllButtonsFeedback();
        SetButtons(false, true, false, false);
    }

    public void OkHandler()
    {
        if(_okFB != null)
            _okFB();
        Close();
    }
    public void CloseHandler()
    {
        if(_closeFB != null)
            _closeFB();
        Close();
    }
    public void SimHandler()
    {
        if(_simFB != null)
            _simFB();
        Close();
    }
    public void NaoHandler()
    {
        if(_naoFB != null)
            _naoFB();
        Close();
    }

   
}
