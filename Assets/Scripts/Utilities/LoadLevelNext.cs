using UnityEngine;
using System.Collections;

public class LoadLevelNext : MonoBehaviour{
	public static string nextLevel;
	void Awake()
	{
		Application.LoadLevelAdditive(nextLevel);
	}
}
