using UnityEngine;
using System.Linq;
using FullInspector;
using System;
using System.Collections.Generic;

namespace MDS.Gameplay.DragDrop
{

    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class BaseDropGroupArea : MDSBehaviour
    {

        [ShowInInspector, InspectorDisabled]
        protected List<DropGroupSlot> slots;

        public virtual bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            if(slot == null)
            {
                if(draggable.forcePreferredIndexOnDrop)
                {
                    if(draggable.PreferredInitialIndex.HasValue)
                        slot = slots.FirstOrDefault(s => s.IsInitialSlot == true && slots.IndexOf(s) == draggable.PreferredInitialIndex.Value);
                }
                else
                {
                    slot = slots.FirstOrDefault(s => s.IsBusy == false);
                }
            }

            if(slot == null)
                return false;

            if(draggable.forcePreferredIndexOnDrop)
            {
                if(slot.IsInitialSlot)
                {
                    if(draggable.PreferredInitialIndex.HasValue)
                    {
                        int slotIndex = slots.IndexOf(slot);
                        if(slotIndex != draggable.PreferredInitialIndex.Value)
                            return false;
                    }
                    else
                    {
                        Debug.LogError("Draggable com 'forcePreferredIndexOnDrop' porém sem 'PreferredInitialIndex'");
                        return false;
                    }
                }
            }

            // todo:
            // da forma como está, ainda é possível 'forçar' um draggable no slot inicial errado 
            // simplesmente fazendo o swap (soltando um draggable em um slot final ocupado por outro draggable)
            DraggableUtilities.SetDraggableInSlot(draggable, slot);

            return true;

        }

        public virtual void Start()
        {
            FillSlots();
        }

        [InspectorButton, InspectorTooltip("Use esse recurso para preencher os 'slots' com os DropGroupSlot filhos")]
        public void FillSlots()
        {
            slots = transform.GetComponentsInChildren<DropGroupSlot>().ToList();
        }


        #region Unity Editor Only

#if UNITY_EDITOR

        Color editorBoundColor = Color.cyan;
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