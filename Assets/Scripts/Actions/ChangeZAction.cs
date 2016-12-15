using System.Collections;
using UnityEngine;
using MDS.Core.Interfaces;
using System;



namespace MDS.Actions {


	public class ChangeZAction : BaseAction {

		[SerializeField]
		private Transform[] m_obj; 

		[SerializeField]
		private float m_newZPos;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			for (int i = 0; i < m_obj.Length; i++) {
				m_obj[i].position = new Vector3 (m_obj[i].position.x, m_obj[i].position.y, m_newZPos);
			}

		}


	}
}
