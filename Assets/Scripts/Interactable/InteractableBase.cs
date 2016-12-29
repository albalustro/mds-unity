using System;
using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Core.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MDS.Interactable
{
    public abstract class InteractableBase : MDSBehaviour
    {


        public IAction[] OnBeforeInteractActions;
        public IAction[] OnAfterInteractActions;


        public abstract void Interact();

      

        public void OnMouseUp()
        {
            ExecuteActions(OnBeforeInteractActions);

            Interact();

            ExecuteActions(OnAfterInteractActions);
        }
    }
}