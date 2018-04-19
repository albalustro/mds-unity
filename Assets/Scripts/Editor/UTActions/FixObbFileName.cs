using System.Collections;
using System.IO;
using AncientLightStudios.uTomate.API;
using UnityEditor;
using UnityEngine;

public class FixObbFileName : UTAction
{
    public string outputPath;

    public override IEnumerator Execute(UTContext context)
    {
        string bundleName = PlayerSettings.applicationIdentifier.Replace("com.xmile.", "");
        string bundleVersion = PlayerSettings.Android.bundleVersionCode.ToString();

        string curObbFileName = string.Format("{0}.main.obb", bundleName);
        string newObbFileName = string.Format("main.{0}.{1}.obb", bundleVersion, bundleName);



        string curObbFilePath = Path.Combine(outputPath, curObbFileName);
        string newObbFilePath = Path.Combine(outputPath, newObbFileName);

        if(File.Exists(curObbFilePath))
        {
            File.Move(curObbFilePath, newObbFilePath);
            Debug.Log("Obb renomeado para " + newObbFilePath);
        }
        yield return null;
    }

    [MenuItem("Assets/Create/uTomate/MDS/Fix Obb File Name", false, 2000)]
    public static void AddAction()
    {
        Create<FixObbFileName>();
    }
}
