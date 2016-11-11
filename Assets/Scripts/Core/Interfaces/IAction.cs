using UnityEngine;
using System.Collections;
using System;

public interface IAction {

    IEnumerator Execute(Action callback = null);

}
