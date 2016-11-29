using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;

namespace MDS.Actions
{
    [Serializable]
    public class FreezeAction : IAction
    {

        public GameObject[] obj;

        public IEnumerator Execute(Action callback = null)
        {
			for (int i = 0; i < obj.Length; i++) {
				obj[i].GetComponent<Collider2D>().enabled = false;
			}
            yield return null;
        }
    }
}
