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
        [InspectorComment("Selecionar qual(is) objeto(os) serão alvos do select/unselect ")]
        [InspectorOrder(3)]
		public bool m_useMemorizedGo;

		[FullInspector.InspectorHideIf("m_useMemorizedGo"), InspectorOrder(4)]
		public GameObject[] obj;

		[SerializeField, InspectorOrder(5), InspectorName("Select/Unselect")]
		private bool m_set;

        [InspectorComment("Abaixo use o grupo para fazer unselect/lock em todos os elementos do grupo")]
		[SerializeField,InspectorOrder(0), Tooltip("Drag SelectableGroup to here if you want to Lock/Unlock all selectable colliders")]
		private Transform m_groupTransform;
		private bool ShowLockUnlockButton {get {return m_groupTransform != null;}}


		[InspectorShowIf("ShowLockUnlockButton")]
		[SerializeField, InspectorOrder(2)]
		private bool m_unselectAll = false;

		[SerializeField, InspectorTooltip("Check this if you want to lock/unlock the selected group")]
		[InspectorShowIf("ShowLockUnlockButton"), InspectorOrder(1)]
		private bool m_lockGroup;



		public override IEnumerator Execute()
		{
            if(byPass) yield break;
            yield return base.Execute();

			if (m_groupTransform != null) {
                foreach(Transform child in m_groupTransform.GetComponentsInChildren<Transform>())
                {
                    Collider2D col = child.GetComponent<Collider2D>();
                    if(col != null)
                        col.enabled = !m_lockGroup;
                    if(m_unselectAll)
                    {
                        Selectable sel = child.GetComponent<Selectable>();
                        if(sel != null)
                            sel.SetSelected(false);
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
