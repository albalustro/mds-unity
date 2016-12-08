using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;
using System;


namespace MDS.Actions{

	[Serializable]
	public class SetSelected : BaseAction
	{

		public GameObject obj;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			obj.GetComponent<Selectable> ().SetSelected ();

		}

	}
}
