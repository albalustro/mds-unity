using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.FSM;
using UnityEngine;

namespace MDS.Actions
{
    public class FSMChangeAction : BaseAction
    {
        [SerializeField]
        private FSM _fsm;

        [SerializeField]
        private string _value;

        [SerializeField]
        private bool _freeze;

        [SerializeField]
        private float _transitionTime = .2f;

        public override IEnumerator Execute()
        {
            if(byPass)yield break;

            yield return base.Execute();

            if(_freeze)
                _fsm.Freeze();

            while(_fsm.GetCurrentState().Labels.Contains(_value)==false)
            {
                _fsm.GoNextState(_transitionTime);
                yield return new WaitForSeconds(_transitionTime);
            }

        }

    }
}