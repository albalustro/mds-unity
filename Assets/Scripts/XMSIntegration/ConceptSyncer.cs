using UnityEngine;
using System.Collections;
using System;

public class ConceptSyncer : Singleton<ConceptSyncer>
{
	private Action<ConceptMap> sendConceptCallback;

	public void SendConceptMapToServer(string login, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		ConnectionManager.Instance.DoSincronize (login, cm, ReceiveConceptMapFromServer);
	}

	public void ReceiveConceptMapFromServer(ConceptMap s)
	{
		sendConceptCallback (s);
	}
}
