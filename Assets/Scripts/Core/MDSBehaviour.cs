using UnityEngine;
using System.Collections;
using FullInspector;
using UnityEngine.SceneManagement;

public class MDSBehaviour : BaseBehavior  
{
    protected void LogError(string msg)
    {
        Debug.LogErrorFormat("{0} : {1}", SceneManager.GetActiveScene().name, msg);
    }

	protected void Log(string msg)
	{
		Debug.LogFormat("{0} : {1}", gameObject.name, msg);
	}

}
