using UnityEngine;

public class ProcessAnswerButton : MDSBehaviour
{
    public delegate void ProcessAnswerDelegate();
    public static event ProcessAnswerDelegate processAnswerEvent;

    public Sprite downSprite;
    public Sprite upSprite;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    private Color enabledColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0.5f);

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Disable();
    }

    public void Disable()
    {
        _spriteRenderer.sprite = upSprite;
        _spriteRenderer.color = disabledColor;
        _collider.enabled = false;
    }

    public void Enable()
    {
        _spriteRenderer.sprite = downSprite;
        _spriteRenderer.color = enabledColor;
        _collider.enabled = true;
    }

    public void OnMouseUp()
    {
        if (processAnswerEvent != null)
            processAnswerEvent();
    }
}
