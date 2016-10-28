using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MDS.Validators;
using System;
using MDS.Validators.Interfaces;

namespace MDS.Gameplay.DragDrop
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DropGroupSlot : MDSBehaviour, IValidatable
    {

        [HideInInspector]
        public Draggable draggableReference;

        [HideInInspector]
        public bool IsInitialSlot { get; internal set; }

        protected override void Awake()
        {
            base.Awake();
            GetComponent<BoxCollider2D>().isTrigger = true;
            if(GetComponentInParent<InitialDropGroupArea>() != null)
                IsInitialSlot = true;
            else
                IsInitialSlot = false;
        }

        public bool IsBusy
        {
            get
            {
                return draggableReference != null;
            }
        }

        #region IValidatable

        public bool ReadyToValidate()
        {
            return IsBusy;
        }

        public bool Validate(string acceptableAnswer)
        {
            if(!ReadyToValidate())
                return false;

            return draggableReference.Labels.Contains(acceptableAnswer);
        }

        #endregion

        #region Unity Editor Only

#if UNITY_EDITOR
        Color editorBoundColor = Color.yellow;
        void OnDrawGizmos()
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if(box != null)
            {
                Gizmos.color = editorBoundColor;
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }
#endif

        #endregion
    }
}