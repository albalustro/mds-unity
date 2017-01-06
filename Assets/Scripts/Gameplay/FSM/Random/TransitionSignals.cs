using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace MDS.Gameplay.FSM
{
    public class TransitionSignals
    {

        public UnityEvent OnTransition;
        [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
        public IAction[] OnTransitionActions;


        public IEnumerator Emit(MonoBehaviour emiter)
        {
            if(OnTransition != null) 
                OnTransition.Invoke();


            if(OnTransitionActions != null)
            {
                for(int i = 0; i < OnTransitionActions.Length; i++)
                {
                    if(OnTransitionActions[i].waitFinish)
                        yield return emiter.StartCoroutine(OnTransitionActions[i].Execute());
                    else
                        emiter.StartCoroutine(OnTransitionActions[i].Execute());
                }
            }
            yield return null;
        }
    }


}