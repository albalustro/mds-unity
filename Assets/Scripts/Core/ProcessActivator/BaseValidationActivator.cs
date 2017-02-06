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

        private bool _processing;

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
            if(!_processing)
            {
                _processing = true;
                StartCoroutine(fve());
            }
        }

        private IEnumerator fve()
        {
            if(OnValidateAnswer != null)
            {

                if(preValidationActions != null)
                {

                    List<IAction> actions = new List<IAction>();

                    List<UnityEngine.Object> vList = new List<UnityEngine.Object>();
                    vList.Add(this);
                    Collider2D c2D = GetComponent<Collider2D>();
                    if(c2D != null)
                        vList.Add(c2D);
                    actions.Add(new EnableDisableAction(EnableDisableAction.EAction.Disable, vList.ToArray()));

                    actions.AddRange(preValidationActions);

//                    actions.Add(new EnableDisableAction(EnableDisableAction.EAction.Enable, vList.ToArray()));

                    foreach(var action in actions)
                    {
                        if(action == null)
                            LogError("Action nula no vetor");
                        action.Initialize(this);
                        Log(action.GetType().Name);
                    }
                    yield return exec(actions.ToArray());
                }

                OnValidateAnswer();

                _processing = false;
            }
        }
    }
}

