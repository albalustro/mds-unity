using System.Collections;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.FSM.State.Transition
{

    public class ChangeOnRndTimerStateTransition : BaseStateTransition
    {
        [InspectorMargin(20)]

        [SerializeField]
        private float _timeToChangeState = 3f;

        [SerializeField]
        private bool _resetOnClick;

        private float _currentTime;

        #region Methods IStateTransition

        public override void Initialize(RndState state)
        {
            base.Initialize(state);
            state.EnableEvent += OnEnable;
            state.UpdateEvent += Update;
            state.ClickEvent += OnMouseUp;
        }

        #endregion

        #region Event handlers

        public void OnEnable()
        {
            _currentTime = 0;
        }

        public void OnMouseUp()
        {
            if(_resetOnClick)
                _currentTime = 0;
        }

        void Update()
        {
            _currentTime += Time.deltaTime;
            if(_currentTime >= _timeToChangeState)
                ExecuteTransition();
        }

        #endregion
    }

}