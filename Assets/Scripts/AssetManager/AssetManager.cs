using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour
{
	private class AssetRef
	{
		public string name;
		public Object asset; 
	}

	static Dictionary<string, AssetRef> dictOfAssetRefs = new Dictionary<string, AssetRef> ();

	public enum LoadType
	{
		LoadWithAssetBundleCreateFromFile,
		LoadWithAssetBundleManager,  
		LoadResources
	}

	static LoadType loadType = LoadType.LoadResources;
	static AssetManager instance = null;

	static AssetManager Instance {
		get { 
			return instance;
		}
	}

	const string NAME = "AssetManager";
	public static void Init ()
	{
		GameObject go = GameObject.Find (NAME);
		if (go == null) {
			go = new GameObject (NAME);
			GameObject.DontDestroyOnLoad (go);
			instance = go.AddComponent<AssetManager> (); 
		}
	}

	public static void Clear ()
	{
		foreach (string name in dictOfAssetRefs.Keys)
			Unload (name);
		dictOfAssetRefs.Clear ();
	}

	static void TryToAddAsset (string name, Object asset)
	{
		AssetRef assetRef;
		if (! dictOfAssetRefs.TryGetValue (name, out assetRef)) {
			assetRef = new AssetRef ();
			assetRef.name = name;
			assetRef.asset = asset; 
			dictOfAssetRefs.Add (assetRef.name, assetRef); 
		}
	}

	public delegate void OnFinished (UnityEngine.Object asset);

	public static void Load (string name, OnFinished onFinished)
	{  
		switch (loadType) { 
		case LoadType.LoadWithAssetBundleCreateFromFile: 
			if (null != onFinished) { 
				string file = PathUtils.GetAssetBundlesPath () + PathUtils.GetExportedName (name);
				Object asset = AssetBundleManager.LoadAssetBundleWithFile (file).mainAsset; 
				TryToAddAsset (name, asset);
				onFinished (asset); 
			}
			break;
		case LoadType.LoadWithAssetBundleManager:
			if (null != onFinished) {
				Instance.StartCoroutine (Instance.DownloadAB (name, onFinished));
			}
			break;
		case LoadType.LoadResources:
		default:
			if (null != onFinished) { 
				string path = PathUtils.TrimSuffix(name);
				Object asset = Resources.Load (path);
				TryToAddAsset (name, asset);
				onFinished (asset);
			}
			break; 
		}
	}

	public static void Unload (string name)
	{
		AssetRef assetRef;
		if (dictOfAssetRefs.TryGetValue (name, out assetRef)) {
			switch (loadType) { 
			case LoadType.LoadWithAssetBundleCreateFromFile:
				{
					string url = PathUtils.GetAssetBundlesPath () + PathUtils.GetExportedName (name);
					AssetBundleManager.Unload (url, true);

				}
				break;
			case LoadType.LoadWithAssetBundleManager:
				{
					string url = PathUtils.GetAssetBundlesPathURL () + PathUtils.GetExportedName (name);
					AssetBundleManager.Unload (url, true);
				}
				break;
			case LoadType.LoadResources:
			default:
				{ 
					Resources.UnloadAsset (dictOfAssetRefs [name].asset);
					dictOfAssetRefs [name].asset = null;
				}
				break; 
			}  
			assetRef.asset = null;
			dictOfAssetRefs.Remove (name);
		}
	}

	IEnumerator DownloadAB (string name, OnFinished onFinished)
	{ 
		string url = PathUtils.GetAssetBundlesPathURL () + PathUtils.GetExportedName (name); 
		yield return StartCoroutine (AssetBundleManager.downloadAssetBundle (url, 1));
		AssetBundle bundle = AssetBundleManager.getAssetBundle (url, 1);  
		TryToAddAsset (name, bundle.mainAsset);
		onFinished (bundle.mainAsset);	 
	}
}

