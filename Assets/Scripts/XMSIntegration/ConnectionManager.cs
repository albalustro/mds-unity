using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using MDS.ScriptableObjects;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using UnityEngine.Networking;

public class ConnectionManager : Singleton<ConnectionManager>
{
    public ConnectionConfig connectionConfig
    {
        get
        {
            return _config;
        }
    }

    [SerializeField]
    private ConnectionConfig _homologConfig;

    [SerializeField]
    private ConnectionConfig _prodConfig;

    [SerializeField]
    private ConnectionConfig _config;

    private Action<LoginInfo> doLoginCallback;
    private Action<ConceptMap> sendConceptCallback;

    protected override void Awake()
    {
        base.Awake();

        _config = _prodConfig;

        if(ConnectionManager.Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void SetHomolgConfig()
    {
        _config = _homologConfig;
        Debug.Log("Configuracao de conexao com homologacao.");
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


        var www = UnityWebRequest.Post(connectionConfig.loginURL, loginForm);
        StartCoroutine(ValidateLogin(www));
    }

    IEnumerator ValidateLogin(UnityWebRequest www)
    {
        LoginInfo info = new LoginInfo();
        yield return www;
        if(www.error == null)
        {
            string wsReturn = www.downloadHandler.text.Trim();
            info = JsonConvert.DeserializeObject<LoginInfo>(wsReturn);
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
        WWW www = new WWW(connectionConfig.conceptURL, conceptForm);
        StartCoroutine(SincronizeConcept(www));
    }

    IEnumerator SincronizeConcept(WWW www)
    {
        ConceptMap cm;
        yield return www;

        // TIMEOUT, para o futuro, se necessario
        //while(!www.isDone)
        //{
        //    if(timer > timeOut) { failed = true; break; }
        //    timer += Time.deltaTime;
        //    yield return null;
        //}

        if(www.error == null)
        {
            string wsReturn = www.text.Trim();
            cm = JsonConvert.DeserializeObject<ConceptMap>(wsReturn);
        }
        else
        {
            LogError("Erro: " + www.error);
            cm = null;
        }
        sendConceptCallback(cm);
    }
    #endregion

}
