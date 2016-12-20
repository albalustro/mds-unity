using System.Collections;
using UnityEngine;
using MDS.Core.Interfaces;
using System;


namespace MDS.Actions {


	public class MakeParentAction : BaseAction {

		[SerializeField]
		private Transform m_obj;

		[SerializeField]
		private Transform m_parent;

		[SerializeField]
		private bool m_resetPosition;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			if (m_parent == null) {
				m_obj.parent = null;
			} else {
				m_obj.SetParent (m_parent);
				if (m_resetPosition) {
					m_obj.position = m_parent.position;
				}
			}

		}

	}
}
