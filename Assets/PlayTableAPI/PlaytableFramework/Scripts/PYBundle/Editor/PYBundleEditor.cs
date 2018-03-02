using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;

namespace Playmove
{
    public class PYBundleEditor : Editor
    {
        [MenuItem("PlaytableAPI/PYBundle/Create bundle folder structure"),
        MenuItem("Assets/PYBundle/Create bundle folder structure")]
        public static void CreateBundleFolderStructure()
        {
            // Bundles buildFolder
            CreateBundleFolders("Bundles");

            // Bundles assetFolders
            CreateBundleFolders("BundlesAssets");
        }

        [MenuItem("Assets/PYBundle/CreateStringsXML")]
        public static void CreateStringsXML()
        {
            string xmlPath = GetSelectedPathOrFallback() + "/strings.xml";

            StreamWriter writer = File.CreateText(xmlPath);
            writer.Write("<Strings xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></Strings>");
            writer.Close();

            AssetDatabase.Refresh();

            Debug.Log("string.xml successfully created at " + xmlPath);
        }

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            DirectoryInfo dirBuild = Directory.GetParent(pathToBuiltProject);

            if (Directory.Exists(dirBuild.FullName + "/Bundles"))
                Directory.Delete(dirBuild.FullName + "/Bundles", true);

            string sourceDir = Application.dataPath + "/Bundles";
            if (!Directory.Exists(sourceDir))
                return;

            string targetDir = dirBuild.FullName + "/Bundles";
            DirectoryCopy(sourceDir, targetDir, true);

            Debug.Log(string.Format("Folder {0} copied to build path at {1}.", sourceDir, targetDir));
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

        private static void CreateBundleFolders(string rootBundleName)
        {
            if (Directory.Exists(Application.dataPath + "/" + rootBundleName))
            {
                Debug.LogWarning(string.Format("Folder {0} already exist!", rootBundleName));
                return;
            }

            AssetDatabase.CreateFolder("Assets", rootBundleName);
            AssetDatabase.CreateFolder("Assets/" + rootBundleName, "Expansions");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Expansions", "Default");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Expansions/Default", "Data");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Expansions/Default", "Localization");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Expansions/Default", "Content");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName, "Global");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Global", "Content");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Global", "Data");
            AssetDatabase.CreateFolder("Assets/" + rootBundleName + "/Global", "Localization");

            Debug.Log(string.Format("Folder {0} created successfully.", rootBundleName));
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles().Where<FileInfo>(name => !name.Name.EndsWith(".meta")).ToArray();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}