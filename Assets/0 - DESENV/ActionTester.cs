using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using UnityEngine;

public class ActionTester : MDSBehaviour
{

    public IAction[] actions;

    public IEnumerator OnMouseUp()
    {
        for(int i = 0; i < actions.Length; i++)
        {
            if (actions[i].waitFinish)
                yield return StartCoroutine(actions[i].Execute());
            else
                StartCoroutine(actions[i].Execute());
        }

    }
}
