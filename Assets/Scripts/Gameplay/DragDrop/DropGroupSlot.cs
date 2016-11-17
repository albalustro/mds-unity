using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MDS.Validators;
using System;
using MDS.Validators.Interfaces;
using FullInspector;

namespace MDS.Gameplay.DragDrop
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DropGroupSlot : MDSBehaviour, IValidatable
    {

        [SerializeField]
        private bool _changeSprite;

        [SerializeField, InspectorShowIf("_changeSprite")]
        private SlotState<Sprite> _sprite;

        private SpriteRenderer _spriteRenderer;

        private Draggable _draggableReference;

        [HideInInspector]
        public Draggable draggableReference
        {
            get
            {
                return _draggableReference;
            }
            set
            {
                _draggableReference = value;
                if (_changeSprite)
                {
                    if(_draggableReference == null)
                        _spriteRenderer.sprite = _sprite.EmptyValue;
                    else
                        _spriteRenderer.sprite = _sprite.FilledValue;
                }
            }
        }

        [HideInInspector]
        public bool IsInitialSlot { get; internal set; }

        [HideInInspector]
        public bool IsInstatiableInitialSlot { get; internal set; }

        protected override void Awake()
        {
            base.Awake();

            gameObject.layer = LayerMask.NameToLayer("GroupSlot");

            GetComponent<BoxCollider2D>().isTrigger = true;

            if(GetComponentInParent<InitialDropGroupArea>() != null)
                IsInitialSlot = true;
            else
                IsInitialSlot = false;

            if(GetComponentInParent<InitialIntanceDropGroupArea>() != null)
                IsInstatiableInitialSlot = true;
            else
                IsInstatiableInitialSlot = false;

            if (_changeSprite)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                if(_spriteRenderer == null)
                {
                    _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                    _spriteRenderer.sortingLayerName = "DropGroup";
                    _spriteRenderer.sortingOrder = 1;
                }
                _spriteRenderer.sprite = _sprite.EmptyValue;
            }
        }

        public bool IsTaken
        {
            get
            {
                return draggableReference != null;
            }
        }

        #region IValidatable

        public bool ReadyToValidate()
        {
            return IsTaken;
        }

        public bool Validate(string acceptableAnswer)
        {
            if(!ReadyToValidate())
                return false;

            return draggableReference.Labels.Contains(acceptableAnswer);
        }

        public int? GetNumericValue()
        {
            int result;
            if(IsTaken && int.TryParse(draggableReference.Labels[0], out result))
            {
                return result;
            }
            return null;
        }
        #endregion

        #region Unity Editor Only

#if UNITY_EDITOR
        Color editorBoundColor = Color.yellow;
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

        #endregion
    }

    public class SlotState<T>
    {
        public T FilledValue { get; set; }
        public T EmptyValue { get; set; }
    }

}