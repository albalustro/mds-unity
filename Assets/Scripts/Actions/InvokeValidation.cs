using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using MDS.Core;
using UnityEngine;

public class InvokeValidation : BaseAction
{

    
    public override IEnumerator Execute()
    {
        if(byPass) yield break;
        yield return base.Execute();
        Challenge.GetActiveInstance().ProcessResult();
    }

}