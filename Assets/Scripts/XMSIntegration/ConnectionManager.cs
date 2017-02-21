using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using MDS.ScriptableObjects;
using UnityEngine.SceneManagement;
using MDS.Utilities;

public class ConnectionManager : Singleton<ConnectionManager>
{
    public ConnectionConfig connectionConfig { get { return _config; } }

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
        Scene curScene = SceneManager.GetActiveScene();
        string game = "MDS" + curScene.GetGameIndex().ToString();
        string season = curScene.GetGameIndex().ToString();

		doLoginCallback = callback;
		WWWForm loginForm = new WWWForm();
		loginForm.AddField("login", user);
		loginForm.AddField("password", pass);
		loginForm.AddField("game", game);
		loginForm.AddField("season_id", season);

        //Log(_config.loginURL);
        //Log(loginForm.ToString());
        //Log("game: " + game);
        //Log("season_id: " + season);
        //Log("login: " + user);
        //Log("pass: " + pass);

        
		WWW www = new WWW(_config.loginURL, loginForm);
		StartCoroutine(ValidateLogin(www));
	}
		
	IEnumerator ValidateLogin(WWW www)
	{
		LoginInfo info = new LoginInfo ();
		yield return www;
        if(www.error == null)
        {
            string wsReturn = www.text.Trim();
           // Log("wsReturn: " + wsReturn);
            info = JsonConvert.DeserializeObject<LoginInfo>(wsReturn);
           // Log("info é nulo?? : " + (info == null).ToString());
        }
        else
        {
            LogError("Erro: " + www.error);
            info = null;
        }
		doLoginCallback(info);
	}
	#endregion

	#region ConceptMap
	public void DoSincronize(string token, ConceptMap cm, Action<ConceptMap> callback)
	{
		sendConceptCallback = callback;
		WWWForm conceptForm = new WWWForm();
		conceptForm.AddField("token", token);
        string json = JsonConvert.SerializeObject(cm);
        conceptForm.AddField("conceptMap", json);
		WWW www = new WWW(_config.conceptURL, conceptForm);
		StartCoroutine(SincronizeConcept(www));
	}
		
	IEnumerator SincronizeConcept(WWW www)
	{
		ConceptMap cm;
		yield return www;
        if(www.error == null)
        {
            string wsReturn = www.text.Trim();
            cm = JsonConvert.DeserializeObject<ConceptMap>(wsReturn);
        }
        else
        {
            LogError("Erro: " +www.error);
            cm = null;
        }
		sendConceptCallback (cm);
	}
	#endregion

}
