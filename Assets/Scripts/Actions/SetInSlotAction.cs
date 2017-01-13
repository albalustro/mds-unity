using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Gameplay.DragDrop;
using UnityEngine;

namespace MDS.Actions
{
    public class SetInSlotAction : BaseAction
    {

        [SerializeField]
        private float _duration = 1.5f;

        [InspectorDivider, InspectorHeader("Target")]
        [SerializeField,  InspectorHideIf("Hide_selfTarget")]
        private bool selfTarget;
        private bool Hide_selfTarget { get { return useMemorizedGameObjectAsTarget || useSlotContentAsTarget != null; } }

        [SerializeField,   InspectorHideIf("Hide_useMemorizedGameObjectAsTarget")]
        private bool useMemorizedGameObjectAsTarget;
        private bool Hide_useMemorizedGameObjectAsTarget { get { return selfTarget || useSlotContentAsTarget != null; } }

        [SerializeField, InspectorHideIf("Hide_useSlotContentAsTarget")]
        private DropGroupSlot useSlotContentAsTarget;
        private bool Hide_useSlotContentAsTarget { get { return selfTarget || useMemorizedGameObjectAsTarget; } }

		[SerializeField]
		private ValidatableDropGroupArea _DropAreaSlotsContentAsTargets;

		[SerializeField]
		private DropGroupSlot[] m_multipleSlotsAsTargets;

        [SerializeField, InspectorHideIf("HideTarget")]
        private Draggable _draggableTarget;
        private bool HideTarget { get { return selfTarget || useMemorizedGameObjectAsTarget || useSlotContentAsTarget!=null; } }

		public GameObject[] m_multipleDraggables;

        [InspectorDivider, InspectorHeader("Destination")]
        
        [SerializeField,   InspectorHideIf("HasSpecificSlot")]
        private BaseDropGroupArea _dropArea;
        private bool HasDropArea { get { return _dropArea != null; } }

        [SerializeField,  InspectorHideIf("HasDropArea")]
        private DropGroupSlot _slot;
        private bool HasSpecificSlot { get { return _slot != null; } }

		[SerializeField, Tooltip("Só funciona para configurações em que o target do SetInSlot é único e não o conteúdo de um grupo")]
		private bool m_makeDraggableChildrenOfSlot;

        public override IEnumerator Execute()
        {
            if(byPass) yield break;
            yield return base.Execute();

			if (_DropAreaSlotsContentAsTargets != null) {
				m_multipleSlotsAsTargets = _DropAreaSlotsContentAsTargets.GetComponentsInChildren<DropGroupSlot> ();
			}

			if(m_multipleSlotsAsTargets != null && m_multipleSlotsAsTargets.Length > 0){
				if (_dropArea == null) {
					_corotineHolder.GetComponent<MDSBehaviour> ().LogError ("Multiplos slots só podem ser colocado em um DropGroupArea");
					yield break;
				}
			}


            Draggable draggabableToBeSetInSlot = _draggableTarget;
            DropGroupSlot originalDraggableSlot = null;
            BaseDropGroupArea g = _dropArea;
			DropGroupSlot slot = null;
			DropGroupSlot tempDropGroupSlot = null;

            if(selfTarget)
            {
                draggabableToBeSetInSlot = _corotineHolder.GetComponent<Draggable>();
                if(draggabableToBeSetInSlot == null)
                    _corotineHolder.GetComponent<MDSBehaviour>().LogError("selfTarget sem que o objeto tenha um elemento Draggable ");
                originalDraggableSlot = draggabableToBeSetInSlot.currentSlot;
            }

            if (useMemorizedGameObjectAsTarget)
            {
                draggabableToBeSetInSlot = MemorizeMe.MemorizedGameObject.GetComponent<Draggable>();
                if(draggabableToBeSetInSlot == null)
                     _corotineHolder.GetComponent<MDSBehaviour>().LogError("useMemorizedGameObjectAsTarget sem que o objeto tenha um elemento Draggable ");
                originalDraggableSlot = draggabableToBeSetInSlot.currentSlot;
            }

            if (useSlotContentAsTarget!=null)
            {
                draggabableToBeSetInSlot = useSlotContentAsTarget.draggableReference;
                originalDraggableSlot = draggabableToBeSetInSlot.currentSlot;
            }

			if (m_multipleSlotsAsTargets != null && m_multipleSlotsAsTargets.Length > 0) {
				for (int i = 0; i < m_multipleSlotsAsTargets.Length; i++) {

					tempDropGroupSlot = m_multipleSlotsAsTargets [i].GetComponent<DropGroupSlot> ();
					if (tempDropGroupSlot != null) {
						draggabableToBeSetInSlot = tempDropGroupSlot.draggableReference;
                        if(draggabableToBeSetInSlot != null) { 
                            originalDraggableSlot = tempDropGroupSlot;
                            slot = null;
                            if(g.SetInSlot(draggabableToBeSetInSlot, ref slot))
                                draggabableToBeSetInSlot.ProcessSlotChanging(originalDraggableSlot);
						}
					}
				}
                draggabableToBeSetInSlot = null;
            }

			if (m_multipleDraggables != null && m_multipleDraggables.Length > 0) {
				for (int i = 0; i < m_multipleDraggables.Length; i++) {
					draggabableToBeSetInSlot = m_multipleDraggables [i].GetComponent<Draggable>();
                    originalDraggableSlot = draggabableToBeSetInSlot.currentSlot;
                    if(!HasSpecificSlot)
					{
						slot = null;
                        if(g.SetInSlot(draggabableToBeSetInSlot, ref slot, _duration))
                            draggabableToBeSetInSlot.ProcessSlotChanging(originalDraggableSlot);
					}
					else
					{
						DraggableUtilities.SetDraggableInSlot(draggabableToBeSetInSlot, _slot, _duration);
					}
				}
				draggabableToBeSetInSlot = null;
			} 

			if(draggabableToBeSetInSlot != null)
			{
				if(!HasSpecificSlot)
				{
					slot = null;
                    if(g.SetInSlot(draggabableToBeSetInSlot, ref slot, _duration))
                        draggabableToBeSetInSlot.ProcessSlotChanging(originalDraggableSlot);
				}
				else
				{
					DraggableUtilities.SetDraggableInSlot(draggabableToBeSetInSlot, _slot, _duration);
				}

				if (m_makeDraggableChildrenOfSlot) {
                    draggabableToBeSetInSlot.transform.SetParent (g.transform);
				}
			}

        }



        public SetInSlotAction()
        {

        }

        public SetInSlotAction(Draggable target, BaseDropGroupArea destination )
        {
            _draggableTarget = target;
            _dropArea = destination;
        }
    }
}