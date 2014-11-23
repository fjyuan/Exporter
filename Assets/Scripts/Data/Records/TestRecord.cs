using System;
using System.Reflection;
using System.IO;

namespace Data
{
	[Serializable]
	public class TestRecord : IRecord<uint>
	{
		public uint   id;
		public string name;
		public string desc;
		public string path;
		
		public TestRecord ()
		{  
		}
		
		public override string ToString ()
		{
			return string.Format ("[{0}, {1}, {2}, {3}]", id, name, desc, path);
		}

		#region IRecord[System.UInt32] implementation
		public bool Parse (string record)
		{
			string[] fields = record.Split ('\t'); 
			if (fields.Length != GetType ().GetFields ().Length) { 
				Logger.D ("Failed to parse a record with: " + record); 
				return false;
			}    
			id = uint.Parse (fields [0]);
			name = fields [1];
			desc = fields [2];
			path = fields [3];  
			Logger.D (string.Format ("Parse a record with: {0}, {1}, {2}, {3}", id, name, desc, path)); 
			return true;
		}
		
		public void Save (ref BinaryWriter writer)
		{
			writer.Write(id);
			writer.Write(name);
			writer.Write(desc);
			writer.Write(path); 
		}

		public void Read (ref BinaryReader reader)
		{
			id = reader.ReadUInt32();
			name = reader.ReadString();
			desc = reader.ReadString();
			path = reader.ReadString(); 
		}

		public uint Key ()
		{
			return id;
		} 
		#endregion
	}
}

