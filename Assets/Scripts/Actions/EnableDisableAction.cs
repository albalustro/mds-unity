using UnityEngine;
using System.Collections;
using System;
using MDS.Core.Interfaces;

namespace MDS.Actions
{
    public class EnableDisableAction : IAction
    {

        public enum EAction
        {
            Enable,
            Disable
        }

        [SerializeField]
        private EAction _action;

        [SerializeField]
        private UnityEngine.Object _target;

        public IEnumerator Execute(Action callback = null)
        {

            bool en = false;

            switch(_action)
            {
                case EAction.Enable:
                    en = true;
                    break;

                case EAction.Disable:
                    en = false;
                    break;

            }

            if (_target is GameObject)
            {
                (_target as GameObject).SetActive(en);
            }   
            else if (_target is Behaviour)
            {
                (_target as Behaviour).enabled = en;
            }

            if(callback != null)
                callback();

            yield return null;

        }

      
    }


}