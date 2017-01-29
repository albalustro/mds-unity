using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using MDS.ScriptableObjects;

public class ConnectionManager : Singleton<ConnectionManager> 
{
    [SerializeField]
    private ConnectionConfig _config;

	private Action<LoginInfo> doLoginCallback;
	private Action<ConceptMap> sendConceptCallback;

    protected override void Awake()
    {
        base.Awake();

        if(ConnectionManager.Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }


    #region Login
    public void DoLogin(string user, string pass, Action<LoginInfo> callback)
	{
		doLoginCallback = callback;
		WWWForm loginForm = new WWWForm();
		loginForm.AddField("login", user);
		loginForm.AddField("password", pass);
		loginForm.AddField("game", "4");
		loginForm.AddField("season_id", "1");
		WWW www = new WWW(_config.loginURL, loginForm);
		StartCoroutine(ValidateLogin(www));
	}
		
	IEnumerator ValidateLogin(WWW www)
	{
		LoginInfo info = new LoginInfo ();
		yield return www;
		if (www.error == null)
		{
			string wsReturn = www.text.Trim ();
			info = JsonConvert.DeserializeObject<LoginInfo> (wsReturn);
		}
		else
			info = null;
		doLoginCallback(info);
	}
	#endregion

	#region ConceptMap
	public void DoSincronize(string token, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		WWWForm conceptForm = new WWWForm();
		conceptForm.AddField("token", token);
        conceptForm.AddField("conceptMap", JsonConvert.SerializeObject(cm));
		WWW www = new WWW(_config.conceptURL, conceptForm);
		StartCoroutine(SincronizeConcept(www));
	}
		
	IEnumerator SincronizeConcept(WWW www)
	{
		ConceptMap cm;
		yield return www;
		if (www.error == null)
		{
			string wsReturn = www.text.Trim ();
			cm = JsonConvert.DeserializeObject<ConceptMap> (wsReturn);
		}
		else
			cm = null;
		sendConceptCallback (cm);
	}
	#endregion

}
