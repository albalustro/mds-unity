using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace MDS.Gameplay.FSM.State.Transition
{
    [Serializable]
    public abstract class BaseStateTransition : IStateTransition
    {

        protected RndState _myState;
        public GameObject _nextState;

        public TransitionSignals exitStateSignals;

        #region Methods

        public virtual void ExecuteTransition()
        {
            if(exitStateSignals!=null)
                exitStateSignals.Emit();

            _nextState.SetActive(true);
            _myState.gameObject.SetActive(false);

        }

        public virtual void Initialize(RndState state)
        {
            _myState = state;
        }

        #endregion

    }

}