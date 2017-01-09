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

		[FullInspector.InspectorTooltip("Dá lock nos draggables dos slots do grupo selecionado")]
		[FullInspector.InspectorName("DropGroupDraggableContent")]
		public GameObject m_freezeEntireGroup;
		private bool FreezeOrUnfreezeEntireGroup {get {return m_freezeEntireGroup != null;}}

		[FullInspector.InspectorTooltip("Dá lock nos filhos do GameObject selecionado")]
		[FullInspector.InspectorName("ChildrensOfThisGO")]
		public GameObject m_freezeGroupContent;

		[SerializeField]
		private bool _useMemorizedObjAsTarget;

		[FullInspector.InspectorHideIf("FreezeOrUnfreezeEntireGroup")]
		public GameObject[] obj;

		public bool unfreeze = false;

		public override IEnumerator Execute()
		{
            if(byPass) yield break;
            yield return base.Execute();

			if (m_freezeEntireGroup != null) {
				DropGroupSlot[] dGroup = m_freezeGroupContent.GetComponentsInChildren<DropGroupSlot> ();
				if (dGroup != null) {
					for (int i = 0; i < dGroup.Length; i++) {
						if (dGroup [i].draggableReference != null) {
							dGroup [i].draggableReference.GetComponent<Collider2D> ().enabled = unfreeze;
						}
					}
				}

			}


			if (_useMemorizedObjAsTarget) {
				GameObject tmpGo = MemorizeMe.MemorizedGameObject;
				tmpGo.GetComponent<Collider2D> ().enabled = false;
			}

			if (obj != null) {
				for (int i = 0; i < obj.Length; i++) {
					obj [i].GetComponent<Collider2D> ().enabled = unfreeze;
				}
			}

			if (m_freezeGroupContent != null) {
				Collider2D[] col = m_freezeGroupContent.GetComponentsInChildren<Collider2D> ();
				for (int i = 0; i < col.Length; i++) {
					if (col [i] != null) {
						col [i].GetComponent<Collider2D> ().enabled = unfreeze;
					}
				}

			}

		}
	}
}
