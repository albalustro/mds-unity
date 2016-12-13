using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Actions
{
    public abstract class BaseAction : IAction
    {

        public float delayBeforeExecution { get; set; }
       
        public bool waitFinish { get; set; }

        protected MonoBehaviour _corotineHolder;

        public virtual IEnumerator Execute()
        {
            if(delayBeforeExecution > 0)
                yield return new WaitForSeconds(delayBeforeExecution);

        }

        public void Initialize(MonoBehaviour coroutineHolder)
        {
            _corotineHolder = coroutineHolder;
        }

    }
}
