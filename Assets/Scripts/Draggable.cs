using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using FullInspector;

public class Draggable : MDSBehaviour
{
    public bool changeSprite;
    public bool changeScale;

   
    [InspectorShowIf("changeSprite")]
    public DraggableState<Sprite> spriteState;

    [InspectorShowIf("changeScale")]
    public DraggableState<float> scaleState;


    private Vector3 _touchOffset;
    private SpriteRenderer _renderer;
    private Vector3 _initialPosition;

    protected override void Awake()
    {
        base.Awake();

        _renderer = this.GetComponent<SpriteRenderer>();

        if(changeScale && (scaleState.draggingValue <= 0f || scaleState.releasedValue <= 0))
            Debug.LogError("[Draggable] Scale cannot be 0");
    }

    public void OnMouseDown()
    {
        _renderer.sortingOrder = 5;

        _touchOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if(changeScale)
        {
            LeanTween.scale(gameObject, Vector3.one * scaleState.draggingValue, 0.5f)
                .setEase(LeanTweenType.easeOutElastic);
        }

        if (changeSprite)
        {
            _renderer.sprite = spriteState.draggingValue;
        }

        _initialPosition = transform.position;
    }

    public void OnMouseDrag()
    {
        Vector3 curVer = new Vector3();
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _touchOffset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref curVer, 0.05f);
    }

    public void OnMouseUp()
    {

        // TODO: verificar onde esta sendo solto para conhecer a posicao final

        if(changeScale)
        {
            LeanTween.scale(gameObject, Vector3.one*scaleState.releasedValue, 0.5f)
                .setEase(LeanTweenType.linear);
        }

        // TODO: OBTER O GROUP ATRAVEZ DE UM RAYCAST

        var group = GameObject.FindObjectOfType<DraggableGroup>();
        group.GetValidPosition(ref _initialPosition, this);
        

        LeanTween.move(gameObject, _initialPosition, 0.5f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(()=> 
            {
                _renderer.sortingOrder = 0;
                if (changeSprite)
                {
                    _renderer.sprite = spriteState.releasedValue;

                }
            });

    }

}

public class DraggableState<T>
{
    public T releasedValue;
    public T draggingValue;
}


