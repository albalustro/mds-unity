using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using UnityEngine;

public class ActionTester : MDSBehaviour
{

    public IAction[] actions;

    public void OnMouseUp()
    {
        ExecuteActions(actions);

    }
}
