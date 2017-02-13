using UnityEngine;
using System.Collections;

public class Ambient : MonoBehaviour {

	private AudioController audioCon;

	void Start () {
		audioCon = GameController.instance.audioController;
		audioCon.StartPlay(GameController.instance.gameSettings.ambientSound1,false,0.1f, true);
		StartCoroutine(GunFire ());
	}

	IEnumerator GunFire()
	{
		yield return new WaitForSeconds(Random.Range (4,8.5f));
		if(Random.Range(0,2) == 1)
		{
			audioCon.StartPlay(GameController.instance.gameSettings.ambientSound2,true,0.1f, false);
			yield return new WaitForSeconds(Random.Range(0.5f,2));
		}
		audioCon.StartPlay(GameController.instance.gameSettings.ambientSound2,true,0.1f, false);
		StartCoroutine(GunFire());
		yield break;
	}
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
