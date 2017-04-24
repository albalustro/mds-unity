using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMapUI : MonoBehaviour {

    public Canvas canvas;
    public GameObject window;

    private CanvasGroup _windowsCanvasGroup;
    private RectTransform _windowRectTransform;

    private void Start()
    {
        _windowRectTransform = window.GetComponent<RectTransform>();
        _windowsCanvasGroup = window.GetComponent<CanvasGroup>();
    }

    private void OnMouseDown()
    {
        Open();
    }

    public void Open()
    {
        LeanTween.move(_windowRectTransform, Vector3.zero, 0.3f);
        LeanTween.scale(window, Vector3.one, 0.3f);
        LeanTween.alphaCanvas(_windowsCanvasGroup, 1, 0.3f);
        canvas.gameObject.SetActive(true);
    }

    public void Close()
    {
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
