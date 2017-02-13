using UnityEngine;
using System.Collections;
using DG.Tweening;
public class GameController : MonoBehaviour {
	//Here is a private reference only this class can access
	private static GameController _instance;

	public GameSettings gameSettings;

	public GameObject uiCanvas = null;

	public GameObject blockController = null;
	public bool blockControllerInstantiated;

	public GameObject uiControllerObj = null;

	public UIControl uiControls;

	public GameObject unitControllerObj = null;

	public GameObject enemySpawnerObj = null;

	public EnemySpawning enemySpawner;
	public bool enemySpawnerInstantiated;
	public UnitController unitControls;

	public GameStats gameStats;

	public GameObject gameStatsObj;

	public GameObject obstacleControllerObj;
	public ObstacleController obstacleController;

	public GameObject objectiveControllerObj;
	public ObjectiveController objectiveController;

	public GameObject audioControllerObj;
	public AudioController audioController;

	public GameObject mainCam;

	public static bool splashShown;
	public static GameController instance 
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<GameController>();
			return _instance;
		}
	}

	void Awake () 
	{
		DOTween.Init();
		StartCoroutine(ClearAssets());

		if(gameSettings != null)
		{
			//UI Canvas Instantiate
			if (gameSettings.uiCanvas != null) 
			{
				//instantiate and declare
				this.uiCanvas = Instantiate (gameSettings.uiCanvas, Vector3.zero, Quaternion.identity) as GameObject;
				uiCanvas.gameObject.name = "UiCanvas";

			} else 
			{
				Debug.Log ("No UI Canvas assigned in Game Settings");
			}
			//Block Controller instantiate
			if(gameSettings.blockController != null)
			{
				if(!blockControllerInstantiated)
				{
					this.blockController = Instantiate(gameSettings.blockController, Vector3.zero, Quaternion.identity) as GameObject;
					blockController.gameObject.name = "BlockController";
				}
				else
				{
					this.blockController = GameObject.FindObjectOfType<BlockControl>().gameObject;
					blockController.gameObject.name = "BlockController";
				}
			}
			else
			{
				Debug.Log ("No Block Controller assigned in Game Settings");
			}
			//UI Controller instantiate
			if(gameSettings.uiController != null && gameSettings.uiCanvas != null)
			{
				this.uiControllerObj = Instantiate(gameSettings.uiController, Vector3.zero, Quaternion.identity) as GameObject;
				uiControllerObj.gameObject.name = "UIController";
				uiControllerObj.transform.SetParent(uiCanvas.transform);
				uiControls = uiControllerObj.GetComponent<UIControl>();
			}
			else
			{
				Debug.Log ("No UI Controller, or UI Canvas, assigned in Game Settings");
			}
			//Unit Controller instantiate
			if(gameSettings.unitController != null)
			{
				this.unitControllerObj = Instantiate(gameSettings.unitController, Vector3.zero, Quaternion.identity) as GameObject;
				unitControllerObj.gameObject.name = "UnitController";
				unitControls = unitControllerObj.GetComponent<UnitController>();
			}
			else
			{
				Debug.Log ("No Unit Controller assigned in Game Settings");
			}
			if(gameSettings.enemySpawner != null)
			{
				if(!enemySpawnerInstantiated)
				{
				this.enemySpawnerObj = Instantiate(gameSettings.enemySpawner, Vector3.zero, Quaternion.identity) as GameObject;
				enemySpawnerObj.gameObject.name = "EnemySpawner";
				enemySpawner = enemySpawnerObj.GetComponent<EnemySpawning>();
				}
				else
				{
					this.enemySpawnerObj = GameObject.FindObjectOfType<EnemySpawning>().gameObject;
					enemySpawner = enemySpawnerObj.GetComponent<EnemySpawning>();
					enemySpawnerObj.gameObject.name = "EnemySpawner";
				}
			}
			else
			{
				Debug.Log ("No Unit Controller assigned in Game Settings");
			}

			if(gameSettings.gameStats != null)
			{
				this.gameStatsObj = Instantiate(gameSettings.gameStats, Vector3.zero, Quaternion.identity) as GameObject;
				gameStatsObj.gameObject.name = "GameStats";
				gameStats = gameStatsObj.GetComponent<GameStats>();
			}
			else
			{
				Debug.Log ("No GameStats assigned in Game Settings");
			}
			if (gameSettings.obstacleControls != null) 
			{
				//instantiate and declare
				this.obstacleControllerObj = Instantiate (gameSettings.obstacleControls, Vector3.zero, Quaternion.identity) as GameObject;
				obstacleControllerObj.gameObject.name = "ObstacleController";
				obstacleController = obstacleControllerObj.GetComponent<ObstacleController>();
				
			} else 
			{
				Debug.Log ("No ObstacleController assigned in Game Settings");
			}

			if (gameSettings.objectiveController != null) 
			{
				//instantiate and declare
				this.objectiveControllerObj = Instantiate (gameSettings.objectiveController, Vector3.zero, Quaternion.identity) as GameObject;
				objectiveControllerObj.gameObject.name = "ObjectiveController";
				objectiveController = objectiveControllerObj.GetComponent<ObjectiveController>(); 
				
			} else 
			{
				Debug.Log ("No ObjectiveController assigned in Game Settings");
			}

			if (gameSettings.audioController != null) 
			{
				//instantiate and declare
				this.audioControllerObj = Instantiate (gameSettings.audioController, Vector3.zero, Quaternion.identity) as GameObject;
				audioControllerObj.gameObject.name = "AudioController";
				audioController = audioControllerObj.GetComponent<AudioController>(); 
				
			} else 
			{
				Debug.Log ("No AudioController assigned in Game Settings");
			}
		}
		else
		{
			Debug.Log ("No GameSettings assigned in Game Controller");
		}
	}
	IEnumerator ClearAssets()
	{
		yield return new WaitForSeconds(1);
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}


}
