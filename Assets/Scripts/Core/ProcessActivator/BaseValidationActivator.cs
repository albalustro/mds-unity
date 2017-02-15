using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using MDS.Actions;
using MDS.Core;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Core.ProcessActivator
{
    public abstract class BaseValidationActivator : MDSBehaviour, IValidationActivator
    {
        [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
        public IAction[] preValidationActions;

        public event ValidateAnswerDelegate OnValidateAnswer;

        public MDSBehaviour Behaviour { get { return this; } }

        protected bool _isEnabled;

        protected bool _executingPreValidationActions;

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
            if(!_executingPreValidationActions)
            {
                _executingPreValidationActions = true;
                StartCoroutine(fve());
            }
        }

        private IEnumerator fve()
        {
            if(OnValidateAnswer != null)
            {

                if(preValidationActions != null)
                {
                    foreach(var action in preValidationActions)
                    {
                        if(action != null)
                            action.Initialize(this); 
                        else
                            LogError("Action nula no vetor");
                        //   Log(action.GetType().Name);
                    }
                    yield return exec(preValidationActions.ToArray());
                }

                OnValidateAnswer();

                _executingPreValidationActions = false;
            }
        }
    }
}

