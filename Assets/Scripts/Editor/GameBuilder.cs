using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameBuilder : EditorWindow
{

	bool _building = false;
	bool _onlySetup;

	public enum Game
	{
		MDS1, MDS2, MDS3
	}


	[MenuItem("OyoLabs/Distribution Builder")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		GameBuilder window = (GameBuilder)EditorWindow.GetWindow(typeof(GameBuilder));
		window.Show();
	}

	void OnGUI()
	{
		GUI.enabled = !_building;
		//if (!_building)
		{
			GUILayout.Space(3);
			PlayerSettings.bundleVersion = EditorGUILayout.TextField("Version", PlayerSettings.bundleVersion);
			PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Build Number", PlayerSettings.Android.bundleVersionCode);
			PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
			_onlySetup = EditorGUILayout.Toggle("Only Setup (do not build)", _onlySetup);
			GUILayout.Space(5);

			GUILayout.BeginHorizontal("Box");
			GUILayout.Space(5);
			{
				GUILayout.BeginVertical("Box");
				GUILayout.Label("Standalone");
				{
					GUILayout.Space(5);
					//if (GUILayout.Button("Assetbundles"))
					//{
					//if (!Confirm("ASSETBUNDLES - WINDOWS")) return;
					//BuildAssetbundlesStandalone();
					//}
					if (GUILayout.Button("MDS1"))
					{
						if (!Confirm("MDS1 - STANDALONE")) return;
						Build_MDS_Standalone(Game.MDS1);
					}
					if (GUILayout.Button("MDS2"))
					{
						if (!Confirm("MDS2 - STANDALONE")) return;
						Build_MDS_Standalone(Game.MDS2);
					}
					if (GUILayout.Button("MDS3"))
					{
						if (!Confirm("MDS3 - STANDALONE")) return;
						Build_MDS_Standalone(Game.MDS3);
					}
					if (GUILayout.Button("ALL Standalone"))
					{
						if (!Confirm("ALL - STANDALONE")) return;
						BuildAllStandalone();
					}
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("Box");
				GUILayout.Label("WebGL");
				{
					GUILayout.Space(5);

					if (GUILayout.Button("Assetbundles"))
					{
						if (!Confirm("ASSETBUNDLES - WEBGL")) return;
						BuildAssetbundlesWebGL();

					}
					if (GUILayout.Button("MDS1"))
					{
						if (!Confirm("MDS1 - WEBGL")) return;
						Build_MDS_WebGL(Game.MDS1);
					}
					if (GUILayout.Button("MDS2"))
					{
						if (!Confirm("MDS2 - WEBGL")) return;
						Build_MDS_WebGL(Game.MDS2);
					}
					if (GUILayout.Button("MDS3"))
					{
						if (!Confirm("MDS3 - WEBGL")) return;
						Build_MDS_WebGL(Game.MDS3);
					}
					if (GUILayout.Button("ALL WebGL"))
					{
						if (!Confirm("ALL - WebGL")) return;
						BuildAllWebGL();
					}

				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("box");
				GUILayout.Label("Android");
				{
					GUILayout.Space(5);

					//if (GUILayout.Button("Assetbundles"))
					//{
					//	if (!Confirm("ASSETBUNDLES - ANDROID")) return;
					//	//BuildAssetbundlesAndroid();

					//}
					if (GUILayout.Button("MDS1"))
					{
						if (!Confirm("MDS1 - ANDROID")) return;
						Build_MDS_Android(Game.MDS1);
					}
					if (GUILayout.Button("MDS2"))
					{
						if (!Confirm("MDS2 - ANDROID")) return;
						Build_MDS_Android(Game.MDS2);
					}
					if (GUILayout.Button("MDS3"))
					{
						if (!Confirm("MDS3 - ANDROID")) return;
						Build_MDS_Android(Game.MDS3);
					}
					if (GUILayout.Button("ALL Android"))
					{
						if (!Confirm("ALL - Android")) return;
						BuildAllAndroid();

					}

				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("box");
				GUILayout.Label("iOS");
				{
					GUILayout.Space(5);
					//if (GUILayout.Button("Assetbundles")) { }
					if (GUILayout.Button("MDS1"))
					{
						if (!Confirm("MDS1 - iOS")) return;
						Build_MDS_iOS(Game.MDS1);
					}
					if (GUILayout.Button("MDS2"))
					{
						if (!Confirm("MDS2 - iOS")) return;
						Build_MDS_iOS(Game.MDS2);
					}
					if (GUILayout.Button("MDS3"))
					{
						if (!Confirm("MDS3 - iOS")) return;
						Build_MDS_iOS(Game.MDS3);

					}
					if (GUILayout.Button("ALL iOS"))
					{
						if (!Confirm("ALL - iOS")) return;
						BuildAlliOS();
					}

				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal("Box");
			GUILayout.Space(5);
			{
				GUILayout.BeginVertical("Box");
				GUILayout.Label("All");
				{
					GUILayout.Space(5);
					//if (GUILayout.Button("All Bundles"))
					//{
					//	if (!Confirm("Assetbundles - ALL")) return;
					//	// BuildAssetbundlesALL();

					//}
					if (GUILayout.Button("All MDS1"))
					{
						if (!Confirm("MDS1 - ALL")) return;
						Build_MDS_All(Game.MDS1);
					}
					if (GUILayout.Button("All MDS2"))
					{
						if (!Confirm("MDS2 - ALL")) return;
						Build_MDS_All(Game.MDS2);
					}
					if (GUILayout.Button("All MDS3"))
					{
						if (!Confirm("MDS3 - ALL")) return;
						Build_MDS_All(Game.MDS3);
					}
					if (GUILayout.Button("All ALL"))
					{
						if (!Confirm("All - All")) return;
						BuildAll();
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();


		}

		GUI.enabled = true;
	}

	private void BuildAll()
	{
		BuildAllStandalone();
		BuildAllAndroid();
		BuildAllWebGL();
	}

	private void Build_MDS_All(Game game)
	{
		Build_MDS_Standalone(game);
		Build_MDS_Android(game);
		Build_MDS_WebGL(game);
	}

	//private void BuildAssetbundlesALL()
	//{
	//    BuildAssetbundlesStandalone();
	//    BuildAssetbundlesWebGL();
	//    BuildAssetbundlesAndroid();
	//}

	private void BuildAllAndroid()
	{
		// BuildAssetbundlesAndroid();
		Build_MDS_Android(Game.MDS1);
		Build_MDS_Android(Game.MDS2);
		Build_MDS_Android(Game.MDS3);
	}

	private void BuildAlliOS()
	{
		Build_MDS_iOS(Game.MDS1);
		Build_MDS_iOS(Game.MDS2);
		Build_MDS_iOS(Game.MDS3);
	}
	private void Build_MDS_Android(Game game)
	{
		Debug.LogFormat("[{0}][Android][{1}] - Starting ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		SetGame(game);
		SetAndroidConfig(game);
		BuildPlayerOptions options = SetupBuildAndroid(game);
		SetDefineSymbols(game);

		EditorBuildSettings.scenes = (from s in options.scenes
									  select new EditorBuildSettingsScene(s, true)).ToArray();

		if (_onlySetup)
		{
			Debug.LogFormat("[{0}][WebGL][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
			return;
		}

		//var ret = BuildPipeline.BuildPlayer(options);
		//if (!string.IsNullOrEmpty(ret))
		//	Debug.LogWarning(ret);
		Debug.LogFormat("[{0}][Android][{1}] - Building report ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		BuildReport(options);
		Debug.LogFormat("[{0}][Android][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
	}

	private void Build_MDS_iOS(Game game)
	{
		Debug.LogFormat("[{0}][iOS][{1}] - Starting ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		SetGame(game);
		SetiOSConfig(game);
		BuildPlayerOptions options = SetupBuildiOS(game);
		SetDefineSymbols(game);

		EditorBuildSettings.scenes = (from s in options.scenes
									  select new EditorBuildSettingsScene(s, true)).ToArray();

		if (_onlySetup)
		{
			Debug.LogFormat("[{0}][iOS][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
			return;
		}

		//var ret = BuildPipeline.BuildPlayer(options);
		//if (!string.IsNullOrEmpty(ret))
		//	Debug.LogWarning(ret);
		Debug.LogFormat("[{0}][iOS][{1}] - Building report ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		BuildReport(options);
		Debug.LogFormat("[{0}][iOS][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
	}
	
	private static void BuildReport(BuildPlayerOptions options)
	{
		BuildReportTool.Util.ShouldSaveGottenBuildReportNow = true;
		string reportPath = BuildReportTool.ReportGenerator.CreateReport(options);
		string outputPath = options.locationPathName + ".report.xml";
		if (File.Exists(outputPath))
			File.Delete(outputPath);
		File.Copy(reportPath, outputPath);
	}

	//private void BuildAssetbundlesAndroid()
	//{
	//    Debug.Log("[Assetbundles][Android][" + System.DateTime.Now.ToShortTimeString() + "] - Starting ");
	//    var window = GetWindow<AssetBundleBrowserMain>();
	//    window.titleContent = new GUIContent("AssetBundles");
	//    window.Show();
	//    window.m_Mode = AssetBundleBrowserMain.Mode.Builder;
	//    window.m_BuildTab.m_BuildTarget = AssetBundleBuildTab.ValidBuildTarget.Android;
	//    window.m_BuildTab.SetDefaultOutputPath();
	//    window.m_BuildTab.m_Compression = AssetBundleBuildTab.CompressOptions.StandardCompression;
	//    window.m_BuildTab.ExecuteBuild();


	//    Debug.Log("[Assetbundles][Android][" + System.DateTime.Now.ToShortTimeString() + "] - Creating version json ");

	//    AssetbundlesVersion abVersion = new AssetbundlesVersion();

	//    List<string> manifestPaths = new List<string>(Directory.GetFiles(window.m_BuildTab.m_OutputPath, "*.manifest", SearchOption.TopDirectoryOnly));
	//    foreach (var manifestFilePath in manifestPaths)
	//    {
	//        if (manifestFilePath.Contains("Android.manifest")) continue;
	//        var filePath = Path.Combine(Application.dataPath + "/..", manifestFilePath);
	//        var fi = new FileInfo(filePath);
	//        var fileName = fi.Name.Replace(".manifest", "");
	//        var lines = File.ReadAllLines(filePath);
	//        string hashString = lines[5].Substring(10, 32);
	//        abVersion.GetType().GetProperty(fileName).SetValue(abVersion, hashString, null);
	//    }

	//    string jsonPath = Path.Combine(Application.dataPath + "/..", window.m_BuildTab.m_OutputPath);
	//    jsonPath = Path.Combine(jsonPath, "bundles.json");
	//    string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(abVersion);
	//    File.WriteAllText(jsonPath, jsonContent);
	//    Debug.Log("[Assetbundles][Android][" + System.DateTime.Now.ToShortTimeString() + "] - Done ");
	//}

	private void BuildAllWebGL()
	{
		BuildAssetbundlesWebGL();
		Build_MDS_WebGL(Game.MDS1);
		Build_MDS_WebGL(Game.MDS2);
		Build_MDS_WebGL(Game.MDS3);
	}

	private void Build_MDS_WebGL(Game game)
	{
		Debug.LogFormat("[{0}][WebGL][{1}] - Starting ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		SetGame(game);
		BuildPlayerOptions options = SetupBuildWebGL(game);
		SetDefineSymbols(game);

		EditorBuildSettings.scenes = (from s in options.scenes
									  select new EditorBuildSettingsScene(s, true)).ToArray();

		if (_onlySetup)
		{
			Debug.LogFormat("[{0}][WebGL][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
			return;
		}

		//var ret = BuildPipeline.BuildPlayer(options);
		//if (!string.IsNullOrEmpty(ret))
		//	Debug.LogWarning(ret);
		Debug.LogFormat("[{0}][WebGL][{1}] - Building report ", game.ToString(), System.DateTime.Now.ToShortTimeString());
		BuildReport(options);
		Debug.LogFormat("[{0}][WebGL][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
	}

	private void BuildAssetbundlesWebGL()
	{
		Debug.LogFormat("[{0}][WebGL][{1}] - Starting ", "ASSETBUNDLES", System.DateTime.Now.ToShortTimeString());
		string path = string.Format("AssetBundles/prod/{0}/WebGL", PlayerSettings.bundleVersion);
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
		Debug.LogFormat("[{0}][WebGL][{1}] - Done ", "ASSETBUNDLES", System.DateTime.Now.ToShortTimeString());

		//Debug.Log("[Assetbundles][WebGL][" + System.DateTime.Now.ToShortTimeString() + "] - Starting ");
		//var window = GetWindow<AssetBundleBrowserMain>();
		//window.titleContent = new GUIContent("AssetBundles");
		//window.Show();
		//window.m_Mode = AssetBundleBrowserMain.Mode.Builder;
		//window.m_BuildTab.m_BuildTarget = AssetBundleBuildTab.ValidBuildTarget.WebGL;
		//window.m_BuildTab.SetDefaultOutputPath();
		//window.m_BuildTab.m_Compression = AssetBundleBuildTab.CompressOptions.StandardCompression;
		//window.m_BuildTab.ExecuteBuild();

		//Debug.Log("[Assetbundles][WebGL][" + System.DateTime.Now.ToShortTimeString() + "] - Creating version json ");

		//AssetbundlesVersion abVersion = new AssetbundlesVersion();

		//List<string> manifestPaths = new List<string>(Directory.GetFiles(window.m_BuildTab.m_OutputPath, "*.manifest", SearchOption.TopDirectoryOnly));
		//foreach (var manifestFilePath in manifestPaths)
		//{
		//    if (manifestFilePath.Contains("WebGL.manifest")) continue;
		//    var filePath = Path.Combine(Application.dataPath + "/..", manifestFilePath);
		//    var fi = new FileInfo(filePath);
		//    var fileName = fi.Name.Replace(".manifest", "");
		//    var lines = File.ReadAllLines(filePath);
		//    string hashString = lines[5].Substring(10, 32);
		//    abVersion.GetType().GetProperty(fileName).SetValue(abVersion, hashString, null);
		//}

		//string jsonPath = Path.Combine(Application.dataPath + "/..", window.m_BuildTab.m_OutputPath);
		//jsonPath = Path.Combine(jsonPath, "bundles.json");
		//string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(abVersion);
		//File.WriteAllText(jsonPath, jsonContent);

		//Debug.Log("[Assetbundles][WebGL][" + System.DateTime.Now.ToShortTimeString() + "] - Done ");
	}

	private void BuildAllStandalone()
	{
		// BuildAssetbundlesStandalone();
		Build_MDS_Standalone(Game.MDS1);
		Build_MDS_Standalone(Game.MDS2);
		Build_MDS_Standalone(Game.MDS3);
	}

	private void Build_MDS_Standalone(Game game)
	{
		Debug.Log(string.Format("[{0}][Standalone][{1}] - Starting ", game.ToString(), System.DateTime.Now.ToShortTimeString()));
		SetGame(game);
		
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, string.Format("com.OyoLabs.{0}", game.ToString())); 
		
		BuildPlayerOptions options = SetupBuildStandAlone(game);
		SetDefineSymbols(game);

		if (_onlySetup)
		{
			Debug.LogFormat("[{0}][WebGL][{1}] - Done ", game.ToString(), System.DateTime.Now.ToShortTimeString());
			return;
		}

		//var ret = BuildPipeline.BuildPlayer(options);
		//if (!string.IsNullOrEmpty(ret))
		//	Debug.LogWarning(ret);
		Debug.Log(string.Format("[{0}][Standalone][{1}] - Building Report ", game.ToString(), System.DateTime.Now.ToShortTimeString()));
		BuildReport(options);
		Debug.Log(string.Format("[{0}][Standalone][{1}] - Starting ", game.ToString(), System.DateTime.Now.ToShortTimeString()));
	}


	//private void BuildAssetbundlesStandalone()
	//{
	//    Debug.Log("[Assetbundles][Standalone][" + System.DateTime.Now.ToShortTimeString() + "] - Starting");
	//    var window = GetWindow<AssetBundleBrowserMain>();
	//    window.titleContent = new GUIContent("AssetBundles");
	//    window.Show();
	//    window.m_Mode = AssetBundleBrowserMain.Mode.Builder;
	//    window.m_BuildTab.m_BuildTarget = AssetBundleBuildTab.ValidBuildTarget.StandaloneWindows;
	//    window.m_BuildTab.SetDefaultOutputPath();
	//    window.m_BuildTab.m_Compression = AssetBundleBuildTab.CompressOptions.StandardCompression;
	//    window.m_BuildTab.ExecuteBuild();

	//    Debug.Log("[Assetbundles][Standalone][" + System.DateTime.Now.ToShortTimeString() + "] - Creating version json");

	//    AssetbundlesVersion abVersion = new AssetbundlesVersion();

	//    List<string> manifestPaths = new List<string>(Directory.GetFiles(window.m_BuildTab.m_OutputPath, "*.manifest", SearchOption.TopDirectoryOnly));
	//    foreach (var manifestFilePath in manifestPaths)
	//    {
	//        if (manifestFilePath.Contains("StandaloneWindows.manifest")) continue;
	//        var filePath = Path.Combine(Application.dataPath + "/..", manifestFilePath);
	//        var fi = new FileInfo(filePath);
	//        var fileName = fi.Name.Replace(".manifest", "");
	//        var lines = File.ReadAllLines(filePath);
	//        string hashString = lines[5].Substring(10, 32);
	//        abVersion.GetType().GetProperty(fileName).SetValue(abVersion, hashString, null);
	//    }

	//    string jsonPath = Path.Combine(Application.dataPath + "/..", window.m_BuildTab.m_OutputPath);
	//    jsonPath = Path.Combine(jsonPath, "bundles.json");
	//    string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(abVersion);
	//    File.WriteAllText(jsonPath, jsonContent);
	//    Debug.Log("[Assetbundles][Standalone][" + System.DateTime.Now.ToShortTimeString() + "] - Done");
	//}

	private bool Confirm(string msg)
	{
		return EditorUtility.DisplayDialog("Build", "Iniciar o build de " + msg + "?", "SIM", "Cancelar");
	}

	private void SetAndroidConfig(Game game)
	{
		//Android
		//com.OyoLabs.MisteriodosSonhos1
		//com.OyoLabs.MisteriodosSonhos2
		//com.OyoLabs.MisteriodosSonhos3
		
		string alias = string.Empty;
		string identifier = string.Empty;
		switch (game)
		{
			case Game.MDS1:
				alias = "mds1";
				identifier = "MisteriodosSonhos1";
				break;
			case Game.MDS2:
				alias = "misterio dos sonhos 2";
				identifier = "MisteriodosSonhos2";
				break;
			case Game.MDS3:
				alias = "misterio dos sonhos 3";
				identifier = "MisteriodosSonhos3";
				break;
		}
		
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, string.Format("com.OyoLabs.{0}", identifier));
		// PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", string.Format("/Keys/{0}.keystore", game.ToString().ToLower()));
		// PlayerSettings.Android.keystorePass = "xmile1@3";
		//
		// PlayerSettings.Android.keyaliasName = alias;
		// PlayerSettings.Android.keyaliasPass = "xmile1@3";
	}

	private void SetiOSConfig(Game game)
	{
		//IOS
		//com.Oyo-Labs.Misterio-dos-Sonhos-1
		//com.Oyo-Labs.Misterio-dos-Sonhos2
		//com.Oyo-Labs.Misterio-dos-Sonhos-3
		string suffix = game == Game.MDS1 ? "-1" : game == Game.MDS2 ? "2" : "-3";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, string.Format("com.Oyo-Labs.Misterio-dos-Sonhos{0}", suffix));
	}
	private static void SetGame(Game game)
	{
		SetNameAndDefaultIcon(game);
		// SetLoginBG(game);
	}

	private BuildPlayerOptions SetupBuildWebGL(Game game)
	{
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.locationPathName = string.Format("Build/{1}/WebGL/{0}/", game.ToString(), PlayerSettings.bundleVersion);
		options.scenes = GetScenesOfMDS(game, false);
		options.target = BuildTarget.WebGL;
		options.targetGroup = BuildTargetGroup.WebGL;
		return options;
	}


	private static string[] GetScenesOfMDS(Game game, bool allscenes = true)
	{
		string g = game.ToString().Replace("MDS", "");
		List<string> scenes = new List<string>();
		scenes.Add("Assets/Common/Scenes/splash.unity");
		scenes.Add(string.Format("Assets/MDS{0}/Scenes/G{0}Login.unity", g));
		scenes.Add(string.Format("Assets/MDS{0}/Scenes/G{0}Room.unity", g));
		for (int w = 1; w < 5; w++)
		{
			scenes.Add(string.Format("Assets/MDS{1}/Scenes/G{1}W{0}EpisodeMap.unity", w, g));
			if (allscenes)
			{
				for (int e = 1; e < 9; e++)
				{
					scenes.Add(string.Format("Assets/MDS{2}/Scenes/World{0}/Episode{1}/G{2}W{0}E{1}.unity", w, e, g));
					for (int c = 1; c < 6; c++)
					{
						scenes.Add(string.Format("Assets/MDS{3}/Scenes/World{0}/Episode{1}/G{3}W{0}E{1}C{2}.unity", w, e, c, g));
					}
				}
			}
		}
		return scenes.ToArray();
	}

	private BuildPlayerOptions SetupBuildAndroid(Game game)
	{
		string version = PlayerSettings.bundleVersion;
		string buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.locationPathName = string.Format("Build/{3}/Android/{0}_{1}_{2}.apk", game.ToString(), version, buildNumber, PlayerSettings.bundleVersion);
		options.scenes = GetScenesOfMDS(game);
		options.target = BuildTarget.Android;
		options.targetGroup = BuildTargetGroup.Android;
		return options;
	}

	private BuildPlayerOptions SetupBuildiOS(Game game)
	{
		string version = PlayerSettings.bundleVersion;
		string buildNumber = PlayerSettings.iOS.buildNumber;
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.locationPathName = string.Format("Build/{1}/iOS/{0}/", game.ToString(), version);
		options.scenes = GetScenesOfMDS(game);
		options.target = BuildTarget.iOS;
		options.targetGroup = BuildTargetGroup.iOS;
		return options;
	}
	private BuildPlayerOptions SetupBuildStandAlone(Game game)
	{
		string version = PlayerSettings.bundleVersion;
		string buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.locationPathName = string.Format("Build/{3}/StandAlone/{0}/{0}_{1}_{2}.exe", game.ToString(), version, buildNumber, PlayerSettings.bundleVersion);
		options.scenes = GetScenesOfMDS(game);
		options.target = BuildTarget.StandaloneWindows;
		options.targetGroup = BuildTargetGroup.Standalone;
		return options;
	}

	private static void SetNameAndDefaultIcon(Game game)
	{
		// string gId = game.ToString().Replace("MDS", "");
		string gname = game switch
		{
			Game.MDS1 => "MDS 1: O Chamado dos Guardiões",
			Game.MDS2 => "MDS 2: A Máquina do Poder",
			Game.MDS3 => "MDS 3: A Grande Jornada",
			_ => throw new ArgumentOutOfRangeException(nameof(game), game, null)
		};
		PlayerSettings.companyName = "Oyo Labs";
		PlayerSettings.productName = gname;
		Texture2D mainIcon = AssetDatabase.LoadAllAssetsAtPath(string.Format("Assets/{0}/Artwork/Icons/new_icon_1024.png", game.ToString()))[0] as Texture2D;
		PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { mainIcon });
	}


	//public static void SetLoginBG(Game game)
	//{
	//    string gId = game.ToString();//.Substring(6, 1);
	//    Scene curScene = EditorSceneManager.OpenScene("Assets/_Scenes/Login.unity");
	//    GameObject.Find("CanvasBG").transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite =
	//        AssetDatabase.LoadAssetAtPath<Sprite>(string.Format("Assets/Art2D/New/{0}.png", gId));

	//    GameObject.Find("CanvasBG").transform.Find("GameName").GetComponent<UnityEngine.UI.Image>().sprite =
	//        AssetDatabase.LoadAssetAtPath<Sprite>(string.Format("Assets/Art2D/{0}_gameName.png", gId));

	//    GameObject.Find("Lbl Version").GetComponent<TMPro.TextMeshProUGUI>().SetText(
	//        string.Format("Vers�o {0} B{1}",
	//        PlayerSettings.bundleVersion,
	//        PlayerSettings.Android.bundleVersionCode));


	//    EditorSceneManager.MarkSceneDirty(curScene);
	//    EditorSceneManager.SaveScene(curScene);
	//}

	public static void SetDefineSymbols(Game game)
	{
		string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
		List<string> allDefines = definesString.Split(';').ToList();
		allDefines.Remove("MDS1");
		allDefines.Remove("MDS2");
		allDefines.Remove("MDS3");

		allDefines.Add(game.ToString().ToUpper());
		try
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				EditorUserBuildSettings.selectedBuildTargetGroup,
				string.Join(";", allDefines.ToArray()));

		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			throw;
		}
	}
}
