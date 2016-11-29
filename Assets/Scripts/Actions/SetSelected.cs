using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;
using System;


namespace MDS.Actions{

	[Serializable]
	public class SetSelected : IAction {

		public GameObject obj;

		public IEnumerator Execute(Action callback = null)
		{
			yield return new WaitForSeconds (0.1f);
			obj.GetComponent<Selectable> ().SetSelected ();
			yield return null;
		}

	}
}
