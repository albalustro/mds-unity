using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MDS.Core.Interfaces;
using System;
using MDS.Actions;


public class CallBehaviourAction : BaseAction {


	public UnityEvent m_callFunctionOnGameObject;

	public override IEnumerator Execute()
	{
        if(byPass) yield break;
        yield return base.Execute();
		m_callFunctionOnGameObject.Invoke ();
	}

}
