using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Xml;

static public class VersionManager
{ 
	static Dictionary<string, string> dictOfFileWithCode;
	static Dictionary<string, string> dictOfFileWithCodeFromXML;
	static MD5CryptoServiceProvider md5Generator;
	static string path_to_md5codes = PathUtils.GetAssetBundlesPath () + "md5codes.xml";
	static string path_to_versions = PathUtils.GetAssetBundlesPath () + "versions.xml";
	const string keyname = "name";
	const string keyversion = "version";
	const string keycode = "code";

	public static void Began ()
	{ 
		md5Generator = new MD5CryptoServiceProvider ();
		dictOfFileWithCode = new Dictionary<string, string> ();   
		dictOfFileWithCodeFromXML = ReadDictionary (path_to_md5codes, keyname, keycode);   
	}

	public static bool Update (string file)
	{
		FileStream stream = System.IO.File.Open (file, FileMode.Open);
		byte[] hash = md5Generator.ComputeHash (stream);
		string code = System.BitConverter.ToString (hash); 
		string name = PathUtils.GetExportedName (file);
		stream.Close (); 
		bool updated = false;
		if (dictOfFileWithCodeFromXML.ContainsKey (name)) {
			string oldcode = dictOfFileWithCodeFromXML [name];
			if (! oldcode.Equals (code)) {
				updated = true;
			}
		} else {
			updated = true;
		} 
		if (updated) {
			dictOfFileWithCode [name] = code;
		} 
		return updated;
	}

	public static void Ended ()
	{     
		Dictionary<string, string> dictOfFileWithVersionFromXML = ReadDictionary (path_to_versions, keyname, keyversion); 

		foreach (KeyValuePair<string, string> pair in dictOfFileWithCodeFromXML) { 
			string oldcode = pair.Value;
			int v = 1; 
			if (dictOfFileWithCode.ContainsKey (pair.Key)) { 
				string code = dictOfFileWithCode [pair.Key];
				if (dictOfFileWithVersionFromXML.ContainsKey (pair.Key)) {
					if (! code.Equals (oldcode)) {
						v += int.Parse (dictOfFileWithVersionFromXML [pair.Key]); 
					}
				} 
			} else {
				dictOfFileWithCode.Add (pair.Key, oldcode);
			}
			dictOfFileWithVersionFromXML [pair.Key] = "" + v;
		}  

		Dictionary<string, string>.KeyCollection keys = dictOfFileWithCode.Keys;
		foreach (var i in keys) {
			if (! dictOfFileWithVersionFromXML.ContainsKey (i)) {
				dictOfFileWithVersionFromXML [i] = "" + 1;
			}
		}    

		SaveDictionary (path_to_versions, dictOfFileWithVersionFromXML, keyname, keyversion); 
		SaveDictionary (path_to_md5codes, dictOfFileWithCode, keyname, keycode);  

		md5Generator = null;

		dictOfFileWithVersionFromXML.Clear ();
		dictOfFileWithVersionFromXML = null;

		dictOfFileWithCode.Clear ();
		dictOfFileWithCode = null;

		dictOfFileWithCodeFromXML.Clear ();
		dictOfFileWithCodeFromXML = null;
	}

	public static List<string> GetUpdatedFiles (Dictionary<string, string> oldversions, Dictionary<string, string> versions)
	{
		List<string> updatedFiles = new List<string> ();
		foreach (KeyValuePair<string, string> pair in versions) {
			if (! oldversions.ContainsKey (pair.Key)) {
				updatedFiles.Add (pair.Key);
			} else {
				if (int.Parse (pair.Value) > int.Parse (oldversions [pair.Key])) {
					updatedFiles.Add (pair.Key);
				}
			} 
		}
		return updatedFiles;
	}

	static Dictionary<string, string> ReadDictionary (string path, string keyname, string valname)
	{
		Dictionary<string, string> dict = new Dictionary<string, string> (); 
		if (! System.IO.File.Exists (path))
			return dict; 
		XmlDocument doc = new XmlDocument ();
		doc.Load (path);
		XmlElement root = doc.DocumentElement; 
		foreach (XmlNode node in root.ChildNodes) {
			if ((node is XmlElement) == false)
				continue; 
			string file = (node as XmlElement).GetAttribute (keyname);
			string code = (node as XmlElement).GetAttribute (valname); 
			if (! dict.ContainsKey (file)) {
				dict.Add (file, code);
			}
		} 
		root = null;
		doc = null; 
		return dict;
	}

	static void SaveDictionary (string path, Dictionary<string, string> dict, string keyname, string valname)
	{
		XmlDocument doc = new XmlDocument ();
		XmlElement root = doc.CreateElement ("files");
		doc.AppendChild (root);
		foreach (KeyValuePair<string, string> pair in dict) {
			XmlElement element = doc.CreateElement ("file");
			root.AppendChild (element);
			element.SetAttribute (keyname, pair.Key);
			element.SetAttribute (valname, pair.Value);
		}
		doc.Save (path);
		doc = null;
	} 
}

