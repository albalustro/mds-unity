using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMoveChallengeMapUI : MonoBehaviour {

    public Canvas canvas;
    public GameObject window;

    private CanvasGroup _windowsCanvasGroup;
    private RectTransform _windowRectTransform;

    public ConceptMap activeMap;

    private void Start()
    {
        _windowRectTransform = window.GetComponent<RectTransform>();
        _windowsCanvasGroup = window.GetComponent<CanvasGroup>();
    }


    public void Open(ConceptMap myMap)
    {
        activeMap = myMap;
        LeanTween.move(_windowRectTransform, Vector3.zero, 0.3f);
        LeanTween.scale(window, Vector3.one, 0.3f);
        LeanTween.alphaCanvas(_windowsCanvasGroup, 1, 0.3f);
        canvas.gameObject.SetActive(true);
    }

    public void Close()
    {
        activeMap = null;
        LeanTween.move(_windowRectTransform, new Vector3(-600, -221, 0), 0.3f);
        LeanTween.scale(window, Vector3.zero, 0.3f);
        LeanTween.alphaCanvas(_windowsCanvasGroup, 0, 0.3f);
        Invoke("InternalClose", 0.3f);
    }

    private void InternalClose()
    {
        canvas.gameObject.SetActive(false);
    }
}
