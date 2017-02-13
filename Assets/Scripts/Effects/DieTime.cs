using UnityEngine;
using System.Collections;

public class DieTime : MonoBehaviour {

	public float dieTime = 0.5f;
	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, dieTime);
	}
	

}
