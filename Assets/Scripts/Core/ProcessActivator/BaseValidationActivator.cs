using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Core.ProcessActivator
{
    public abstract class BaseValidationActivator : MDSBehaviour, IValidationActivator
    {
        public IAction[] preValidationActions;

        public event ValidateAnswerDelegate OnValidateAnswer;

        protected bool _isEnabled;

        public virtual void Disable()
        {
            _isEnabled = false;
           
        }

        public virtual void Enable()
        {
            _isEnabled = true;
           
        }

        protected void FireValidationEvent()
        {
            StartCoroutine(fve());   
        }

        private IEnumerator fve()
        {
            if(OnValidateAnswer != null)
            {

                if(preValidationActions != null)
                {
                    foreach(var action in preValidationActions)
                    {
                        if(action == null)
                            LogError("Action nula no vetor");
                        action.Initialize(this);
                    }
                    yield return exec(preValidationActions);
                }

                OnValidateAnswer();
            }
        }
    }
}
