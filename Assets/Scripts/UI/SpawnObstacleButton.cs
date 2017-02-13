using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class SpawnObstacleButton : MonoBehaviour, IPointerDownHandler// required interface when using the OnPointerDown method.
{
	[HideInInspector]
	public float cost;
//	[HideInInspector]
	public ObstacleController.ObstacleType type;
//	public GameObject unit;
	private UnitController uCon;
	private ObstacleController obstCon;
	public GameObject effect;

	public Text costText;
//	public GameObject costText;
//	public GameObject maxUnitsText;
//
	private GameStats gStats;
	void Start()
	{
		obstCon = GameController.instance.obstacleController;
		gStats = GameController.instance.gameStats;
		uCon = GameController.instance.unitControls;
		if(type == ObstacleController.ObstacleType.MINE)
		{
//			obstCon.cost = GameController.instance.gameSettings.mineCost;
						costText.text = ""+GameController.instance.gameSettings.mineCost;
			
		}
		if(type == ObstacleController.ObstacleType.BOMB)
		{
//			obstCon.cost = GameController.instance.gameSettings.bombCost;
						costText.text = ""+GameController.instance.gameSettings.bombCost;
		}
	}
	//Do this when the mouse is clicked over the selectable object this script is attached to.
	public void OnPointerDown (PointerEventData eventData) 
	{
//		obstCon.cost = GameController.instance.gameSettings.mineCost;
		if(type == ObstacleController.ObstacleType.MINE)
		{
			obstCon.cost = GameController.instance.gameSettings.mineCost;
//			costText.text = ""+GameController.instance.gameSettings.mineCost;

		}
		if(type == ObstacleController.ObstacleType.BOMB)
		{
			obstCon.cost = GameController.instance.gameSettings.bombCost;
//			costText.text = ""+GameController.instance.gameSettings.bombCost;
		}
		obstCon.ObstaclePlacementActivate(type);

//		switch(unit.GetComponent<UnitShooting>().weaponType)
//		{
//		case UnitShooting.WeaponType.FLAMER:
//
//			if(uCon.flamerAmt < GameController.instance.gameSettings.maxFlamers && gStats.commandPoints >= GameController.instance.gameSettings.flamerCost)
//			{
//				uCon.unitType = UnitShooting.WeaponType.FLAMER;
//				SendToUnitCon();
//			}
//			break;
//		case UnitShooting.WeaponType.GRENADER:
//			if(uCon.grenaderAmt < GameController.instance.gameSettings.maxGrenaders && gStats.commandPoints >= GameController.instance.gameSettings.grenaderCost)
//			{
//				uCon.unitType = UnitShooting.WeaponType.GRENADER;
//				SendToUnitCon();
//			}
//			break;
//		case UnitShooting.WeaponType.MACHINEGUN:
//			if(uCon.gunnerAmt < GameController.instance.gameSettings.maxGunners && gStats.commandPoints >= GameController.instance.gameSettings.gunnerCost)
//			{
//				uCon.unitType = UnitShooting.WeaponType.MACHINEGUN;
//				SendToUnitCon();
//			}
//			break;
//		}


	}

	void SendToUnitCon()
	{
//		uCon.ToggleUnitSpawningMode(true);
//		uCon.cost = cost;
//		uCon.spawningUnitType = unit;
	}

}
