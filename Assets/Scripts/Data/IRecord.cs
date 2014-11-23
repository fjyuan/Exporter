using System;
using System.IO;

namespace Data
{
	public interface IRecord<K>
	{
		bool Parse (string record);
		
		void Save (ref BinaryWriter writer);

		void Read (ref BinaryReader reader);
		
		K Key ();  
	}
}

