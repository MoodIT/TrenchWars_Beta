using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Vector3 deltaCombined;
	public bool movingCam;
	public Vector3 curVel;

	private BlockControl bControl;

	public int rightBlockZoomTo = 15;
	public int leftBlockZoomTo = 0;


	[HideInInspector]
	public Vector3 refVel;
	void Start () 
	{
//TODO		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
//TODO		StartCoroutine(FixToAspect());
	}
	

//	void LateUpdate () 
//	{
//		Debug.Log (Camera.main.WorldToScreenPoint(bControl.interactableBlockObjs[rightBlockZoomTo/2, bControl.length/2].transform.position) + " " + Screen.height);
//	}
//		if(Input.touchCount > 1)
//		{
//			movingCam = true;
//			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//#if UNITY_ANDROID || UNITY_IOS
//			Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, Camera.main.transform.position - new Vector3(Input.GetTouch(0).deltaPosition.x, 0, Input.GetTouch(0).deltaPosition.y), ref curVel, 0.2f);
//			Debug.Log("NUEGIE");
//#endif

//			for (var i = 0; i < Input.touchCount; ++i) 
//			{
//				if (Input.GetTouch(i).phase == TouchPhase.Began)
//				{
//					deltaCombined += new Vector3(Input.GetTouch(i).deltaPosition.x, 0,Input.GetTouch(i).deltaPosition.y) ;
//				}
//			}
//
//		}
//		else if(Input.GetMouseButton(0) && Input.GetMouseButton(1))
//		{
//			movingCam = true;
//			#if UNITY_EDITOR_WIN || UNITY_STANDALONE
////			Vector3inputDir1 = Quaternion.Euler(0,cameraT.rotation.eulerAngles.y,0) * inputDir1;
////			Vector3 d =  Camera.main.transform.position - new Vector3(Input.GetAxis("Mouse X"), 0 , Input.GetAxis("Mouse Y"));
////			Quaternion cDir = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y,0);
//			Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,Camera.main.transform.position - new Vector3(Input.GetAxis("Mouse X"), 0 , Input.GetAxis("Mouse Y")), ref curVel, 0.2f);
//			#endif
//		}
//		else if(movingCam)
//		{
//			movingCam = false;
//		}



//	}
	IEnumerator FixToAspect()
	{
		yield return new WaitForSeconds(0.5f);
		bool onOff = true;
		while(onOff)
		{
			if(!bControl.interactableBlockObjs[rightBlockZoomTo, bControl.length/2].GetComponent<Renderer>().IsVisibleFrom(Camera.main) || !bControl.interactableBlockObjs[leftBlockZoomTo, bControl.length/2].GetComponent<Renderer>().IsVisibleFrom(Camera.main))
			{
				transform.localPosition += -transform.forward * Time.deltaTime * 5;
//				transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transform.localPosition + -transform.forward, ref refVel, 0.5f);
			}
			else if(Camera.main.WorldToScreenPoint(bControl.interactableBlockObjs[rightBlockZoomTo/2, bControl.length/2].transform.position).y < ((Screen.height/2) + Screen.height/30))
			{
//				Debug.Log (Screen.height/30);
				transform.localPosition += -transform.up * Time.deltaTime * 5;
			}

			else
			{
				onOff = false;
				yield break;
			}				
			yield return new WaitForEndOfFrame();
		}
	}

}
