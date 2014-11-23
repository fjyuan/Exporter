using System;

namespace Data
{
	public static class Logger
	{
		public static void D (string s)
		{ 
			UnityEngine.Debug.Log (s);
		}

		public static void W (string s)
		{
			UnityEngine.Debug.LogWarning (s);
		}

		public static void E (string s)
		{
			UnityEngine.Debug.LogError (s);
		}
		
	}
}

