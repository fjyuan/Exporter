using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Data;

public class AssetManagerExample : MonoBehaviour
{ 
	public string assetName = "Male.Prefab";
	Object assetObject = null;
	List<GameObject> gameObjects = new List<GameObject> ();

	void Start ()
	{  
		AssetManager.Init (); 
		TableManager.Init ();
		//Only uncompressed asset bundles are supported by AssetBundle.CreateFromFile function. 
		//This is the fastest way to load an asset bundle.
		//bundle = AssetBundle.CreateFromFile (PathUtils.GetAssetBundlesPath() + "Cube.asset");  
	}

	void UnloadAsset ()
	{
		AssetManager.Unload (assetName);
		assetObject = null; 
	}

	void OnDisable ()
	{
		TableManager.Clear ();
		assetObject = null; 
	}

	const float ratio = 1.0f / (1024 * 1204);
	int r = 1;

	void OnGUI ()
	{ 
		Rect area = new Rect ();
		area.width = 320;
		area.height = 320;
		area.x = (int)(Screen.width - area.width) >> 1;
		area.y = (int)(Screen.height - area.height) >> 1; 
		GUILayout.BeginArea (area);
		GUI.skin.label.fontSize = 18;
		GUI.skin.button.fontSize = 18;
		GUILayout.Label (string.Format ("Profiler.GetTotalAllocatedMemory: {0:f2}m.", Profiler.GetTotalAllocatedMemory () * ratio));  
		GUILayout.Label (string.Format ("gameObjects.Count: {0:d}.", gameObjects.Count));  

		if (GUILayout.Button ("Reload Level")) {
			Application.LoadLevel (Application.loadedLevelName);
		}

		if (assetObject == null) {
			if (GUILayout.Button ("Load Asset")) {   
				AssetManager.Load (assetName, delegate(Object asset) {
					assetObject = asset;
				});
			}   
		} 
		if (this.assetObject != null) { 
			if (GUILayout.Button ("Create GameObject")) {   
				GameObject go = GameObject.Instantiate (assetObject) as GameObject;  
				gameObjects.Add (go);
				Vector3 pos = go.transform.localPosition;
				pos.x += gameObjects.Count * r + 0.5f; 
				go.transform.localPosition = pos; 
				r = -r;
			}   
			if (gameObjects.Count > 0 && GUILayout.Button ("Destroy GameObject")) { 
				GameObject go = gameObjects [gameObjects.Count - 1];
				if (gameObjects.Remove (go)) {
					GameObject.Destroy (go); 
					go = null;
				}
			}   
			if (gameObjects.Count == 0 && GUILayout.Button ("UnloadAsset")) { 
				UnloadAsset ();
			} 
		}   
		GUILayout.EndArea ();
	}
}
