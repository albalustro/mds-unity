using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using MDS.Core.ProcessActivator;
using UnityEngine;

public class StopTimer : BaseAction
{
	public ProcessAnswerTimer timer;
	public override IEnumerator Execute()
	{
        if(byPass) yield break;
        yield return base.Execute();
		timer.Disable();
	}

}
