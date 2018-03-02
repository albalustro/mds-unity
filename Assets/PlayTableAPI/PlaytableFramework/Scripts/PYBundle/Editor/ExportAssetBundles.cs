using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

using Object = UnityEngine.Object;

namespace Playmove
{
    public static class ExportAssetBundles
    {
        public static bool ExportResource(string srcPath, string savePath)
        {
            string[] filesAtPath = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories);
            filesAtPath = filesAtPath.Where(p => !p.EndsWith(".meta")).ToArray();

            // Remove last character with its a slash char
            if (savePath.EndsWith("/") || savePath.EndsWith("\\"))
                savePath = savePath.Remove(savePath.Length - 1, 1);

            if (srcPath.EndsWith("/") || srcPath.EndsWith("\\"))
                srcPath = srcPath.Remove(srcPath.Length - 1, 1);

            ExportAssetBundles.CreateVersionFile(srcPath);

            Object[] objs = new Object[filesAtPath.Length];
            for (int x = 0; x < objs.Length; x++)
            {
                objs[x] = AssetDatabase.LoadAssetAtPath(filesAtPath[x].Replace(Application.dataPath, "Assets"), typeof(Object));
            }

            string[] split = srcPath.Split('/');
            savePath = savePath.Replace("BundlesAssets", "Bundles");

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            return ExportAssetBundles.ExportResource(objs, savePath, split[split.Length - 1]);
        }

        public static bool ExportResource(Object[] selection, string savePath, string bundleName)
        {
            // Bring up save panel
            string path = EditorUtility.SaveFilePanel("Save Resource", savePath, bundleName, "unity3d");
            if (path.Length != 0)
            {
                string[] tempSplit = path.Split('/');
                bundleName = tempSplit[tempSplit.Length - 1].Split('.')[0];

                AssetBundleBuild bundle = new AssetBundleBuild();
                bundle.assetBundleName = bundleName + ".unity3d";
                string[] assetsName = new string[selection.Length];
                for (int x = 0; x < assetsName.Length; x++)
                {
                    assetsName[x] = AssetDatabase.GetAssetPath(selection[x]);
                }
                bundle.assetNames = assetsName;

                BuildPipeline.BuildAssetBundles(savePath, new AssetBundleBuild[1] { bundle }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
                AssetDatabase.Refresh();

                return true;
            }

            return false;
        }

        [MenuItem("PlaytableAPI/PYBundle/Build/Build selected folder"),
        MenuItem("Assets/PYBundle/Build/Build selected folder")]
        static void ExportResourceFromUnityMenus()
        {
            // Build the resource file from the active selection.
            DirectoryInfo selectedDir = new DirectoryInfo(GetSelectedPathOrFallback());

            // Create version file
            CreateVersionFile(selectedDir.FullName);

            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            ExportResource(selection, selectedDir.FullName.Replace("BundlesAssets", "Bundles"), selectedDir.Name);

            Selection.objects = selection;
        }

        [MenuItem("PlaytableAPI/PYBundle/Build/Build unity marked bundles"),
        MenuItem("Assets/PYBundle/Build/Build unity marked bundles")]
        static void ExportResourceUnityMarkedBundles()
        {
            string path = EditorUtility.SaveFilePanel("Save Resource", Application.dataPath, "bundle", "unity3d");
            if (path.Length != 0)
            {
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
            }
        }

        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        public static void CreateVersionFile(string path)
        {
            if (path[path.Length - 1] == '/')
                path = path.Remove(path.Length - 1, 1);

            string filePath = path + "/version.xml";

            PYBundleVersion version = new PYBundleVersion();
            if (File.Exists(filePath))
                version = PYXML.Deserializer<PYBundleVersion>(filePath);

            // Update informations
            version.Version = BuildModeEditor.GetBundleVersion();
            version.CreationDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            PYXML.Serializer(filePath, version);

            AssetDatabase.Refresh();
        }

#if !UNITY_5
    [MenuItem("PlaytableAPI/PYBundle/Build/From Selection/Track dependencies"),
    MenuItem("Assets/PYBundle/Build/From Selection/Track dependencies")]
    static void ExportResourceFromUnityMenus()
    {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                              BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
            Selection.objects = selection;
        }
    }

    [MenuItem("PlaytableAPI/PYBundle/Build/From Selection/No dependency tracking"),
    MenuItem("Assets/PYBundle/Build/From Selection/No dependency tracking")]
    static void ExportResourceNoTrack()
    {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path);
        }
    }
    [MenuItem("PlaytableAPI/PYBundle/Build/From Selection/Uncompressed Tracking dependencies"),
    MenuItem("Assets/PYBundle/Build/From Selection/Uncompressed Tracking dependencies")]
    static void ExportUncompressedResource()
    {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                              BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle);
            Selection.objects = selection;
        }
    }
    [MenuItem("PlaytableAPI/PYBundle/Build/From Selection/Uncompressed With No dependency tracking"),
    MenuItem("Assets/PYBundle/Build/From Selection/Uncompressed With No dependency tracking")]
    static void ExportUncompressedResourceNoTrack()
    {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.UncompressedAssetBundle);
        }
    }
}

namespace Playmove { public class SceneSelectionWindow : EditorWindow
{
    public string expansionName = "Default";
    public string bundleName;
    //public static string[] scenes;
    public static System.Collections.Generic.List<string> scenes = new System.Collections.Generic.List<string>(1);
    public int maxIndex = 1;

    [MenuItem("PlaytableAPI/PYBundle/Build/Uncompressed Scenes"),
    MenuItem("Assets/PYBundle/Build/Uncompressed Scenes")]
    static void Init()
    {
        EditorWindow.GetWindow<SceneSelectionWindow>();
    }

    void OnGUI()
    {
        expansionName = EditorGUILayout.TextField("Expansion name", expansionName);
        bundleName = EditorGUILayout.TextField("Bundle name", bundleName);

        if (GUILayout.Button("Add"))
        {
            scenes.Add(EditorUtility.OpenFilePanel("Select scene", "", "unity"));
        }
        for (int i = 0; i < scenes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            scenes[i] = EditorGUILayout.TextField(scenes[i].Split('/')[scenes[i].Split('/').Length - 1], scenes[i]);

            if (GUILayout.Button("Change"))
            {
                scenes[i] = EditorUtility.OpenFilePanel("Select scene", "", "unity");
            }
            if (GUILayout.Button("Remove"))
            {
                scenes.Remove(scenes[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Build AssetBundle"))
        {
            string path = EditorUtility.SaveFilePanel("Save Bundle", Application.dataPath + "/../Expansions", expansionName + "_" + bundleName, "unity3d");
            if (path.Length != 0)
                BuildPipeline.BuildStreamedSceneAssetBundle(scenes.ToArray(), path, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.UncompressedAssetBundle);
        }
    }
}

namespace Playmove { public class ObjectSelectionWindow : EditorWindow
{
    public string expansionName = "Default";
    public string bundleName;
    public static Object[] objects;
    public static string objectsPath;
    public bool track = false;

    [MenuItem("PlaytableAPI/PYBundle/Build/Uncompressed Objects"),
    MenuItem("Assets/PYBundle/Build/Uncompressed Objects")]
    static void Init()
    {
        EditorWindow.GetWindow<ObjectSelectionWindow>();
    }

    void OnGUI()
    {
        expansionName = EditorGUILayout.TextField("Expansion name", expansionName);
        bundleName = EditorGUILayout.TextField("Bundle name", bundleName);

        track = EditorGUILayout.Toggle("Track Dependencies", track);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Path", objectsPath);

        if (GUILayout.Button("Select Folder"))
        {
            objectsPath = EditorUtility.OpenFolderPanel("Objects Path", "", "");       
        }        
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Build AssetBundle"))
        {
            objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(objectsPath);

            if (objects != null)
            {
                string path = EditorUtility.SaveFilePanel("Save Bundle", Application.dataPath + "/../Expansions", expansionName + "_" + bundleName, "unity3d");
                if (track)
                {
                    BuildPipeline.BuildAssetBundle(objects[0], objects, path,
                                  BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle);
                }
                else
                {
                    BuildPipeline.BuildAssetBundle(objects[0], objects, path, BuildAssetBundleOptions.UncompressedAssetBundle);
                }
            }
        }
    }
#endif
    }
}