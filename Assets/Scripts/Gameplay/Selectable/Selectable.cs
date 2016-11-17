using UnityEngine;
using System.Collections;
using System;
using MDS.Validators.Interfaces;
using System.Linq;
using FullInspector;

namespace MDS.Gameplay.Selectable
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class Selectable : MDSBehaviour, IValidatable
    {
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private SelectionState<Sprite> _sprite;

        [SerializeField]
        private bool _changeLocalPosition;

        [SerializeField, InspectorShowIf("_changeLocalPosition")]
        private SelectionState<Vector2> _localPositionDisplacement;



        [SerializeField]
        private bool _changeLocalScale;

        [SerializeField, InspectorShowIf("_changeLocalScale")]
        private SelectionState<Vector2> _localScale;


        [SerializeField]
        private bool _changeZRotation;

        [SerializeField, InspectorShowIfAttribute("_changeZRotation")]
        private SelectionState<float> _zRotation;

        private Vector2 _startLocalPosition;
        private float _startZRotation;

        private bool _selected;
        public bool Selected { get { return _selected; } }

        private SelectableGroup _group;
        public string[] Labels;

        [SerializeField, InspectorTooltip("Objectos que terão seu estado Active refletindo a propriedade Selected")]
        private GameObject[] _activateOnSelect;

        [SerializeField, InspectorTooltip("Objectos que terão seu estado Active refletindo o inverso de Selected")]
        private GameObject[] _deactivateOnSelect;

        #region Unity 

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            _spriteRenderer.sprite = _sprite.UnselectedValue;
            _selected = false;
            _startLocalPosition = transform.localPosition;
            _startZRotation = transform.localRotation.eulerAngles.z;


            SetLocalPosition();
            SetLocalScale();
            SetZRotation();
            SetSprite();
            SetChildrenActivation();

        }

        public void OnMouseUp()
        {
            if(!Selected)
            {
                if(_group != null)
                {
                    if(!_group.CanSelect())
                        return;
                }
            }

            ToggleSelect();
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {

            base.OnValidate();

            if (_sprite==null)
            {
                _sprite = new SelectionState<Sprite>();
            }

            if(_changeLocalScale && _localScale == null)
            {
                _localScale = new SelectionState<Vector2>();
                _localScale.SelectedValue = Vector2.one;
                _localScale.UnselectedValue = Vector2.one;
            }

            if (_changeLocalPosition && _localPositionDisplacement == null)
            {
                _localPositionDisplacement = new SelectionState<Vector2>();
                _localPositionDisplacement.SelectedValue = Vector2.zero;
                _localPositionDisplacement.UnselectedValue = Vector2.zero;
            }

            if (_changeZRotation && _zRotation==null)
            {
                _zRotation = new SelectionState<float>();
                _zRotation.SelectedValue = 0;
                _zRotation.UnselectedValue = 0;
            }

            if(_spriteRenderer != null && _spriteRenderer.sprite == null)
                _spriteRenderer.sprite = _sprite.UnselectedValue;
        }
#endif
        #endregion

        #region Methods

        private void SetSprite()
        {
            if(_selected)
                _spriteRenderer.sprite = _sprite.SelectedValue;
            else
                _spriteRenderer.sprite = _sprite.UnselectedValue;
        }

        private void SetLocalPosition()
        {
            if(!_changeLocalPosition) return;

            Vector2 p = _localPositionDisplacement.UnselectedValue;

            if(_selected)
                p = _localPositionDisplacement.SelectedValue;

            transform.localPosition = _startLocalPosition + p;
        }

        private void SetLocalScale()
        {
            if(!_changeLocalScale) return;

            Vector2 s = _localScale.UnselectedValue;

            if(_selected)
                s = _localScale.SelectedValue;

            transform.localScale = new Vector3(s.x, s.y, 1f);
        }

        private void SetZRotation()
        {
            if(!_changeZRotation) return;

            float z = _zRotation.UnselectedValue;

            if(_selected)
                z = _zRotation.SelectedValue;

            transform.localRotation = Quaternion.Euler(0f, 0f, _startZRotation + z);
        }

        public void SetUnselected()
        {
            _selected = false;
            SetLocalPosition();
            SetLocalScale();
            SetZRotation();
            SetSprite();
            SetChildrenActivation();
        }

        private void ToggleSelect()
        {
            _selected = !_selected;
            SetLocalPosition();
            SetLocalScale();
            SetZRotation();
            SetSprite();
            SetChildrenActivation();
            if (_group != null)
            {
                _group.SelectItem(this);
            }
        }

        public void SetGroup(SelectableGroup group)
        {
            _group = group;
        }

        private void SetChildrenActivation()
        {
            if(_activateOnSelect != null && _activateOnSelect.Length > 0)
            {
                foreach(var go in _activateOnSelect)
                {
                    go.SetActive(_selected);
                }
            }

            if(_deactivateOnSelect != null && _deactivateOnSelect.Length > 0)
            {
                foreach(var go in _deactivateOnSelect)
                {
                    go.SetActive(!_selected);
                }
            }


        }

        #endregion

        #region IValidatable

        public bool ReadyToValidate()
        {
            return true;
        }

        public bool Validate(string acceptableAnswer)
        {
            return Selected && Labels.Contains(acceptableAnswer);
        }

        public int? GetNumericValue()
        {
            int result;
            if (Selected && int.TryParse(Labels[0],out result))
            {
                return result;
            }
            return null;
        }

        #endregion

    }


    public class SelectionState<T>
    {
        public T SelectedValue { get; set; }
        public T UnselectedValue { get; set; }
    }
}