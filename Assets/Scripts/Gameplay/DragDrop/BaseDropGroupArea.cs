using UnityEngine;
using System.Linq;
using FullInspector;

namespace MDS.Gameplay.DragDrop
{

    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class BaseDropGroupArea : MDSBehaviour
    {
        [ShowInInspector, InspectorDisabled]
        protected DropGroupSlot[] slots;

        public bool SetInSlot(Draggable draggable, DropGroupSlot slot)
        {
            if(slot == null)
                slot = slots.FirstOrDefault(s => s.IsBusy == false);

            if(slot == null)
                return false;

            DraggableUtilities.SetDraggableInSlot(draggable, slot);

            return true;

        }

        public virtual void Start()
        {
            FillSlots();
        }

        [InspectorButton, InspectorTooltip("Use esse recurso para preencher os 'slots' com os DragGroupSlot filhos")]
        public void FillSlots()
        {
            slots = transform.GetComponentsInChildren<DropGroupSlot>();
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