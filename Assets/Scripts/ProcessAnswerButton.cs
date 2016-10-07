using UnityEngine;
using System.Collections;
using System;

public class ProcessAnswerButton : MDSBehaviour
{

    public Sprite downSprite;
    public Sprite upSprite;

    private SpriteRenderer _spriteRenderer;

    private bool _enabled;
    private Color enabledColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0.5f);

    protected override void Awake()
    {
        base.Awake();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Disable();
    }

    public void Disable()
    {
        _enabled = false;
        _spriteRenderer.sprite = upSprite;
        _spriteRenderer.color = disabledColor;
    }

    public void Enable()
    {
        _enabled = true;
        _spriteRenderer.sprite = downSprite;
        _spriteRenderer.color = enabledColor;
    }

    public void OnMouseUp()
    {
        
    }
}
