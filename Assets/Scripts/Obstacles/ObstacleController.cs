using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour {
	public enum DragState{PASSIVE, DRAGGING};
	public enum ObstacleType{MINE, BOMB};
	[HideInInspector]
	public ObstacleType curObstacleType;
	public GameObject selectingCircle;


	private UnitController uCon;
	private BlockControl bControl;
	private CommandPointsControl cPControl;

	private bool dragging;

	[HideInInspector]
	public float cost;

	//	public GameObject costText;
	//	public GameObject maxUnitsText;
	//
	private GameStats gStats;
	public Vector3 spawnPos;
//	// Use this for initialization
	void Start () 
	{
		uCon = GameController.instance.unitControls;
		gStats = GameController.instance.gameStats;
		cPControl = gStats.GetComponent<CommandPointsControl>();
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	public void Update()
	{
		if(dragging)
		{
			PlacementObstacle(curObstacleType);
		}
		if(Input.GetMouseButtonUp(0))
		{
			if(dragging)
			{
				PlaceObstacle(curObstacleType);
			}
		}
	}
	public void ObstaclePlacementActivate(ObstacleType type)
	{
		if(gStats.commandPoints >= cost)
		{
			curObstacleType = type;
			selectingCircle = InstCircle();
			dragging = true;
			foreach(GameObject u in uCon.units)
			{
				if(u != null)
					u.GetComponent<Collider>().enabled = false;
			}
		}
		
	}
	GameObject InstCircle()
	{
		GameObject circle = Instantiate(GameController.instance.gameSettings.unitPlacementCircle, Vector3.zero, GameController.instance.gameSettings.unitPlacementCircle.transform.rotation) as GameObject;
		return circle;
		//		GameController.instance.gameSettings.unitPlacementCircle
	}
	void PlacementObstacle(ObstacleType type)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100))
		{
			selectingCircle.transform.position = new Vector3(hit.transform.position.x, 0.55f, hit.transform.position.z);
			for(int i = 0; i < bControl.interactableBlockObjs.GetLength(0); i++)
			{
				for(int i2 = 0; i2 < bControl.interactableBlockObjs.GetLength(1); i2++)
				{
					if(hit.collider.gameObject != bControl.interactableBlockObjs[i,i2]) continue; //is not an interactable block

					if(bControl.interactableBlockStates[(int)hit.collider.gameObject.transform.position.x,(int)hit.collider.gameObject.transform.position.z] == 1 && hit.collider.gameObject == bControl.interactableBlockObjs[i,i2])
					{ //continue;

					spawnPos = hit.collider.gameObject.transform.position;
					}
					else spawnPos = Vector3.zero;
					//					if(hit.collider.gameObject == tempObj) continue; // is the same as previous
//					if(placingSpawn)
//					{
//						Debug.Log(hit.collider.gameObject);
//						tempObj = hit.collider.gameObject;
//						hitObj = hit.collider.gameObject;
//					}
				}
			}
		}
	}

	void PlaceObstacle(ObstacleType type)
	{
		dragging = false;
		foreach(GameObject u in uCon.units)
		{
			if(u != null)
				u.GetComponent<Collider>().enabled = true;
		}
		if(bControl.interactableBlockStates[(int)spawnPos.x,(int)spawnPos.z] == 1 && spawnPos !=  Vector3.zero)
		{
			InstObstacle(type);
		}
		Destroy (selectingCircle);
	}

	void InstObstacle(ObstacleType type)
	{
		gStats.commandPoints -= cost;
		cPControl.UpdateCommandPoints();
		if(type == ObstacleType.MINE)
		{
			GameObject g = Instantiate(GameController.instance.gameSettings.mine, new Vector3(spawnPos.x, 0.5f, spawnPos.z), Quaternion.identity) as GameObject;
			g.GetComponent<Mine>().placedOnBlock = new Block{mWidth = (int)spawnPos.x, mLength = (int)spawnPos.z};
//			g.GetComponent<SpawnObstacleButton>().costText.text = ""+GameController.instance.gameSettings.mineCost;
		}
		if(type == ObstacleType.BOMB)
		{
			GameObject g = Instantiate(GameController.instance.gameSettings.bomb, new Vector3(spawnPos.x, 20f, spawnPos.z), GameController.instance.gameSettings.bomb.transform.rotation) as GameObject;
//			g.GetComponent<SpawnObstacleButton>().costText.text = ""+GameController.instance.gameSettings.bombCost;
        }
	}
}
