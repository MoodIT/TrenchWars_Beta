using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

//	public bool pause;
	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	private GameStats gStats;

	void Start()
	{
		gStats = GameController.instance.gameStats;
	}
	public void PauseTime(bool onOff, bool showMenu)
	{
		if(onOff)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}

		if(showMenu)
		{
			if(gStats.gameComplete)
			{
				if(gStats.vCanvas != null)
					gStats.vCanvas.SetActive(false);

				if(gStats.eGCanvas != null)
					gStats.eGCanvas.SetActive(false);
			}

			GameController.instance.uiControls.pauseMenuCanvas.SetActive(true);
		}
		else
		{
			if(gStats.gameComplete)
			{
				if(gStats.vCanvas != null)
					gStats.vCanvas.SetActive(true);
				
				if(gStats.eGCanvas != null)
					gStats.eGCanvas.SetActive(true);
			}
			GameController.instance.uiControls.pauseMenuCanvas.SetActive(false);        
		}
	}

	void OnApplicationPause(bool pauseStatus) 
	{
		if(pauseStatus)
		{
			PauseTime(true,true);
		}
//		else
//		{
//			PauseTime(false, false);
//		}
	}
}
