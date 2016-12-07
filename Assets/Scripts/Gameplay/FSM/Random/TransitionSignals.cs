using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace MDS.Gameplay.FSM
{
    public class TransitionSignals
    {

        public UnityEvent OnTransition;
        public IAction[] OnTransitionActions;


        public void Emit()
        {
            if(OnTransition != null) 
                OnTransition.Invoke();

            if(OnTransitionActions != null)
            {
                foreach(var ac in OnTransitionActions)
                {
                    if(ac != null)
                        ac.Execute();
                }
            }
        }
    }


}