using System;
using System.Collections;
using AncientLightStudios.uTomate.API;
using UnityEditor;

public class SetBundleVersion : UTAction
{
    public bool setBuildDateTimeAsBundleVersionCode = true;
    public int bundleVersionCode = 200;

    public override IEnumerator Execute(UTContext context)
    {
        string version = bundleVersionCode.ToString();

        if(setBuildDateTimeAsBundleVersionCode)
        {
            DateTime now = DateTime.Now;
            version = string.Format("{0}{1:00}{2:00}{3:00}{4:00}", now.Year - 2000, now.Month, now.Day, now.Hour, now.Minute);
        }

        PlayerSettings.Android.bundleVersionCode = int.Parse(version);
        yield return null;
    }

    [MenuItem("Assets/Create/uTomate/MDS/Set Bundle Version", false, 2000)]
    public static void AddAction()
    {
        Create<SetBundleVersion>();
    }
}