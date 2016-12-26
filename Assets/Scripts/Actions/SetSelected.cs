using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;
using System;
using FullInspector;


namespace MDS.Actions{

	[Serializable]
	public class SetSelected : BaseAction
	{

		public bool m_useMemorizedGo;

		[FullInspector.InspectorHideIf("m_useMemorizedGo")]
		public GameObject[] obj;

		[SerializeField]
		private bool m_set;

		[SerializeField, Tooltip("Drag SelectableGroup to here if you want to Lock/Unlock all selectable colliders")]
		private Transform m_groupTransform;
		private bool ShowLockUnlockButton {get {return m_groupTransform != null;}}


		[InspectorShowIf("ShowLockUnlockButton")]
		[SerializeField]
		private bool m_unselectAll = false;

		[SerializeField, InspectorTooltip("Check this if you want to lock/unlock the selected group")]
		[InspectorShowIf("ShowLockUnlockButton")]
		private bool m_lockGroup;



		public override IEnumerator Execute()
		{
			yield return base.Execute();

			if (m_groupTransform != null) {
				foreach (Transform child in m_groupTransform) {
					child.GetComponent<Collider2D> ().enabled = !m_lockGroup;
					if (m_unselectAll) {
						child.GetComponent<Selectable> ().SetSelected (false);
					}
				}
			}


			if (!m_useMemorizedGo) {

				for (int i = 0; i < obj.Length; i++) {
					obj [i].GetComponent<Selectable> ().SetSelected (m_set);
				}
			} else {
				GameObject tmpGo = MemorizeMe.MemorizedGameObject;
				tmpGo.GetComponent<Selectable> ().SetSelected (m_set);
			}




		}

	}
}
