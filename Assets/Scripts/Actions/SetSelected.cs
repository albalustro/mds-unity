using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;
using System;


namespace MDS.Actions{

	[Serializable]
	public class SetSelected : BaseAction
	{

		public GameObject[] obj;

		[SerializeField]
		private bool m_set;

		public override IEnumerator Execute()
		{
			yield return base.Execute();
			for (int i = 0; i < obj.Length; i++) {
				obj[i].GetComponent<Selectable> ().SetSelected (m_set);
			}

		}

	}
}
