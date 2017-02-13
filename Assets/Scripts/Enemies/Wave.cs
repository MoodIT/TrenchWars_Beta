using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wave {

	public float time;
	[HideInInspector]
	public bool spawned;
//		get{return mTime; } set{ mTime = value; }
	
//	public float mTime;

	public Wave(float newTime)
	{
		time = newTime;
	}
}
