using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Playmove
{
    [RequireComponent(typeof(PYDebug))]
    public class PlaytableWin32 : MonoBehaviour
    {
        #region Instance

        private static PlaytableWin32 _instance;

        public static PlaytableWin32 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<PlaytableWin32>();
                return _instance;
            }
        }

        #endregion Instance

        public const int SETTING_INDEX_RESOLUTION = 0;
        public const int SETTING_INDEX_FULLSCREEN = 1;
        public const int SETTING_INDEX_FULLSCREEN_MODE = 2;
        public const int SETTING_INDEX_BUILD_MODE = 3;
        public const int SETTING_INDEX_GAME_NAME = 4;
        public const int SETTING_INDEX_TARGET_FRAMERATE = 5;
        public const int SETTING_INDEX_GAME_VERSION = 6;
        public const int SETTING_INDEX_BUNDLE_VERSION = 7;
        public const int SETTING_INDEX_GAME_GUID = 8;

        public static string GameName;
        public static GameVersion GameVersion;
        public static GameVersion BundleVersion;

        #region events and delegates

        [Serializable]
        public class PlaytableEvents : UnityEvent { }

        /// <summary>
        /// Lançado após o jogo ser autenticado e identificado no banco.
        /// </summary>
        [Header("Lançado depois que o jogo autenticar no SOP e validar em banco.")]
        public PlaytableEvents onGameValidate = new PlaytableEvents();

        #endregion events and delegates

        //public delegate void DelegateAfterGetVolume(int volume);

        private PYGameServiceManager.Game Game;
        public long GameId { get { return Game != null ? Game.Id : 0; } }

        private PlaytableDataManager _data;

        public PlaytableDataManager Data
        {
            get
            {
                if (!_data)
                {
                    _data = GetComponent<PlaytableDataManager>();
                    if (!_data)
                        _data = gameObject.AddComponent<PlaytableDataManager>();
                }
                return _data;
            }
        }

        [SerializeField, HideInInspector] // Editor use serialize to hold data but dont show on inspector
        private string _language = "pt-BR";

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        [SerializeField, HideInInspector] // Editor use serialize to hold data but dont show on inspector
        private string _expansionName = "Default";

        public string ExpansionName
        {
            get { return _expansionName; }
            set { _expansionName = value; }
        }

        private Authenticate _authentication;

        public Authenticate Authentication
        {
            get
            {
                if (_authentication == null)
                    _authentication = new Authenticate();
                return _authentication;
            }
        }

        public string ValidationKey { get; private set; }
        public string GameGuid { get; private set; }

        private bool _bundleLoaded, _gameValidaded;

        private void Awake()
        {
            #region Instance

            if (Instance != null && Instance != this)
            {
                if (gameObject.transform.parent != null)
                    Destroy(gameObject.transform.parent.gameObject);
                else
                    Destroy(gameObject);
                return;
            }

            if (gameObject.transform.parent != null)
                DontDestroyOnLoad(gameObject.transform.parent.gameObject);
            else
                DontDestroyOnLoad(gameObject);

            #endregion Instance

            try
            {
                if (Authentication.IsValid())
                    Debug.Log("Dll Authenticated");
                else
                    Debug.LogWarning("Dll Not Authenticated (Use only on SopVirtual)");
            }
            catch (Exception e)
            {
                Debug.LogError("Playtable Launcher: Dll missing and \n" + e.Message);
                ForceExit(888);
                throw;
            }
        }

        private void Start()
        {
            TextAsset gameSettings = Resources.Load<TextAsset>("gameSettings");
            string[] settings = gameSettings.text.Split('\n');

            Application.targetFrameRate = int.Parse(settings[SETTING_INDEX_TARGET_FRAMERATE].Replace("Targer Frame Rate: ", "") ?? "60"); //TargetFrameRate;

            GameName = settings[SETTING_INDEX_GAME_NAME] == null ? "DefaultGame" : settings[SETTING_INDEX_GAME_NAME];

            if (gameSettings == null)
                return;

            GameVersion = new GameVersion(settings[SETTING_INDEX_GAME_VERSION].Replace("Game Version: ", ""));
            BundleVersion = new GameVersion(settings[SETTING_INDEX_BUNDLE_VERSION].Replace("Bundle Version: ", ""));

            string res = settings[SETTING_INDEX_RESOLUTION].Replace("Resolution: ", "");
            string[] resolution = res.Split('x');

            string fs = settings[SETTING_INDEX_FULLSCREEN].Replace("Fullscreen: ", "");
            bool fullscreen = bool.Parse(fs.Trim());

            Screen.SetResolution(int.Parse(resolution[0].Trim()), int.Parse(resolution[1].Trim()), fullscreen);
#if TEST
            Cursor.visible = true;
#else
            Cursor.visible = false;
#endif

#if UNITY_EDITOR
            Cursor.visible = true;
#endif

            GameGuid = settings[SETTING_INDEX_GAME_GUID].Split(':')[1].Trim();// .Replace("Game GUID: ", "");

            ReadArguments();

#if RELEASE && !UNITY_EDITOR
            ValidateArgKey();
#endif

            if (!string.IsNullOrEmpty(GameGuid))
            {
                new PYGameServiceManager().Get(GameGuid, GetGameCallback);
            }

            Data.Initialize();

            PYBundleManager.Instance.Load();
            PYBundleManager.Instance.onLoadCompleted.AddListener((data) =>
            {
                _bundleLoaded = true;
                IsReady();
            });
        }

        private void ValidateArgKey()
        {
            if (string.IsNullOrEmpty(ValidationKey))
            {
                ForceExit(900);
            }
            else
            {
                string[] splitArq = ValidationKey.Split('#');
                byte[] byt = System.Text.Encoding.UTF8.GetBytes(GameGuid);
                if (splitArq[0] != Convert.ToBase64String(byt))
                    ForceExit(800);
            }
        }

        private void GetGameCallback(PYGameServiceManager.Game gameInfo)
        {
            Game = gameInfo;
#if TEST || UNITY_EDITOR
            _gameValidaded = true;
            IsReady();
#else
            DoLauncherAuthentication(GameName);
#endif
        }

        private void ReadArguments()
        {
            bool hasFoundLanguage = false;
            string[] paramsExe = System.Environment.GetCommandLineArgs();

            // This if editor, is just for us to be able to test different languages in Editor
            // without building the game
#if UNITY_EDITOR
            paramsExe = new string[1] { "lang:" + Language };
#endif

            ValidationKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(GameGuid)) + "#123";

            for (int i = 0; i < paramsExe.Length; i++)
            {
                if (paramsExe[i].StartsWith("lang:"))
                {
                    hasFoundLanguage = LoadLanguage(paramsExe[i]);
                }
                else if (paramsExe[i].StartsWith("expan:"))
                {
                    LoadExpansionName(paramsExe[i]);
                }
#if RELEASE && !UNITY_EDITOR
                else if (paramsExe[i].StartsWith("key:"))
                {
                    ValidationKey = paramsExe[i].Replace("key:", "");
                }
#endif
            }

            if (!hasFoundLanguage)
                Language = "pt-BR";
        }

        /// <summary>
        /// Check if exists some bundle for the language passed in exe param
        /// </summary>
        /// <param name="argm"></param>
        /// <returns></returns>
        private bool LoadLanguage(string argm)
        {
            bool hasFoundLanguage = false;
            string language = argm.Replace("lang:", "");
            foreach (string pathLocalization in PYBundleFolderScanner.GetGlobalLocalizationBundlesPath(language))
            {
                if (pathLocalization.Contains(".unity3d"))
                {
                    Language = language;
                    hasFoundLanguage = true;
                }
            }

            return hasFoundLanguage;
        }

        /// <summary>
        /// Check if exists some bundle for the expansion passed in exe param
        /// </summary>
        /// <param name="argm"></param>
        private void LoadExpansionName(string argm)
        {
            string expansionName = argm.Replace("expan:", "");
            foreach (string pathLocalization in PYBundleFolderScanner.GetExpansionBundlesPath(expansionName, PYBundleType.Localization))
            {
                if (pathLocalization.Contains(".unity3d"))
                    ExpansionName = expansionName;
            }
        }

        private void DoLauncherAuthentication(string gameTicket)
        {
            string url = string.Format("{0}TicketProvider/Authenticate?gameTicket={1}",
                SopService.Configuration.REQUEST_FULLPATH,
                gameTicket);
            StartCoroutine(StartAuthentication(url, gameTicket));
        }

        private IEnumerator StartAuthentication(string url, string gameTicket)
        {
            WWW www = new WWW(url);
            yield return www;

            if (Authentication.IsValid())
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    var key = www.text;
                    int step = Authentication.SopAuthentication(key);
                    Debug.LogError(step);
                    if (step != 0)
                        ForceExit(step);
                    else
                    {
                        Debug.LogError("Playtable Launcher: Awesome! Game authenticated.");
                        _gameValidaded = true;
                        IsReady();
                    }
                }
                else
                {
                    Debug.LogError("Playtable Launcher: " + www.error);
                    ForceExit(999);
                }
            }
            else
            {
                Debug.LogError("Playtable Launcher " + "Autenticação falsa em execução");
            }
        }

        private void ForceExit(int step)
        {
            ExitGame();
            throw new UnityException(string.Format("Bad: Invalid Playtable Launcher. Step: {0}", step));
        }

        public void ExitGame()
        {
            if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void IsReady()
        {
            if (_bundleLoaded && _gameValidaded)
            {
                onGameValidate.Invoke();
            }
        }
    }
}