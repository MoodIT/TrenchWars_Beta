using UnityEngine;
using System.Collections;

public class ObjectiveController : MonoBehaviour {
	[HideInInspector]
	public Flag[] flags;
//	[HideInInspector]
	public int flagCount;
	public bool completeInOrder;
	[HideInInspector]
	public int completedFlags;
	// Use this for initialization
	void Start () 
	{
		flags = new Flag[GameObject.FindObjectsOfType<Flag>().Length];
		for(int i = 0; i < GameObject.FindObjectsOfType<Flag>().Length; i++)
		{
			flags[i] = GameObject.FindObjectsOfType<Flag>()[i].GetComponent<Flag>();
			flagCount++;
		}
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}

	public void UnitMovementEnd()
	{
		foreach(Flag f in flags)
		{
			if(f.nearbyDug)
			{
				f.MovementUpdate();
			}
		}
	}
}
