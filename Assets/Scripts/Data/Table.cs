using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Data
{
	public class Table<K, R> where R : IRecord<K>, new ()
	{	
		string name;
		Dictionary<K, R> records;
		
		public Table (string path)
		{
			records = new Dictionary<K, R> (); 
			name = path;  
		}
		
		void Load ()
		{  
			if (name == null || name == string.Empty || !File.Exists (name)) {
				return;
			}   
			Stream stream = File.Open (name, FileMode.Open);
			Load (stream); 
		}

		public void Load(string str)
		{ 
			TextReader reader = new StringReader (str);
			string fields = reader.ReadLine ();   
			string table = reader.ReadToEnd ();  
			reader.Close(); 
			string[] strRecords = table.Split(new string[]{"\n"}, StringSplitOptions.RemoveEmptyEntries);    
			foreach (string record in strRecords) {
				R r = new R ();
				if (r.Parse (record)) {
					records [r.Key ()] = r;
				}
			}
			Logger.D (string.Format ("Load table: {0} \nwith fields: {1}.", name, fields.Replace('\t', ','))); 
		}

		public void Load(Stream stream)
		{ 
			TextReader reader = new StreamReader (stream);
			string fields = reader.ReadLine ();   
			string table = reader.ReadToEnd ();  
			reader.Close();
			stream.Close ();  
			string[] strRecords = table.Split (new string[1] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);    
			foreach (string record in strRecords) {
				R r = new R ();
				if (r.Parse (record)) {
					records [r.Key ()] = r;
				}
			}
			Logger.D (string.Format ("Load table: {0} \nwith fields: {1}.", name, fields)); 
		}
		
		public R GetRecord (K k)
		{
			return records [k];
		}
		
		public void Serialize ()
		{ 
			Stream s = File.Open (name, FileMode.Create);		
			BinaryFormatter formatter = new BinaryFormatter ();
			formatter.Serialize (s, records); 
			s.Close ();  
		}
		
		public void Deserialize ()
		{
			records = null;
			Stream s = File.Open (name, FileMode.Open);		
			BinaryFormatter formatter = new BinaryFormatter ();
			records = formatter.Deserialize (s) as Dictionary<K, R>; 
			s.Close ();  
		}
		
		public void SaveBinary ()
		{
			Stream s = File.Open (name, FileMode.Create);
			BinaryWriter writer = new BinaryWriter (s); 
			int count = records.Count; 
			writer.Write (count); 
			Dictionary<K, R>.KeyCollection keys = records.Keys;
			foreach (K k in keys) {
				R r = records [k];
				r.Save (ref writer);
			} 
			writer.Close ();
			s.Close ();
		}
		
		public void LoadBinary ()
		{
			records = null;
			records = new Dictionary<K, R> (); 
			Stream s = File.Open (name, FileMode.Open);
			BinaryReader reader = new BinaryReader (s); 
			int count = reader.ReadInt32 ();
			for (int i = 0; i < count; ++i) {
				R r = new R ();
				r.Read (ref reader); 
				records [r.Key ()] = r;
			}
			reader.Close ();
			s.Close ();
		}
	}
}

