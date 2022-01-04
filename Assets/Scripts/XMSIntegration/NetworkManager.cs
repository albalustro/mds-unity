using UnityEngine;
using Newtonsoft.Json;
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

	private async Task<NetworkResponse> Post(string url, string bodyJsonString = null)
	{
		using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
		{
			if (UserProfile.Instance.loginInfo != null)
			{
				string authorization = $"Bearer {UserProfile.Instance.loginInfo.Token}";
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

			return new NetworkResponse() { Result = request.result, HttpCode = request.responseCode, Body = request.downloadHandler.text };
		}
	}

	#region Login
	public async Task<LoginInfo> DoLogin(string user, string pass)
	{
		Scene curScene = SceneManager.GetActiveScene();
		string season = curScene.GetGameIndex().ToString();

		var loginRequest = new LoginRequest()
		{
			Login = user,
			Password = pass,
			SeasonId = season
		};

		var requestStr = JsonConvert.SerializeObject(loginRequest, settings);
		var postResult = await Post(connectionConfig.loginURL, requestStr);

		switch (postResult.Result)
		{
			case UnityWebRequest.Result.ConnectionError:
			case UnityWebRequest.Result.ProtocolError:
			case UnityWebRequest.Result.DataProcessingError:
				return null;
		}

		XmsLoginInfo info = JsonConvert.DeserializeObject<XmsLoginInfo>(postResult.Body);

		return info?.Data;
	}

	#endregion

	#region ConceptMap

	public Task RefreshConceptMap()
	{

	}

	public async Task<ConceptMap> DoSincronize(ConceptMap cm)
	{
		var requestStr = JsonConvert.SerializeObject(cm, settings);
		var postResult = await Post(connectionConfig.conceptURL, requestStr);

		switch (postResult.Result)
		{
			case UnityWebRequest.Result.ConnectionError:
			case UnityWebRequest.Result.ProtocolError:
			case UnityWebRequest.Result.DataProcessingError:
				return null;
		}

		var newConceptMap = JsonConvert.DeserializeObject<XmsConceptInfo>(postResult.Body);

		return newConceptMap?.Data;
	}

	#endregion


	private class NetworkResponse
	{
		public long HttpCode { get; set; }

		public UnityWebRequest.Result Result { get; set; }

		public string Body { get; set; }
	}

}
