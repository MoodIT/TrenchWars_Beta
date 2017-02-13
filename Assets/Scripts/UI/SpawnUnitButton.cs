using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class SpawnUnitButton : MonoBehaviour, IPointerDownHandler// required interface when using the OnPointerDown method.
{
	[HideInInspector]
	public float cost;
	[HideInInspector]
	public GameObject unit;
	private UnitController uCon;
	public GameObject costText;
	public GameObject maxUnitsText;

	private GameStats gStats;
	void Start()
	{
		gStats = GameController.instance.gameStats;
		uCon = GameController.instance.unitControls;
	}
	//Do this when the mouse is clicked over the selectable object this script is attached to.
	public void OnPointerDown (PointerEventData eventData) 
	{
		if(eventData.button == PointerEventData.InputButton.Left)
		{
//		eventData.currentInputModule == 
			switch(unit.GetComponent<UnitShooting>().weaponType)
			{
			case UnitShooting.WeaponType.FLAMER:

				if(uCon.flamerAmt < GameController.instance.gameSettings.maxFlamers && gStats.commandPoints >= GameController.instance.gameSettings.flamerCost)
				{
					uCon.unitType = UnitShooting.WeaponType.FLAMER;
					SendToUnitCon();
				}
				break;
			case UnitShooting.WeaponType.GRENADER:
				if(uCon.grenaderAmt < GameController.instance.gameSettings.maxGrenaders && gStats.commandPoints >= GameController.instance.gameSettings.grenaderCost)
				{
					uCon.unitType = UnitShooting.WeaponType.GRENADER;
					SendToUnitCon();
				}
				break;
			case UnitShooting.WeaponType.MACHINEGUN:
				if(uCon.gunnerAmt < GameController.instance.gameSettings.maxGunners && gStats.commandPoints >= GameController.instance.gameSettings.gunnerCost)
				{
					uCon.unitType = UnitShooting.WeaponType.MACHINEGUN;
					SendToUnitCon();
				}
				break;
			}
		}


	}

	void SendToUnitCon()
	{
		uCon.ToggleUnitSpawningMode(true);
		uCon.cost = cost;
		uCon.spawningUnitType = unit;
	}


}
