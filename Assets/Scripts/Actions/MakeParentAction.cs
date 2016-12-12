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

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			m_obj.SetParent (m_parent);

		}

	}
}
