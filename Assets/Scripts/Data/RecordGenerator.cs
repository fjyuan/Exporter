using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Data
{
	public class Field
	{ 
		/// <summary>
		/// The type.
		/// can be set value with follows:
		/// Boolean
		/// Byte
		/// SByte
		/// Char
		/// Int16
		/// UInt16
		/// Int32
		/// UInt32
		/// Int64
		/// UInt64
		/// Single
		/// Double
		/// Decimal
		/// String
		/// Char[]
		/// Byte[]  
		/// </summary>
		public Type type = null;
		public string name = null; 
	}

	public static class RecordGenerator
	{
		static RecordGenerator ()
		{
		}

		public static void Generate (string recordName, Field[] recordFields)
		{
			string filename = "..\\..\\Data\\Records\\" + recordName + ".cs";
			Stream stream = File.Open (filename, FileMode.Create);
			StreamWriter writer = new StreamWriter (stream);
			Field keyField = recordFields [0]; 
			//file-began
			writer.WriteLine ("using System; ");
			writer.WriteLine ("using System.Reflection; ");
			writer.WriteLine ("using System.IO; ");
			writer.WriteLine ("\n\n" + "namespace Data");
			writer.WriteLine ("{"); 
			//class 
			string indent = "\t"; 
			writer.WriteLine (indent + "[Serializable]");
			writer.WriteLine (indent + "public class " + recordName + ": IRecord<" + keyField.type.Name + ">");
			writer.WriteLine (indent + "{"); 
			indent = "\t\t"; 
			//fields
			foreach (Field f in recordFields) { 
				writer.WriteLine (indent + "public " + f.type + " " + f.name + ";");
			}  
			//methods
			writer.WriteLine ();
			#region Parse
			writer.WriteLine (indent + "public bool Parse (string record)");
			writer.WriteLine (indent + "{");   
			writer.WriteLine (indent + "\tstring[] fields = record.Split ('\\t');");
			writer.WriteLine (indent + "\tif (fields.Length != GetType ().GetFields ().Length) { ");
			writer.WriteLine (indent + "\t\tLogger.D (\"Failed to parse a record with: \" + record);");
			writer.WriteLine (indent + "\t\treturn false;");
			writer.WriteLine (indent + "\t}"); 
			int i = 0;
			string fieldsStr = null;
			foreach (Field f in recordFields) {
				if (f.type == typeof(string)) {
					writer.WriteLine (indent + "\t" + f.name + "=fields[" + i + "];");
				} else {
					writer.WriteLine (indent + "\t" + f.name + "=" + f.type + ".Parse(fields[" + i + "]);");
				}
				fieldsStr += f.name + " + \",\" + ";
				++i;
			} 
			fieldsStr = fieldsStr.Trim (' ', '+', ' ', '\"', ',', '\"', ' ', '+', ' ');
			writer.WriteLine (indent + "\tLogger.D (string.Format (\"Parse a record with: \" + " + fieldsStr + "));"); 
			writer.WriteLine (indent + "\treturn true" + ";"); 
			writer.WriteLine (indent + "}");
			#endregion  
			writer.WriteLine (); 
			#region Save
			writer.WriteLine (indent + "public void Save (ref BinaryWriter writer)"); 
			writer.WriteLine (indent + "{");
			foreach (Field f in recordFields) {
				writer.WriteLine (indent + "\twriter.Write(" + f.name + ");");
			} 
			writer.WriteLine (indent + "}");
			#endregion  
			writer.WriteLine (); 
			#region Read
			writer.WriteLine (indent + "public void Read (ref BinaryReader reader)"); 
			writer.WriteLine (indent + "{"); 
			Dictionary<string, string> nameDict = new Dictionary<string, string> ();
			Type type = typeof(BinaryReader);
			MethodInfo[] methods = type.GetMethods ();
			foreach (MethodInfo m in methods) {
				if (m.Name.Contains ("Read") && ! m.Name.Equals ("Read")) { 
					nameDict.Add (m.ReturnType.Name, m.Name);
				}
			}  
			foreach (Field f in recordFields) { 
				writer.WriteLine (indent + "\t" + f.name + " = reader." + nameDict [f.type.Name] + "();");
			} 
			writer.WriteLine (indent + "}");
			#endregion  
			writer.WriteLine ();
			#region Key
			writer.WriteLine (indent + "public " + keyField.type.Name + " Key ()");  
			writer.WriteLine (indent + "{");
			writer.WriteLine (indent + "\treturn " + keyField.name + ";");
			writer.WriteLine (indent + "}");
			#endregion
			#region Reflection
			//			Type t = typeof(IRecord<T>); 
			//			MethodInfo[] methods = t.GetMethods ();
			//			foreach (MethodInfo m in methods) {
			//				string permission = (m.IsPublic ? "public" : (m.IsPrivate ? "private" : "")); 
			//				ParameterInfo[] parameters = m.GetParameters ();
			//				string parametersStr = "";  
			//				foreach (ParameterInfo p in parameters) {
			//					parametersStr += p.ParameterType.Name + " " + p.Name + ", ";
			//				} 
			//				parametersStr = parametersStr.TrimEnd (',', ' ');
			//				writer.WriteLine (indent + permission + " " + m.ReturnType.Name + " " + m.Name + "(" + parametersStr + ")");
			//				writer.WriteLine (indent + "{");  
			//
			//				if (m.ReturnType != typeof(void)) {
			//					if (m.Name.Equals ("Key")) {
			//						writer.WriteLine (indent + "\treturn " + keyname + ";");
			//					} else if (m.Name.Equals ("Parse")) { 
			//						writer.WriteLine (indent + "\treturn false" + ";"); 
			//					}
			//				} else {
			//
			//				} 
			//				writer.WriteLine (indent + "}"); 
			//			}
			#endregion 
			//class-end
			indent = "\t";
			writer.WriteLine (indent + "}"); 
			//file-ended
			writer.WriteLine ("}");
			writer.Close (); 
			stream.Close ();
		}  
	}
}

