using UnityEngine;
using System.Collections;
using FullInspector;
using UnityEngine.SceneManagement;
using MDS.Core.Interfaces;

public class MDSBehaviour : BaseBehavior  
{

    protected void ExecuteActions(IAction[] a)
    {
        StartCoroutine(exec(a));
    }


    private IEnumerator exec(IAction[] a)
    {
        for(int i = 0; i < a.Length; i++)
        {
            if(a[i] == null)
            {
                LogError("Action não definida.");
                continue;
            }

            if(a[i].waitFinish)
                yield return StartCoroutine(a[i].Execute());
            else
                StartCoroutine(a[i].Execute());
        }
    }

    protected void LogError(string msg)
    {
        Debug.LogErrorFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

    protected void LogWarning(string msg)
    {
        Debug.LogWarningFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

    protected void Log(string msg)
    {
        Debug.LogFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

}
