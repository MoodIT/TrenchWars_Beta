using UnityEngine;
using System.Collections;

public class CommandPoint : MonoBehaviour {

	private GameStats gStats;
	private CommandPointsControl cPControl;
	void Start () {

		gStats = GameController.instance.gameStats;
		cPControl =	gStats.gameObject.GetComponent<CommandPointsControl> ();	
	}


	void OnMouseDown()
	{
		Debug.Log ("Picked up command points");
//		gStats.commandPoints += GameController.instance.gameSettings.commandPointPickup;
//		GameController.instance.uiControls.commandPointTextC.text = GameController.instance.gameSettings.commandPointUIText + gStats.commandPoints;
//		cPControl.UpdateCommandPoints();
		cPControl.PointsFlyUp(this.gameObject.transform.position, GameController.instance.gameSettings.commandPointPickup);
		Destroy (this.gameObject);
	}

	void OnCollisionEnter(Collision col)
	{
		Destroy(this.gameObject, 5);
	}
}
