using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	public AudioSource StartPlay(AudioClip aC, bool pitchRnd, float volume = default(float), bool loop = default(bool))
	{
		GameObject aS = Instantiate(GameController.instance.gameSettings.audioSourceObj, Vector3.zero, Quaternion.identity) as GameObject;
		aS.GetComponent<AudioSource>().loop = loop;
		PlayClip(aS.GetComponent<AudioSource>(), aC, pitchRnd, volume);
		return aS.GetComponent<AudioSource>();
	}
	void PlayClip(AudioSource aS, AudioClip aC, bool pitchRnd, float volume)
	{
		aS.clip = aC;
		if(pitchRnd)
			aS.pitch = Random.Range(0.85f, 1.2f);

		if(volume != 0)
			aS.volume = volume;

		aS.Play();
	}

	public void DestroyAudio(AudioSource aS)
	{
		//implement fadeout
		Destroy (aS.gameObject);
	}
}
