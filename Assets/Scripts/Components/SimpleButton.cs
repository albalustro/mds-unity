using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SimpleButton : MDSBehaviour
{


    public Sprite downSprite;
    public Sprite upSprite;

    private SpriteRenderer _spriteRenderer;

    [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    public IAction[] actions;
    public UnityEvent _events;

    public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = upSprite;
    }

    public void OnMouseDown()
    {
        if(downSprite != null)
            _spriteRenderer.sprite = downSprite;
    }

    void OnMouseUp()
    {
        if(_events != null)
            _events.Invoke();

        if(actions != null)
        {
            ExecuteActions(actions);
        }

        _spriteRenderer.sprite = upSprite;
    }

}
