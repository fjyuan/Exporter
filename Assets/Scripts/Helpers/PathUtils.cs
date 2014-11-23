using UnityEngine;

//TODO: Implement iPhone/Android Gets functions for key paths. 
static public class PathUtils
{
	public static string TrimSuffix (string path)
	{
		return path.Remove (path.LastIndexOf ('.'));
	}

	public static string GetAssetBundlesPath ()
	{
		string path = "Assets/StreamingAssets/assetbundles/";
#if UNITY_EDITOR
		if (! System.IO.Directory.Exists (path))
			System.IO.Directory.CreateDirectory (path);   
#endif
		return path;
	}

	public static string GetAssetBundlesPathURL ()
	{
		return "file:///" + GetAssetBundlesPath ();
	}

	public static string GetResoucesPath ()
	{
		return "Assets/Resources/";
	}

	public static string GetExportedName (string file)
	{
		string name = file.Replace (PathUtils.GetResoucesPath (), ""); 
		name = name.Replace ('/', '.'); 
		name = name.Replace ("\\", ".");
		name = name + ".asset";
		return name;
	}
}