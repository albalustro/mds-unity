using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Gameplay.DragDrop;
using UnityEngine;

namespace MDS.Actions
{
    public class SetInSlotAction : BaseAction
    {

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

		[SerializeField]
		private bool m_makeDraggableChildrenOfSlot;

        public override IEnumerator Execute()
        {
            yield return base.Execute();


            Draggable d = _draggableTarget;
            BaseDropGroupArea g = _dropArea;


            if(selfTarget)
            {
                d = _corotineHolder.GetComponent<Draggable>();
                if(d == null)
                    _corotineHolder.GetComponent<MDSBehaviour>().LogError("selfTarget sem que o objeto tenha um elemento Draggable ");
            }

            if (useMemorizedGameObjectAsTarget)
            {
                d = MemorizeMe.MemorizedGameObject.GetComponent<Draggable>();
                if(d == null)
                     _corotineHolder.GetComponent<MDSBehaviour>().LogError("useMemorizedGameObjectAsTarget sem que o objeto tenha um elemento Draggable ");
            }

            if (useSlotContentAsTarget!=null)
            {
                d = useSlotContentAsTarget.draggableReference;
//                if(d == null)
//                    _corotineHolder.GetComponent<MDSBehaviour>().LogError("useSlotContentAsTarget sem que o slot tenha um elemento Draggable ");
            }

			if (m_multipleDraggables != null) {
				for (int i = 0; i < m_multipleDraggables.Length; i++) {
					d = m_multipleDraggables [i].GetComponent<Draggable>();

					if(!HasSpecificSlot)
					{
						DropGroupSlot slot = null;
						g.SetInSlot(d, ref slot);
					}
					else
					{
						DraggableUtilities.SetDraggableInSlot(d, _slot);
					}
				}
			} 

			if(d != null)
			{
				if(!HasSpecificSlot)
				{
					DropGroupSlot slot = null;
					g.SetInSlot(d, ref slot);
				}
				else
				{
					DraggableUtilities.SetDraggableInSlot(d, _slot);
				}

				if (m_makeDraggableChildrenOfSlot) {
					_draggableTarget.transform.SetParent (g.transform);
				}
			}

        }
    }
}