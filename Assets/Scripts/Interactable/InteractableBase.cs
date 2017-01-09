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

        [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
        public IAction[] OnBeforeInteractActions;

        [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
        public IAction[] OnAfterInteractActions;


        private Collider2D _collider;

        public abstract void Interact();

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider2D>();
        }

        public virtual void OnEnable()
        {
            _collider.enabled = true;
        }

        public virtual void OnDisable()
        {
            _collider.enabled = false;
        }

        public void OnMouseUp()
        {
            
            ExecuteActions(OnBeforeInteractActions);

            Interact();

            ExecuteActions(OnAfterInteractActions);
        }
    }
}