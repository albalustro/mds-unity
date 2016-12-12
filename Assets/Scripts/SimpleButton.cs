using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SimpleButton : MDSBehaviour
{


    public Sprite downSprite;
    public Sprite upSprite;

    private SpriteRenderer _spriteRenderer;

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

    IEnumerator OnMouseUp()
    {
        if(_events != null)
            _events.Invoke();

        if(actions != null)
        {
            for(int i = 0; i < actions.Length; i++)
            {
                if(actions[i] == null)
                {
                    LogError("Action não definida.");
                    continue;
                }

                if(actions[i].waitFinish)
                    yield return StartCoroutine(actions[i].Execute());
                else
                    StartCoroutine(actions[i].Execute());
            }
        }

        _spriteRenderer.sprite = upSprite;
    }

}
