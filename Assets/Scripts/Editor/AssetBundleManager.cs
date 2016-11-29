using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class AssetBundleManager : Editor {

	[MenuItem("MDS/Assetbundles/Create")]
	public static void CreateAssetBundles()
	{

		string outputPath = "Build/WebGL/AssetBundles";

		if(!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);

		var options = BuildAssetBundleOptions.None;

		BuildPipeline.BuildAssetBundles(outputPath, options, BuildTarget.WebGL);
	}
}
