using UnityEngine;
using System.Collections;
using FullInspector;
using UnityEngine.SceneManagement;

public class MDSBehaviour : BaseBehavior  
{
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
