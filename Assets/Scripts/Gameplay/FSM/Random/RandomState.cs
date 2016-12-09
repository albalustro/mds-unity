using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.FSM.State.Transition;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.FSM.State
{

    public class RandomState : MDSBehaviour
    {
        public bool IsInitialState { get; internal set; }

        public TransitionSignals enterStateSignals;

        [SerializeField]
        private IStateTransition[] _transitions;

        #region Events

        public event Action ClickEvent;
        public event Action EnableEvent;
        public event Action DisableEvent;
        public event Action UpdateEvent;

        #endregion

        #region Unity events

        protected override void Awake()
        {
            base.Awake();
            foreach(var t in _transitions)
            {
                t.Initialize(this);
            }
        }

        public void OnMouseUp()
        {
            if(ClickEvent != null)
                ClickEvent();
        }

        public void OnEnable()
        {
            if(EnableEvent != null)
                EnableEvent();

            if (enterStateSignals!=null)
                enterStateSignals.Emit(this);
        }

        public void OnDisable()
        {
            if(DisableEvent != null)
                DisableEvent();
        }

        public void Update()
        {
            if(UpdateEvent != null)
                UpdateEvent();
        }

        #endregion

        public void ChangeState()
        {
            _transitions[0].ExecuteTransition();
        }
    }

}