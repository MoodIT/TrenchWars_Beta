using UnityEngine;
using System.Collections;

public class CameraChange : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	public GameObject camIso;
	public GameObject camPersp;
	public GameObject camPortrait;

	private int camNr;
//	void OnGUI()
//	{
//		if (GUI.Button(new Rect(Screen.width/2, Screen.height/1.1f, Screen.width/20, Screen.height/20), "Cam"))
//		{
//			if(camNr == 0)
//			{
//				camIso.SetActive(true);
//				camPersp.SetActive(false);
//				camPortrait.SetActive(false);
//			}
//			if(camNr == 1)
//			{
//				camIso.SetActive(false);
//				camPersp.SetActive(true);
//				camPortrait.SetActive(false);
//			}
//			if(camNr == 2)
//			{
//				camIso.SetActive(false);
//				camPersp.SetActive(false);
//				camPortrait.SetActive(true);
//			}
//			if(camNr <3)
//				camNr++;
//			else
//				camNr = 0;
//			Debug.Log("Clicked the button with text");
//			camIso.SetActive(!camIso.activeSelf);
//			camPersp.SetActive(!camPersp.activeSelf);
//		}

//		if (GUI.Button(new Rect(Screen.width/2.2f, Screen.height/1.1f, 50, 30), "Iso"))
//		{	
//			camIso.SetActive(true);
//			camPersp.SetActive(false);
//		}
//	}
}
