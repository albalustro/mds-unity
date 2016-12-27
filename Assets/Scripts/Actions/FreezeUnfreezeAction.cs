using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;
using MDS.Gameplay.DragDrop;

namespace MDS.Actions
{
	[Serializable]
	public class FreezeUnfreezeAction : BaseAction
	{

		public GameObject m_freezeEntireGroup;
		private bool FreezeOrUnfreezeEntireGroup {get {return m_freezeEntireGroup != null;}}

		public GameObject m_freezeGroupContent;


		[FullInspector.InspectorHideIf("FreezeOrUnfreezeEntireGroup")]
		public GameObject[] obj;

		public bool unfreeze = false;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			if (FreezeOrUnfreezeEntireGroup != null) {
				DropGroupSlot[] dGroup = m_freezeGroupContent.GetComponentsInChildren<DropGroupSlot> ();
				if (dGroup != null) {
					for (int i = 0; i < dGroup.Length; i++) {
						if (dGroup [i].draggableReference != null) {
							dGroup [i].draggableReference.GetComponent<Collider2D> ().enabled = unfreeze;
						}
					}
				}

			}

			if (obj != null) {
				for (int i = 0; i < obj.Length; i++) {
					obj [i].GetComponent<Collider2D> ().enabled = unfreeze;
				}
			}

			if (m_freezeGroupContent != null) {
				DropGroupSlot[] d = m_freezeGroupContent.GetComponentsInChildren<DropGroupSlot> ();
				for (int i = 0; i < d.Length; i++) {
					if (d [i] != null) {
						d [i].GetComponent<Collider2D> ().enabled = unfreeze;
					}
				}

			}

		}
	}
}
