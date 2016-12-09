using UnityEngine;
using System.Collections;
using System;

namespace MDS.Core.Interfaces
{

    public interface IAction
    {

        bool waitFinish { get; set; }
        float delayBeforeExecution { get; set; }
        IEnumerator Execute();

        void Initialize(MonoBehaviour coroutineEmiter);

    }
}