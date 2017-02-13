using UnityEngine;
using System.Collections;

public class AudioDieTime : MonoBehaviour {


	void Start () 
	{
		if(!GetComponent<AudioSource>().loop)
			Invoke ("DestroySource", GetComponent<AudioSource>().clip.length);
	}
	void DestroySource()
	{
		Destroy (this.gameObject);
	}
}
