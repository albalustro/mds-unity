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

        protected RandomState _myState;
        public GameObject _nextState;

        public TransitionSignals exitStateSignals;

        #region Methods

        public virtual void ExecuteTransition()
        {
            
			_myState.StartCoroutine (InternalExecuteTransition());

        }

        public virtual void Initialize(RandomState state)
        {
            _myState = state;
        }

        #endregion

		IEnumerator InternalExecuteTransition()
		{
			if (exitStateSignals != null) {
				yield return _myState.StartCoroutine (exitStateSignals.Emit (_myState));
			}

			_nextState.SetActive(true);
			_myState.gameObject.SetActive(false);
		}

    }

}