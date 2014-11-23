public class Tester
{
	public delegate void VoidCallfunc ();

	public static float TimeConsumed (VoidCallfunc callfunc, string title)
	{
		float time = UnityEngine.Time.time;
		callfunc ();
		time = UnityEngine.Time.time - time; 
		UnityEngine.Debug.Log (title + " TimeConsumed with " + time + " ms."); 
		return time;
	} 
}