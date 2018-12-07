using System;
using System.Collections;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Actions
{
    public abstract class BaseAction : IAction
    {
        public bool byPass { get; set; }

        public float delayBeforeExecution { get; set; }

        public bool waitFinish { get; set; }

        protected MonoBehaviour _corotineHolder;

        public virtual IEnumerator Execute()
        {
            if(byPass) yield break;

            if(delayBeforeExecution > 0)
                yield return new WaitForSeconds(delayBeforeExecution);

        }

        public virtual void Initialize(MonoBehaviour coroutineHolder)
        {
            _corotineHolder = coroutineHolder;
        }

#if UNITY_EDITOR
        public IAction Clone()
        {
             return (IAction)this.MemberwiseClone(); 
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
    }
}
