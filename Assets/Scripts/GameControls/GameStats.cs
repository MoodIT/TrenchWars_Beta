using UnityEngine;
using System.Collections;
[System.Serializable]
public class GameStats : MonoBehaviour {

	[HideInInspector]
	public float commandPoints;
	[HideInInspector]
	public float counter = 0;

//	private BlockControl bControl;
	private UnitController uCon;
	private EnemySpawning enmSpawn;

	private bool stopCount;
	public Transform winningCamT;
//	public float levelDuration = 75;
	[HideInInspector]
	public bool gameComplete;

	public GameObject vCanvas;
	public GameObject eGCanvas;
	void Start()
	{
		enmSpawn = GameController.instance.enemySpawner;
		uCon = GameController.instance.unitControls;
//		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		commandPoints = GameController.instance.gameSettings.startCommandPoints;
	}

	void Update()
	{
		if(!stopCount)
			counter = Time.timeSinceLevelLoad;
	}

	public void EndGame()
	{
		gameComplete = true;
		eGCanvas = Instantiate (GameController.instance.gameSettings.endGameCanvas, Vector3.zero, Quaternion.identity) as GameObject;
		GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(true, false);
		GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.endGameClip, false);
//		Time.timeScale = 0.01f;
	}

	public void WinGame()
	{
		gameComplete = true;
		foreach(GameObject u in uCon.units)
		{
			if(u != null)
			{
				Animator a = u.GetComponent<UnitMovement>().animator;
				a.SetBool("Dance",true);
				a.SetBool("Shoot",false);
				a.SetBool("InPosition",false);
			}
		}

		foreach(GameObject e in enmSpawn.enemiesInGame)
		{
			e.GetComponent<EnemyStats>().health = 0;
		}
		Camera.main.transform.position = winningCamT.position;
		Camera.main.transform.rotation = winningCamT.rotation;
//		GameController.instance.mainCam.transform.position = 
		stopCount = true;
		enmSpawn.stopSpawn = true;
		vCanvas = Instantiate (GameController.instance.gameSettings.victoryCanvas, Vector3.zero, Quaternion.identity) as GameObject;
//		GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(true, false);
		GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.winGameClip, false);

	}
}
