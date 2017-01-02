using System.Collections;
using UnityEngine;
using MDS.Core.Interfaces;
using System;
using MDS.Gameplay.DragDrop;


namespace MDS.Actions {


	public class MakeParentAction : BaseAction {

		[SerializeField]
		private Transform m_obj;

		[SerializeField]
		private Transform m_parent;

		[SerializeField]
		private DropGroupSlot m_dropSlotContentAsTarget;

		[SerializeField]
		private bool m_resetPosition;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			Draggable d = null;

			if (m_dropSlotContentAsTarget != null) {
				d = m_dropSlotContentAsTarget.draggableReference;
				if (d != null) {
					d.gameObject.transform.SetParent (m_parent);
				}
			}

			if (m_obj != null) {
				m_obj.SetParent (m_parent);
			}


			if (m_parent == null) {
				if(m_obj != null)
					m_obj.parent = null;
				if (d != null)
					d.transform.SetParent (null);
			} 


			if (m_resetPosition) {
				if (m_obj != null) {
					m_obj.position = m_parent.position;
				}
				if (d != null) {
					d.transform.position = m_parent.position;
				}
			}

		}

	}
}
