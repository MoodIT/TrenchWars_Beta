using UnityEngine;
using System.Collections;

public class StopEmission : MonoBehaviour {
	public float stopTime = 1;
	void Start () 
	{
		StartCoroutine (EmissionEnd ());
	}
	
	IEnumerator EmissionEnd()
	{
		yield return new WaitForSeconds (stopTime);
		GetComponent<ParticleSystem> ().emissionRate = 0;
	}
}
