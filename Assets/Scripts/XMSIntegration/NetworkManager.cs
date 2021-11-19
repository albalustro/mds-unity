using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using MDS.ScriptableObjects;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using UnityEngine.Networking;
using ConnectionConfig = MDS.ScriptableObjects.ConnectionConfig;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

public class NetworkManager : Singleton<NetworkManager>
{
	[SerializeField] private ConnectionConfig _devConfig;
	[SerializeField] private ConnectionConfig _homologConfig;
	[SerializeField] private ConnectionConfig _prodConfig;

	private ConnectionConfig _config;
	public ConnectionConfig connectionConfig => _config;


	JsonSerializerSettings settings = new JsonSerializerSettings()
	{
		ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
	};


	protected override void Awake()
	{
		base.Awake();

#if PROD
        _config = _prodConfig;
#else
		_config = _devConfig;
#endif

		if (NetworkManager.Instance != this)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);
	}

	private async Task<string> Post(string url, string bodyJsonString = null)
	{
		using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
		{
			if (UserProfile.Instance.loginInfo != null)
			{
				string authorization = $"Bearer {UserProfile.Instance.loginInfo.token}";
				request.SetRequestHeader("Authorization", authorization);
			}

			if (!string.IsNullOrEmpty(bodyJsonString))
			{
				byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
				request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			}

			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			await request.SendWebRequest();

			return request.downloadHandler.text;
		}
	}

	#region Login
	public async Task<LoginInfo> DoLogin(string user, string pass)
	{
		Scene curScene = SceneManager.GetActiveScene();
		string game = "MDS" + curScene.GetGameIndex().ToString();
		string season = curScene.GetGameIndex().ToString();

		var loginRequest = new LoginRequest()
		{
			Login = user,
			Password = pass,
			Game = game,
			SeasonId = season
		};

		var requestStr = JsonConvert.SerializeObject(loginRequest, settings);
		var resultStr = await Post(connectionConfig.loginURL, requestStr);

		XmsLoginInfo info = JsonConvert.DeserializeObject<XmsLoginInfo>(resultStr);

		return info?.Data;
	}
	#endregion

	#region ConceptMap
	public async Task<ConceptMap> DoSincronize(ConceptMap cm)
	{
		var requestStr = JsonConvert.SerializeObject(cm, settings);
		var resultStr = await Post(connectionConfig.conceptURL, requestStr);

		var newConceptMap = JsonConvert.DeserializeObject<XmsConceptInfo>(resultStr);

		return newConceptMap?.Data;
	}

	#endregion

}
