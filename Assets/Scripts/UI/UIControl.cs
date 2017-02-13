using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIControl : MonoBehaviour {

	private GameObject uiCanvas;
	private UnitController uCon;
	private BlockControl blockControls;
	[HideInInspector]
	public GameObject selectedStateB;
	[HideInInspector]
	public GameObject digButton;

	[HideInInspector]
	public GameObject pauseButton;
	[HideInInspector]
	public GameObject pauseMenuCanvas;

	[HideInInspector]
	public Text commandPointTextC;

	public GameObject spawnGunnerObj;
	public GameObject spawnFlamerObj;
	public GameObject spawnGrenaderObj;

	void Start()
	{
		uCon = GameController.instance.unitControls;
		uiCanvas = this.transform.parent.gameObject;

		blockControls = GameController.instance.blockController.GetComponent<BlockControl>();

		spawnGunnerObj = SpawnButtons (GameController.instance.gameSettings.spawnGunnerUnitButton, GameController.instance.gameSettings.gunnerUnit, GameController.instance.gameSettings.gunnerCost, GameController.instance.gameSettings.maxGunners);
		spawnFlamerObj = SpawnButtons (GameController.instance.gameSettings.spawnFlamerUnitButton, GameController.instance.gameSettings.flamerUnit, GameController.instance.gameSettings.flamerCost, GameController.instance.gameSettings.maxFlamers);
		spawnGrenaderObj = SpawnButtons (GameController.instance.gameSettings.spawnGrenaderUnitButton, GameController.instance.gameSettings.grenaderUnit, GameController.instance.gameSettings.grenaderCost, GameController.instance.gameSettings.maxGrenaders);

//		if(GameController.instance.gameSettings.selectStateButton != null)
//		{
//			selectedStateB = Instantiate (GameController.instance.gameSettings.selectStateButton, Vector3.zero, Quaternion.identity) as GameObject;
//			selectedStateB.transform.SetParent (uiCanvas.transform);
//			selectedStateB.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.selectStateButton.GetComponent<RectTransform>().anchoredPosition;
//			selectedStateB.GetComponent<Button>().onClick.AddListener(() => blockControls.ChangeState());
//		}
//		else
//		{
//			Debug.Log ("Select State button not assigned in Game Settings");
//		}
		if(GameController.instance.gameSettings.digButton != null)
		{
			digButton = Instantiate (GameController.instance.gameSettings.digButton, Vector3.zero, Quaternion.identity) as GameObject;
			digButton.transform.SetParent (uiCanvas.transform);
			digButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.digButton.GetComponent<RectTransform>().anchoredPosition;
//			foreach(Block b in blockControls.selectedBlocks)
//			{
				digButton.GetComponent<Button>().onClick.AddListener(() => blockControls.DigBlock());
//			}
			RectTransform rT = digButton.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.digButton.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;

			digButton.SetActive(false);
		}
		else
		{
			Debug.Log ("Dig Button not assigned in Game Settings");
		}

		if(GameController.instance.gameSettings.pauseButton != null)
		{
			pauseButton = Instantiate (GameController.instance.gameSettings.pauseButton, Vector3.zero, Quaternion.identity) as GameObject;
			pauseButton.transform.SetParent (uiCanvas.transform);
			pauseButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.pauseButton.GetComponent<RectTransform>().anchoredPosition;

			pauseButton.GetComponent<Button>().onClick.AddListener(() => pauseButton.GetComponent<Pause>().PauseTime(true, true));
			//			}
			RectTransform rT = pauseButton.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.pauseButton.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;
		}
		else
		{
			Debug.Log ("pauseButton not assigned in Game Settings");
		}
		if(GameController.instance.gameSettings.pauseMenuCanvas != null)
		{
			pauseMenuCanvas = Instantiate (GameController.instance.gameSettings.pauseMenuCanvas, Vector3.zero, Quaternion.identity) as GameObject;
//			pauseMenuCanvas.transform.SetParent (uiCanvas.transform);
			pauseMenuCanvas.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.pauseMenuCanvas.GetComponent<RectTransform>().anchoredPosition;
			pauseMenuCanvas.SetActive(false);
//			pauseButton.GetComponent<Button>().onClick.AddListener(() => pauseButton.GetComponent<Pause>().PauseTime(true, true));
			//			}
//			RectTransform rT = pauseMenuCanvas.GetComponent<RectTransform> ();
//			RectTransform rTPrefab = GameController.instance.gameSettings.pauseMenuCanvas.GetComponent<RectTransform> ();
//			rT.sizeDelta = rTPrefab.sizeDelta;
//			rT.anchoredPosition = rTPrefab.anchoredPosition;
//			rT.localScale = rTPrefab.localScale;
		}
		else
		{
			Debug.Log ("pauseButton not assigned in Game Settings");
		}
//		if(GameController.instance.gameSettings.restartLevelButton != null)
//		{
//			GameObject restartLevelButton = Instantiate (GameController.instance.gameSettings.restartLevelButton, Vector3.zero, Quaternion.identity) as GameObject;
//			restartLevelButton.transform.SetParent (uiCanvas.transform);
//			restartLevelButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.restartLevelButton.GetComponent<RectTransform>().anchoredPosition;
//			//			foreach(Block b in blockControls.selectedBlocks)
//			//			{
//			restartLevelButton.GetComponent<Button>().onClick.AddListener(() => Application.LoadLevel(Application.loadedLevel));
//			//			}
//			RectTransform rT = restartLevelButton.GetComponent<RectTransform> ();
//			RectTransform rTPrefab = GameController.instance.gameSettings.restartLevelButton.GetComponent<RectTransform> ();
//			rT.sizeDelta = rTPrefab.sizeDelta;
//			rT.anchoredPosition = rTPrefab.anchoredPosition;
//			rT.localScale = rTPrefab.localScale;
//		}
//		else
//		{
//			Debug.Log ("restartLevelButton not assigned in Game Settings");
//		}
//
//		if(GameController.instance.gameSettings.restartSpawnButton != null)
//		{
//			GameObject restartSpawnButton = Instantiate (GameController.instance.gameSettings.restartSpawnButton, Vector3.zero, Quaternion.identity) as GameObject;
//			restartSpawnButton.transform.SetParent (uiCanvas.transform);
//			restartSpawnButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.restartSpawnButton.GetComponent<RectTransform>().anchoredPosition;
//			//			foreach(Block b in blockControls.selectedBlocks)
//			//			{
//			restartSpawnButton.GetComponent<Button>().onClick.AddListener(() => GameController.instance.enemySpawner.Reset());
//			//			}
//		}
//		else
//		{
//			Debug.Log ("restartLevelButton not assigned in Game Settings");
//		}

		if(GameController.instance.gameSettings.timelinePrefab != null)
		{
			GameObject timelinePrefab = Instantiate (GameController.instance.gameSettings.timelinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			timelinePrefab.transform.SetParent (uiCanvas.transform);
			timelinePrefab.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.timelinePrefab.GetComponent<RectTransform>().anchoredPosition;
			//			foreach(Block b in blockControls.selectedBlocks)
			//			{
//			timelinePrefab.GetComponent<Button>().onClick.AddListener(() => GameController.instance.enemySpawner.Reset());
			//			}
			RectTransform rT = timelinePrefab.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.timelinePrefab.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;
		}
		else
		{
			Debug.Log ("timelinePrefab not assigned in Game Settings");
		}

		if(GameController.instance.gameSettings.commandPointText != null)
		{
			GameObject commandPointText = Instantiate (GameController.instance.gameSettings.commandPointText, Vector3.zero, Quaternion.identity) as GameObject;
			commandPointText.transform.SetParent (uiCanvas.transform);
			commandPointText.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.commandPointText.GetComponent<RectTransform>().anchoredPosition;
			commandPointTextC = commandPointText.GetComponent<Text>();
			RectTransform rT = commandPointText.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.commandPointText.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;
			//			foreach(Block b in blockControls.selectedBlocks)
			//			{
			//			timelinePrefab.GetComponent<Button>().onClick.AddListener(() => GameController.instance.enemySpawner.Reset());
			//			}
		}
		else
		{
			Debug.Log ("commandPointText not assigned in Game Settings");
		}
		if(GameController.instance.gameSettings.mineSpawnButton != null)
		{
			GameObject mineSpawnButton = Instantiate (GameController.instance.gameSettings.mineSpawnButton, Vector3.zero, Quaternion.identity) as GameObject;
			mineSpawnButton.transform.SetParent (uiCanvas.transform);
			mineSpawnButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.mineSpawnButton.GetComponent<RectTransform>().anchoredPosition;
			
//			mineSpawnButton.GetComponent<Button>().onClick.AddListener(() => mineSpawnButton.GetComponent<Pause>().PauseTime(true, true));
			//			}
			RectTransform rT = mineSpawnButton.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.mineSpawnButton.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;
		}
		else
		{
			Debug.Log ("mineSpawnButton not assigned in Game Settings");
		}
		if(GameController.instance.gameSettings.bombSpawnButton != null)
		{
			GameObject bombSpawnButton = Instantiate (GameController.instance.gameSettings.bombSpawnButton, Vector3.zero, Quaternion.identity) as GameObject;
			bombSpawnButton.transform.SetParent (uiCanvas.transform);
			bombSpawnButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.bombSpawnButton.GetComponent<RectTransform>().anchoredPosition;
			
			//			mineSpawnButton.GetComponent<Button>().onClick.AddListener(() => mineSpawnButton.GetComponent<Pause>().PauseTime(true, true));
			//			}
			RectTransform rT = bombSpawnButton.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.bombSpawnButton.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;
        }
        else
        {
			Debug.Log ("bombSpawnButton not assigned in Game Settings");
        }
//		if(GameController.instance.gameSettings.attackButton != null)
//		{
//			GameObject attackButton = Instantiate(GameController.instance.gameSettings.attackButton, Vector3.zero, Quaternion.identity) as GameObject;
//			attackButton.transform.SetParent(uiCanvas.transform);
//			attackButton.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.attackButton.GetComponent<RectTransform>().anchoredPosition;
//
//			attackButton.GetComponent<Button>().onClick.AddListener(() => GameController.instance.unitControls.SendAttackUnits());
//		}
//		else
//		{
//			Debug.Log("Attack Button not assigned in game settins");
//		}

	}
	public GameObject SpawnButtons(GameObject button, GameObject unit, float cost, int maxUnits)
	{
		GameObject spawnButton = Instantiate (button , Vector3.zero, Quaternion.identity) as GameObject;
		spawnButton.transform.SetParent (GameController.instance.uiCanvas.transform);
		RectTransform rT = spawnButton.GetComponent<RectTransform> ();
		RectTransform rTPrefab = button.GetComponent<RectTransform> ();
		rT.sizeDelta = rTPrefab.sizeDelta;
		rT.anchoredPosition = rTPrefab.anchoredPosition;
		rT.localScale = rTPrefab.localScale;
		spawnButton.GetComponent<SpawnUnitButton>().cost = cost;
		spawnButton.GetComponent<SpawnUnitButton>().unit = unit;

//		spawnButton.GetComponent<Button>().onClick.AddListener(() => uCon.SpawnUnit(unit, false, cost));
//		spawnButton.GetComponent<Button>().onClick.AddListener(() => uCon.ToggleUnitSpawningMode(true));
//		spawnButton.GetComponentInChildren<Text> ().text = "" + cost;
		spawnButton.GetComponent<SpawnUnitButton>().costText.GetComponent<Text>().text = "" + cost;
		spawnButton.GetComponent<SpawnUnitButton>().maxUnitsText.GetComponent<Text>().text = "0/" + maxUnits;
		return spawnButton;
	}
}
