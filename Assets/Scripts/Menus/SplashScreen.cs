using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SplashScreen : MonoBehaviour {
	public float duration = 5;
	public Image image;
	public static bool splashShown;
	// Use this for initialization
	void Start () {
		if(!GameController.splashShown)
			StartCoroutine(ImageFade());
		else
			this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator ImageFade()
	{
		yield return new WaitForSeconds(duration);
//		Destroy(image.gameObject);
		this.gameObject.SetActive(false);
		GameController.splashShown = true;
		yield break;
	}
}
