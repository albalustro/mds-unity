using UnityEngine;
using System.Collections;
using System;
using MDS.ScriptableObjects;

public class ConceptSyncer : Singleton<ConceptSyncer>
{

    private Action<ConceptMap> sendConceptCallback;

    protected override void Awake()
    {
        base.Awake();

        if(ConceptSyncer.Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }



    public void SendConceptMapToServer(string token, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		ConnectionManager.Instance.DoSincronize (token, cm, ReceiveConceptMapFromServerCallback);
	}

	public void ReceiveConceptMapFromServerCallback(ConceptMap s)
	{
		sendConceptCallback (s);
	}
}
