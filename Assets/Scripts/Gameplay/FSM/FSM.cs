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
            public bool Rotate { get; set; }
            public float ZAngle { get; set; }
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
        private bool _animating;

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
            if(_animating) return;

            if(++_currentStateIndex == states.Length)
                _currentStateIndex = 0;


            if(states[_currentStateIndex].Rotate)
            {
                _animating = true;

                LeanTween.rotateZ(base.gameObject, states[_currentStateIndex].ZAngle, 0.5f)
                    .setEase(LeanTweenType.easeInCirc)
                    .setOnComplete(() => _animating = false);
            }
            else
            {
                SetSprite();
            }
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

        [FullInspector.InspectorButton]
        public void CreateRotationPatern()
        {
            Sprite s = GetComponent<SpriteRenderer>().sprite;


            states = new State[4];

            states[0] = new State();
            states[0].Rotate = true;
            states[0].stateSprite = s;
            states[0].ZAngle = 0;
            states[0].Labels = new List<string>() { "0" };

            states[1] = new State();
            states[1].Rotate = true;
            states[1].stateSprite = s;
            states[1].ZAngle = 90;
            states[1].Labels = new List<string>() { "90" };

            states[2] = new State();
            states[2].Rotate = true;
            states[2].stateSprite = s;
            states[2].ZAngle = 180;
            states[2].Labels = new List<string>() { "180" };

            states[3] = new State();
            states[3].Rotate = true;
            states[3].stateSprite = s;
            states[3].ZAngle = 270;
            states[3].Labels = new List<string>() { "270" };
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

        public int? GetNumericValue()
        {
            int result;
            if(Selected && int.TryParse(states[_currentStateIndex].Labels[0], out result))
            {
                return result;
            }
            return null;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #endregion
    }
}