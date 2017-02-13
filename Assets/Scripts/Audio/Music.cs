using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {
	public AudioSource source;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		source.Play();
		if(GameObject.FindObjectOfType<Music>() != this)
		{
//			GameObject g = GameObject.FindObjectOfType<Music>().gameObject;
			Destroy(GameObject.FindObjectOfType<Music>().gameObject); // LAZYYY FIX TO SINGLETOOON
		}
	}
	

}
