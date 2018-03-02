using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playmove
{
    public class LocalizationFilesSync
    {
        private static LocalizationFilesSync _instance;

        public static LocalizationFilesSync Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocalizationFilesSync();
                return _instance;
            }
        }

        // 0: BundleType: Always PYBundleType.Localization
        // 1: Language
        public static readonly string PATH_TO_GLOBAL_LOCALIZATION = Application.dataPath + PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS + "{1}/Texts/";

        // 0: Expansion name
        // 1: BundleType: Always PYBundleType.Localization
        // 2: Language
        public static readonly string PATH_TO_EXPANSION_LOCALIZATION = Application.dataPath + PYBundleFolderScanner.EXPANSION_ASSETS_FOLDERS + "{2}/Texts/";

        #region Data Classes
        public class DownloadFileData
        {
            // This is the application name to query the webservice
            // this is necessary because with have genericTags and appTags
            // Generic tags should be queried as "" (empty string) while
            // the appTags should be queried as "appName"
            // QueryFileTag = ExecutableName
            public string QueryFileTag;

            public string FileName;
            public string FileDescription;
            public bool IsFileUpdated = true;

            public DownloadFileData(string queryFileTag, string name, string description)
            {
                QueryFileTag = queryFileTag;
                FileName = name;
                FileDescription = description;
            }
        }

        public class GameData
        {
            public string Name;
            public string ExecutableName;

            public Dictionary<string, DownloadFileData> Files = new Dictionary<string, DownloadFileData>();

            public GameData(string name)
            {
                Name = name;
            }
        }

        public class AppLocalizationData
        {
            public string Localizacao { get; set; }
        }
        #endregion

        // Constructor
        private LocalizationFilesSync()
        {
            ScanProjectFolders();
        }

        #region Private Members
        private List<GameData> _gamesData = new List<GameData>();
        private List<string> _bundlesPathNeedUpdate = new List<string>();
        private List<string> tempSyncFilesKey = new List<string>();
        #endregion

        /// <summary>
        /// Sync localization files from the webservice and return 
        /// a list of bundles path that need to be rebuilded
        /// </summary>
        /// <param name="callback">Return of bundles path that need to be Rebuilded</param>
        public void Sync(Action<List<string>> callback)
        {
            _bundlesPathNeedUpdate.Clear();

            ScanProjectFolders();
            RequestLanguages(0, _gamesData, () =>
            {
                tempSyncFilesKey.Clear();
                SyncFiles(0, () =>
                {
                    if (callback != null)
                        callback(_bundlesPathNeedUpdate);
                });
            });
        }

        private void SyncFiles(int gameDataIndex, Action callbackCompleted)
        {
            string fileLocalization = "";
            foreach (string localizationName in _gamesData[gameDataIndex].Files.Keys)
            {
                if (!tempSyncFilesKey.Contains(localizationName))
                {
                    fileLocalization = localizationName;
                    tempSyncFilesKey.Add(localizationName);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(fileLocalization))
            {
                DownloadFileData fileData = _gamesData[gameDataIndex].Files[fileLocalization];

                string pathStoredAsset = "";
                if (gameDataIndex == 0)
                    pathStoredAsset = string.Format(PATH_TO_GLOBAL_LOCALIZATION, PYBundleType.Localization, fileLocalization) + fileData.FileName + ".json";
                else
                    pathStoredAsset = string.Format(PATH_TO_EXPANSION_LOCALIZATION, _gamesData[gameDataIndex].Name, PYBundleType.Localization, fileLocalization) + fileData.FileName + ".json";

                RequestLocalizationFile(_gamesData[gameDataIndex].ExecutableName, fileLocalization,
                    (json) =>
                    {
                        List<LocalizationData> jsonObject = new List<LocalizationData>(new List<LocalizationData>());

                        // Parse json to object to exclude some unecessary fields
                        if (!string.IsNullOrEmpty(json))
                            jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LocalizationData>>(json);

                        string oldStringFile = null;
                        if (File.Exists(pathStoredAsset))
                            oldStringFile = File.ReadAllText(pathStoredAsset);

                        string newStringFile = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(pathStoredAsset, newStringFile);

                        bool needToUpdateBundle = oldStringFile.Length != newStringFile.Length;
                        if (!needToUpdateBundle)
                        {
                            for (int i = 0; i < newStringFile.Length; i++)
                            {
                                if (oldStringFile[i] != newStringFile[i])
                                {
                                    needToUpdateBundle = true;
                                    break;
                                }
                            }
                        }

                        if (needToUpdateBundle)
                            _bundlesPathNeedUpdate.Add(pathStoredAsset.Replace(fileData.FileName + ".json", "").Replace("Texts/", ""));

                        SyncFiles(gameDataIndex, callbackCompleted);
                    });
            }
            else if (gameDataIndex >= _gamesData.Count - 1)
            {
                // When the entire request has completed refresh the unity assets
                AssetDatabase.Refresh();

                if (callbackCompleted != null)
                    callbackCompleted();
            }
            else
            {
                tempSyncFilesKey.Clear();
                gameDataIndex++;
                SyncFiles(gameDataIndex, callbackCompleted);
            }
        }

        private void ScanProjectFolders()
        {
            _gamesData.Clear();
            _gamesData.Add(new GameData(PlayerSettings.productName));

            string[] dirspath = new string[0];
            try
            {
                dirspath = Directory.GetDirectories(Application.dataPath + "/BundlesAssets/Expansions/");
            }
            catch { }

            foreach (string dirPath in dirspath)
            {
                DirectoryInfo info = new DirectoryInfo(dirPath);
                _gamesData.Add(new GameData(info.Name));
                _gamesData[_gamesData.Count - 1].ExecutableName = info.Name;
            }

            // Restore executable names
            foreach (GameData data in _gamesData)
            {
                data.ExecutableName = data.Name;
                if (string.IsNullOrEmpty(data.ExecutableName))
                    data.ExecutableName = data.Name;
            }

            // Find localization files already in project
            for (int i = 0; i < _gamesData.Count; i++)
            {
                string[] localizationFoldersPath = new string[0];
                // The global folder
                if (i == 0)
                {
                    try
                    {
                        localizationFoldersPath = Directory.GetDirectories(Application.dataPath +
                            string.Format(PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS, PYBundleType.Localization));
                    }
                    catch { }
                }
                // The expansions folder
                else
                {
                    try
                    {
                        localizationFoldersPath = Directory.GetDirectories(Application.dataPath +
                            string.Format(PYBundleFolderScanner.EXPANSION_ASSETS_FOLDERS, _gamesData[i].Name, PYBundleType.Localization));
                    }
                    catch { }
                }

                foreach (string localizationPath in localizationFoldersPath)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(localizationPath);

                    // If we found files in project
                    DownloadFileData fileData = new DownloadFileData(_gamesData[i].ExecutableName, "string0", "");
                    if (!_gamesData[i].Files.ContainsKey(dirInfo.Name))
                        _gamesData[i].Files.Add(dirInfo.Name, fileData);
                }
            }
        }

        private void RequestLanguages(int index, List<GameData> gamesData, Action callbackCompleted)
        {
            RequestLocalizationForApp(gamesData[index].ExecutableName, (json) =>
            {
                if (string.IsNullOrEmpty(json))
                {
                    if (callbackCompleted != null)
                        callbackCompleted();
                    return;
                }

                List<AppLocalizationData> jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AppLocalizationData>>(json);

                // If server dont returned anything we just try another
                if (jsonObject == null || jsonObject == null)
                {
                    index++;
                    if (index < gamesData.Count)
                        RequestLanguages(index, gamesData, callbackCompleted);
                    else if (callbackCompleted != null)
                        callbackCompleted();
                    return;
                }

                // Serve found localization
                foreach (AppLocalizationData locData in jsonObject)
                {
                    if (!gamesData[index].Files.ContainsKey(locData.Localizacao))
                    {
                        gamesData[index].Files.Add(locData.Localizacao,
                            new DownloadFileData(gamesData[index].ExecutableName, "string0", ""));
                    }
                }

                index++;
                if (index < gamesData.Count)
                    RequestLanguages(index, gamesData, callbackCompleted);
                else if (callbackCompleted != null)
                    callbackCompleted();
            });
        }

        #region Webservice Requests
        private void RequestLocalizationForApp(string executableName, Action<string> callback)
        {
            EditorCoroutine.Start(WebRequest(string.Format(@"https://api.playmove.com.br/api/v2/LocalizacaoApps/ListarLocalizacaoApp?" +
                "executavel={0}.exe", executableName), callback));
        }

        private void RequestLocalizationFile(string executableName, string localization, Action<string> callback)
        {
            EditorCoroutine.Start(WebRequest(string.Format(@"https://api.playmove.com.br/api/v2/LocalizacaoApps/ListarTodos?" +
                "Filtro.Executavel={0}.exe&Filtro.Localizacao={1}", executableName, localization), callback));
        }

        private IEnumerator WebRequest(string url, Action<string> callback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization-Id", "gerenciarapps@playmove.com.br");
            headers.Add("Authorization-Token", "gerencia@play123");
            headers.Add("Authorization-Role", "30");
            url = url.Replace("\r", "");
            WWW www = new WWW(url, null, headers);
            while (!www.isDone)
                yield return null;

            if (callback != null)
                callback(www.text);
        }
        #endregion
    }
}