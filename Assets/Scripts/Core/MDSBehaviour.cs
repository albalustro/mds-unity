using UnityEngine;
using System.Collections;
using FullInspector;
using UnityEngine.SceneManagement;
using MDS.Core.Interfaces;

public class MDSBehaviour : BaseBehavior  
{

    public static void ExternalExecuteActions(MDSBehaviour actionExecutioner, IAction[] actions)
    {
        actionExecutioner.ExecuteActions(actions);
    }

    protected void ExecuteActions(IAction[] a)
    {
        if(a != null)
        {
            foreach(var action in a)
            {
                if(action == null)
                    LogError("Action nula no vetor");
                else
                    action.Initialize(this);
            }
            StartCoroutine(exec(a));
        }
    }


    protected virtual IEnumerator exec(IAction[] a)
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

	protected void PlaySFX(AudioClip clip)
	{
		AudioController.Instance.PlaySoundFX (clip);
	}

    // colocado como public para que as actions tenham acesso ao metodo..
    public void LogError(string msg)
    {
        Debug.LogErrorFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

    public void LogWarning(string msg)
    {
        Debug.LogWarningFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

    public void Log(string msg)
    {
        Debug.LogFormat("Cena: {0} | GameObject: {2} => {1}", SceneManager.GetActiveScene().name, msg, gameObject.name);
    }

}
