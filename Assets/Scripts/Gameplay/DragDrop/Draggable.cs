using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using FullInspector;
using System.Collections.Generic;

namespace MDS.Gameplay.DragDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class Draggable : MDSBehaviour
    {
        public List<string> Labels;

        public bool changeSprite;
        public bool changeScale;

        [InspectorShowIf("changeSprite")]
        public DraggableState<Sprite> spriteState;

        [InspectorShowIf("changeScale")]
        public DraggableState<float> scaleState;

        private Vector3 _touchOffset;
        private SpriteRenderer _renderer;

        [HideInInspector]
        public DropGroupSlot currentSlot;

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
                LeanTween.scale(gameObject, Vector3.one * scaleState.draggingValue, 0.5f).setEase(LeanTweenType.easeOutElastic);
            if(changeSprite)
                _renderer.sprite = spriteState.draggingValue;

        }

        public void OnMouseDrag()
        {
            Vector3 curVer = new Vector3();
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _touchOffset;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref curVer, 0.05f);
        }

        public void OnMouseUp()
        {

            BaseDropGroupArea group = null;
            DropGroupSlot slot = null;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector2 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
            RaycastHit2D hitGroup = Physics2D.Raycast(screenPos, Vector2.zero, 20f,
                                                        LayerMask.GetMask(new[] { "Group" }));
            if(hitGroup)
            {
                group = hitGroup.transform.GetComponent<BaseDropGroupArea>();

                RaycastHit2D hitSlot = Physics2D.Raycast(screenPos, Vector2.zero, 20f,
                                                LayerMask.GetMask(new[] { "GroupSlot" }));

                if(hitSlot)
                {
                    slot = hitSlot.transform.GetComponent<DropGroupSlot>();
                }

                group.SetInSlot(this, slot);

            }
            else
            {
                TweenGoto(currentSlot.transform.position);
            }


        }

        public void TweenGoto(Vector3 pos, float speed = 0.5f)
        {
            _renderer.sortingOrder = 5;
            LeanTween.move(gameObject, pos, speed)
                .setEase(LeanTweenType.easeOutCubic)
                .setOnComplete(() =>
                {
                    _renderer.sortingOrder = 0;
                    if(changeSprite)
                        _renderer.sprite = spriteState.releasedValue;
                    if(changeScale)
                        LeanTween.scale(gameObject, Vector3.one * scaleState.releasedValue, 0.2f).setEase(LeanTweenType.linear);
                    pos.z = -1;
                    transform.position = pos;
                });
        }


#if UNITY_EDITOR
        Color editorBoundColor = Color.blue;
        void OnDrawGizmos()
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if(box != null)
            {
                Gizmos.color = editorBoundColor;
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }
#endif

    }

}