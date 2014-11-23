using UnityEngine;
using UnityEditor;
using System.IO;

public static class Exporter
{
	static BuildAssetBundleOptions options;
	static BuildTarget buildTarget;
	static string exportPath;
	static string resourcesPath;

	static Exporter ()
	{
		options = BuildAssetBundleOptions.CollectDependencies
			| BuildAssetBundleOptions.CompleteAssets
			| BuildAssetBundleOptions.UncompressedAssetBundle; //This makes it faster to build & load, but since it is much bigger it will take longer to download.
 
		buildTarget = EditorUserBuildSettings.activeBuildTarget;

		exportPath = PathUtils.GetAssetBundlesPath ();

		resourcesPath = PathUtils.GetResoucesPath (); 
	}

	static void ExportResourcesWithSuffix (string suffix)
	{
		string[] files = Directory.GetFiles (resourcesPath, suffix);
		if (files.Length > 0) {
			for (int i = 0; i < files.Length; i++) {
				var file = files [i];
				if (VersionManager.Update (file)) { 
					Object mainAsset = AssetDatabase.LoadMainAssetAtPath (file); 
					string pathName = exportPath + PathUtils.GetExportedName (file);
					BuildPipeline.BuildAssetBundle (mainAsset, null, pathName, options, buildTarget);
				}
			}
		}
	}
    
	[MenuItem("Assets/Export Resources")]
	static void ExportResources ()
	{  
		VersionManager.Began ();
		//export prefab
		ExportResourcesWithSuffix ("*.prefab");  
		//export txt
		ExportResourcesWithSuffix ("*.txt");

		VersionManager.Ended ();
	} 
}