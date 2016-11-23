using UnityEngine;
using System.Collections;
using System;

namespace MDS.Core.Interfaces
{

    public interface IAction
    {

        IEnumerator Execute(Action callback = null);

    }
}