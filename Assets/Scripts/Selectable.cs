using UnityEngine;
using System.Collections;
using System;

namespace MDS
{

    public class Selectable : MDSBehaviour
    {
        public Sprite selectedSprite;
        public Sprite unselectedSprite;

        private SpriteRenderer _spriteRenderer;

        private bool _selected;
        public bool Selected { get { return _selected; } }

        private SelectableGroup _group;
        public string ChoiseTag;

        #region Unity 

        protected override void Awake()
        {
            base.Awake();

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            _spriteRenderer.sprite = unselectedSprite;
            _selected = false;
        }

        public void OnMouseUp()
        {
            _selected = !_selected;
            SetSprite();
            if(_selected)
                Select();
        }

        #endregion

        #region Methods

        private void SetSprite()
        {
            if(_selected)
                _spriteRenderer.sprite = selectedSprite;
            else
                _spriteRenderer.sprite = unselectedSprite;
        }

        public void SetUnselected()
        {
            _selected = false;
            SetSprite();
        }

        private void Select()
        {
            if(_group != null)
                _group.SelectMe(this);
        }


        public void SetGroup(SelectableGroup group)
        {
            _group = group;
        }

        #endregion
    }
}