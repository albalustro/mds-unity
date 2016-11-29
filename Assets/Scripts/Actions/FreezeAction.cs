using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;

namespace MDS.Actions
{
    [Serializable]
    public class FreezeAction : IAction
    {

        public GameObject obj;

        public IEnumerator Execute(Action callback = null)
        {
            obj.GetComponent<Collider2D>().enabled = false;
            yield return null;
        }
    }
}
