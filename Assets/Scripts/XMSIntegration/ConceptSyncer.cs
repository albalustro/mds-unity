using UnityEngine;
using System.Collections;
using System;
using MDS.ScriptableObjects;

public class ConceptSyncer : Singleton<ConceptSyncer>
{
    private ConnectionConfig _config;
    private Action<ConceptMap> sendConceptCallback;

    public void Initialize(ConnectionConfig config)
    {
        _config = config;
    }

    public void SendConceptMapToServer(string token, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		ConnectionManager.Instance.DoSincronize (token, cm, ReceiveConceptMapFromServer);
	}

	public void ReceiveConceptMapFromServer(ConceptMap s)
	{
		sendConceptCallback (s);
	}
}
