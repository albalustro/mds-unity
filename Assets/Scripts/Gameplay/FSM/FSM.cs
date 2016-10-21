using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MDS.Validators;
using System.Linq;
using MDS.Validators.Interfaces;

namespace MDS.Gameplay.FSM
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class FSM : MDSBehaviour, IValidatable
    {

        public struct State
        {
            public Sprite stateSprite { get; set; }
            public List<string> Labels;
        }

        public State[] states;
        public int? UnselectedStateIndex;

        public bool Selected
        {
            get
            {
                if (UnselectedStateIndex.HasValue)
                    return _currentStateIndex != UnselectedStateIndex.Value;
                return true;
            }
        }

        private SpriteRenderer _spriteRenderer;
        private int _currentStateIndex;

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
        }

        #endregion

        #region Methods

        protected virtual void SetSprite()
        {
            _spriteRenderer.sprite = states[_currentStateIndex].stateSprite;
        }

        public void SetUnselected()
        {
            if (UnselectedStateIndex.HasValue)
                _currentStateIndex = UnselectedStateIndex.Value;
            SetSprite();
        }

        #endregion

        #region IValidatable

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