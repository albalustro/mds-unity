using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;

namespace MDS.Actions
{
	[Serializable]
	public class FreezeUnfreezeAction : BaseAction
	{

		public GameObject[] obj;
		public bool unfreeze = false;

		public override IEnumerator Execute()
		{
			yield return base.Execute();

			for(int i = 0; i < obj.Length; i++) {
				obj[i].GetComponent<Collider2D>().enabled = unfreeze;
			}

		}
	}
}
