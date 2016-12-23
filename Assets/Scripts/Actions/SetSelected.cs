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

//		[InspectorHideIf("Hide_m_useMemorizedGo")]
		public bool m_useMemorizedGo;
//		private bool Hide_m_useMemorizedGo { get { return obj != null;} }

		[FullInspector.InspectorHideIf("m_useMemorizedGo")]
		public GameObject[] obj;


		[SerializeField]
		private bool m_set;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

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
