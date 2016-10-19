using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MDS.Validators;
using System.Linq;
using MDS.Validators.Interfaces;

namespace MDS
{

    public class FSM : MDSBehaviour, IValidatable
    {

        public struct State
        {
            public Sprite stateSprite { get; set; }
            public List<string> Labels;
        }

        public State[] states;
        public int UnselectedStateIndex = 0;

        public bool Selected
        {
            get
            {
                return _currentStateIndex != UnselectedStateIndex;
            }
        }

        private SpriteRenderer _spriteRenderer;
        private int _currentStateIndex;

        private FSMGroup _group;

        #region Unity 

        protected override void Awake()
        {
            base.Awake();

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public State GetCurrentState()
        {
            return states[_currentStateIndex];
        }

        void Start()
        {
            SetUnselected();
        }

        public void OnMouseUp()
        {
            if(++_currentStateIndex == states.Length)
                _currentStateIndex = 0;

            SetSprite();
            Select();
        }

        #endregion

        #region Methods

        protected virtual void SetSprite()
        {
            _spriteRenderer.sprite = states[_currentStateIndex].stateSprite;
        }

        public void SetUnselected()
        {
            _currentStateIndex = UnselectedStateIndex;
            SetSprite();
        }

        private void Select()
        {
            if(_group != null)
            {
                _group.SelectItem(this);
            }
        }

        public void SetGroup(FSMGroup group)
        {
            _group = group;
        }

        public bool ReadyToValidate()
        {
            return Selected;
        }

        public bool Validate(string acceptableAnswer)
        {
            if(!Selected)
                return false;

            bool ret = false;

            IList<string> labels = states[_currentStateIndex].Labels;

            ret = labels.Any(cur => cur == acceptableAnswer);

            return ret;

        }

       

        #endregion
    }
}