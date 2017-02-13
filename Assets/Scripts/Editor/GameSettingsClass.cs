using UnityEngine;
using UnityEditor;

public class GameSettingsClass
{
	[MenuItem("Assets/Create/GameSettings")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<GameSettings> ();
	}
}