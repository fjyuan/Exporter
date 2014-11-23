using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;

namespace Data
{
	public static class TableManager
	{ 
		static bool isInited;

		public static bool IsInited {
			get {
				return isInited;
			}
		}

		static Dictionary<string, object> tables;

		static TableManager ()
		{
			tables = new Dictionary<string, object> ();
			isInited = false;
		}

		public static void Init ()
		{  
			AddTable<uint, TestRecord> ("test.txt");

			LoadAll ();

			isInited = true;
		}

		static void LoadAll ()
		{
			foreach (var pair in tables) {
				object tableObject = pair.Value;
				Type tableType = tableObject.GetType ();
				MethodInfo mi = tableType.GetMethod ("Load", new Type[] { typeof(string) });   
				AssetManager.Load (pair.Key, delegate(UnityEngine.Object asset) {
					TextAsset ta = asset as TextAsset;
					object[] parameters = new object[1];
					parameters [0] = ta.text;
					mi.Invoke (tableObject, parameters);
					ta = null;
					AssetManager.Unload(pair.Key);
				});
			} 
		}
		
		public static Table<K, R> GetTable<K, R> (string name)where R : IRecord<K>, new()
		{
			object table;
			if (tables.TryGetValue (name, out table)) {
				return table as Table<K, R>;
			} 
			return null;
		}

		public static void AddTable<K, R> (string name) where R : IRecord<K>, new()
		{ 
			Table<K, R> table = new Table<K, R> (name);
			tables.Add (name, table);
		}

		public static void RemoveTable (string name)
		{
			object table;
			if (tables.TryGetValue (name, out table)) {
				tables.Remove (name);
				table = null;
			}
		}

		public static void Clear ()
		{
			foreach (var pair in tables) {  
				TextAsset ta = Resources.Load (pair.Key) as TextAsset;
				Resources.UnloadAsset (ta);
			} 
			tables.Clear ();
		}
	}
}

