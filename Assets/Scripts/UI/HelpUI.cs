using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour {

    public GameObject root;
    public GameObject window;
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;

    public GameObject[] panels;

    public float animTime;

    private Canvas _canvas;

    private int _currentPanelIndex;
    private int _maxPanels;
    private void Start()
    {
        _canvas = root.GetComponentInChildren<Canvas>();
        ResetPanels();
        root.SetActive(false);
        window.transform.localScale = Vector3.zero;
    }

    private void ResetPanels()
    {
        _currentPanelIndex = 0;
        _maxPanels = panels.Length;
        for(int i = 0; i < _maxPanels; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);
    }

    public void Close()
    {
        LeanTween.scale(window, Vector3.zero, animTime)
            .setOnComplete(InternalClose)
            .setEase(LeanTweenType.easeInBack);
    }

    private void InternalClose()
    {
        root.SetActive(false);
    }

    
    public void Open()
    {
        ResetPanels();
        root.SetActive(true);
        LeanTween.scale(window, Vector3.one, animTime).setEase( LeanTweenType.easeOutBack);
        _canvas.worldCamera = Camera.main;
        FixButtons();
    }

    public void Next()
    {
        panels[_currentPanelIndex].SetActive(false);
        _currentPanelIndex++;
        panels[_currentPanelIndex].SetActive(true);
        FixButtons();   
    }

    public void Previous()
    {
        panels[_currentPanelIndex].SetActive(false);
        _currentPanelIndex--;
        panels[_currentPanelIndex].SetActive(true);
        FixButtons();
    }

    private void FixButtons()
    {
        nextButton.interactable = _currentPanelIndex < (_maxPanels-1);
        previousButton.interactable = _currentPanelIndex > 0;
    }
}
