using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Playmove
{
    public class PYBundleBuildWindow : EditorWindow
    {
        private static PYBundleBuildWindow _instance;
        public static PYBundleBuildWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = PYBundleBuildWindow.GetWindow<PYBundleBuildWindow>();
                return _instance;
            }
            private set { _instance = value; }
        }

        private List<string> _languagesGlobal = new List<string>();

        private List<string> _expansions = new List<string>();
        private bool[] _expansionsFoldout;

        private Vector2 _mainScroll = Vector2.zero;

        private GUIStyle _warningMessageStyle = new GUIStyle();

        [MenuItem("PlaytableAPI/PYBundle/Automatic Build"),
        MenuItem("Assets/PYBundle/Automatic Build")]
        public static void Init()
        {
            PYBundleBuildWindow window = PYBundleBuildWindow.GetWindow<PYBundleBuildWindow>();
            window.Initialize();

            Instance = window;
        }

        void OnFocus()
        {
            Initialize();
        }

        public void Initialize()
        {
            _warningMessageStyle.normal.textColor = Color.yellow * 0.8f;

            try
            {
                _expansions = new List<string>(Directory.GetDirectories(Application.dataPath +
                    "/BundlesAssets/Expansions/"));
                _expansionsFoldout = new bool[_expansions.Count];
            }
            catch
            {
                _expansions = new List<string>();
            }

            try
            {
                _languagesGlobal = new List<string>(Directory.GetDirectories(Application.dataPath +
                    string.Format(PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS, PYBundleType.Localization)));
                FilterPaths(_languagesGlobal);
            }
            catch
            {
                _languagesGlobal.Clear();
            }

            RestoreFoldoutStates();
        }

        void OnGUI()
        {
            _mainScroll = GUILayout.BeginScrollView(_mainScroll);

            GUILayout.Label("Expansions");
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical(GUI.skin.box);

            if (_expansions.Count > 0)
            {
                string[] expansionsName = GetLastNameFromPaths(_expansions);
                for (int x = 0; x < _expansions.Count; x++)
                {
                    _expansionsFoldout[x] = EditorGUILayout.Foldout(_expansionsFoldout[x], expansionsName[x]);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.BeginVertical();
                    if (_expansionsFoldout[x])
                    {
                        try
                        {
                            GUILayout.Label("Content");
                            List<string> _contents = new List<string>(Directory.GetDirectories(Application.dataPath +
                                string.Format(PYBundleFolderScanner.EXPANSION_ASSETS_FOLDERS, expansionsName[x], PYBundleType.Content)));

                            if (_contents.Count > 0)
                            {
                                FilterPaths(_contents);
                                string[] _contentsName = GetLastNameFromPaths(_contents);
                                for (int y = 0; y < _contentsName.Length; y++)
                                {
                                    // Apply green color with already exist a bundle created
                                    GUI.color = Directory.Exists(_contents[y].Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                                    if (GUILayout.Button("Build " + _contentsName[y]))
                                    {
                                        BuildBundle(_contents[y], _contents[y]);
                                    }
                                }
                                GUI.color = Color.white;
                            }
                            else
                            {
                                WarningMessage("Empty!");
                            }
                        }
                        catch
                        {
                            WarningMessage("Not found!");
                        }

                        try
                        {
                            GUILayout.Label("Data");

                            string savePath = _expansions[x] + "/Data";
                            if (!Directory.Exists(savePath))
                                throw new DirectoryNotFoundException();

                            if (Directory.GetFiles(savePath, "*.*", SearchOption.AllDirectories).Length > 0)
                            {
                                // Apply green color with already exist a bundle created
                                GUI.color = Directory.Exists(savePath.Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                                if (GUILayout.Button("Build Data"))
                                {
                                    BuildBundle(savePath, savePath);
                                }
                            }
                            else
                            {
                                WarningMessage("Empty!");
                            }
                            GUI.color = Color.white;
                        }
                        catch
                        {
                            WarningMessage("Not found!");
                        }

                        try
                        {
                            GUILayout.Label("Localization");
                            List<string> _languagesLocal = new List<string>(Directory.GetDirectories(Application.dataPath +
                                string.Format(PYBundleFolderScanner.EXPANSION_ASSETS_FOLDERS, expansionsName[x], PYBundleType.Localization)));

                            if (_languagesLocal.Count > 0)
                            {
                                FilterPaths(_languagesLocal);
                                string[] _languagesLocalName = GetLastNameFromPaths(_languagesLocal);
                                for (int y = 0; y < _languagesLocalName.Length; y++)
                                {
                                    // Apply green color with already exist a bundle created
                                    GUI.color = Directory.Exists(_languagesLocal[y].Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                                    if (GUILayout.Button("Build " + _languagesLocalName[y]))
                                    {
                                        BuildBundle(_languagesLocal[y], _languagesLocal[y]);
                                    }
                                }
                                GUI.color = Color.white;
                            }
                            else
                            {
                                WarningMessage("Empty!");
                            }
                        }
                        catch
                        {
                            WarningMessage("Not found!");
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                WarningMessage("Empty!");
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Global
            GUILayout.Space(15);
            GUILayout.Label("Global");
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical(GUI.skin.box);

            try
            {
                GUILayout.Label("Content");
                List<string> _contents = new List<string>(Directory.GetDirectories(Application.dataPath +
                    string.Format(PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS, PYBundleType.Content)));

                if (_contents.Count > 0)
                {
                    FilterPaths(_contents);
                    string[] _contentsName = GetLastNameFromPaths(_contents);

                    for (int y = 0; y < _contentsName.Length; y++)
                    {
                        // Apply green color with already exist a bundle created
                        GUI.color = Directory.Exists(_contents[y].Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                        GUILayout.Space(2);
                        if (GUILayout.Button("Build " + _contentsName[y], GUILayout.Width(300)))
                        {
                            BuildBundle(_contents[y], _contents[y]);
                        }
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    WarningMessage("Empty!");
                }
            }
            catch
            {
                WarningMessage("Not found!");
            }

            try
            {
                GUILayout.Label("Data");

                string dataGlobalPath = Application.dataPath + string.Format(PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS, PYBundleType.Data);
                if (!Directory.Exists(dataGlobalPath))
                    throw new DirectoryNotFoundException();

                if (Directory.GetFiles(dataGlobalPath, "*.*", SearchOption.AllDirectories).Length > 0)
                {
                    // Apply green color with already exist a bundle created
                    GUI.color = Directory.Exists(dataGlobalPath.Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                    if (GUILayout.Button("Build Data"))
                    {
                        BuildBundle(dataGlobalPath, dataGlobalPath);
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    WarningMessage("Empty!");
                }
            }
            catch
            {
                WarningMessage("Not found!");
            }

            GUILayout.Space(15);
            try
            {
                GUILayout.Label("Localization");

                if (!Directory.Exists(Application.dataPath +
                    string.Format(PYBundleFolderScanner.GLOBAL_ASSETS_FOLDERS, PYBundleType.Localization)))
                    throw new DirectoryNotFoundException();

                if (_languagesGlobal.Count > 0)
                {
                    string[] _languagesGlobalTemp = GetLastNameFromPaths(_languagesGlobal);
                    for (int x = 0; x < _languagesGlobalTemp.Length; x++)
                    {
                        // Apply green color with already exist a bundle created
                        GUI.color = Directory.Exists(_languagesGlobal[x].Replace("BundlesAssets", "Bundles")) ? Color.green : Color.white;

                        if (GUILayout.Button("Build " + _languagesGlobalTemp[x]))
                        {
                            BuildBundle(_languagesGlobal[x], _languagesGlobal[x]);
                        }
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    WarningMessage("Empty!");
                }
            }
            catch
            {
                WarningMessage("Not found!");
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            SaveFoldoutsStateForExpansions();
        }

        private void RestoreFoldoutStates()
        {
            if (_expansions.Count == 0)
                return;

            string[] _expansionsName = GetLastNameFromPaths(_expansions);
            for (int x = 0; x < _expansionsFoldout.Length; x++)
            {
                _expansionsFoldout[x] = EditorPrefs.GetBool("EEFS" + _expansionsName[x] + x, false);
            }
        }
        private void SaveFoldoutsStateForExpansions()
        {
            if (_expansions.Count == 0)
                return;

            // EEFS = Editor Expansion Foldout State
            string[] _expansionsName = GetLastNameFromPaths(_expansions);
            for (int x = 0; x < _expansionsFoldout.Length; x++)
            {
                EditorPrefs.SetBool("EEFS" + _expansionsName[x] + x, _expansionsFoldout[x]);
            }
        }

        private void WarningMessage(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label(message, _warningMessageStyle);
            GUILayout.EndHorizontal();
        }

        public static string[] GetLastNameFromPaths(List<string> paths)
        {
            string[] temp = new string[paths.Count];
            for (int x = 0; x < paths.Count; x++)
            {
                string[] split = paths[x].Split('/');
                temp[x] = split[split.Length - 1];
            }
            return temp;
        }

        private void FilterPaths(List<string> paths)
        {
            for (int x = 0; x < paths.Count; x++)
                paths[x] = paths[x].Replace('\\', '/');
        }

        private void BuildBundle(string srcPath, string savePath)
        {
            ExportAssetBundles.ExportResource(srcPath, savePath);
            this.Focus();
        }
    }
}