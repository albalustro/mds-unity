using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public class ConnectionManager : Singleton<ConnectionManager> 
{
	private string _url;
	private Action<LoginInfo> doLoginCallback;
	private Action<ConceptMap> sendConceptCallback;

	#region Login
	public void DoLogin(string user, string pass, Action<LoginInfo> callback)
	{
		doLoginCallback = callback;
		_url = "https://stage-xms.xmile.com.br/api/gamelogin";
		WWWForm loginForm = new WWWForm();
		loginForm.AddField("login", user);
		loginForm.AddField("password", pass);
		loginForm.AddField("game", "4");
		loginForm.AddField("season_id", "1");
		WWW www = new WWW(_url, loginForm);
		StartCoroutine(ValidateLogin(www));
	}
		
	IEnumerator ValidateLogin(WWW www)
	{
		LoginInfo info = new LoginInfo ();
		//Tentando logar online
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
	public void DoSincronize(string l, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		_url = "http://localhost/xms.php";
		WWWForm conceptForm = new WWWForm();
		conceptForm.AddField("login", l);
		conceptForm.AddField("conceptMap", JsonConvert.SerializeObject(cm));
		WWW www = new WWW(_url, conceptForm);
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
