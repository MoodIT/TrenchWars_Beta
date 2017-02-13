using UnityEngine;
using System.Collections; 
using UnityEngine.UI;
using DG.Tweening;
public class UnitStats : MonoBehaviour {
//	[HideInInspector]
	public float health;
		[HideInInspector]

	public float startHealth = 10;
//	[HideInInspector]
	public Block currentBlock;
//	private UnitMovement u
	private GameObject healthBar;
	private Slider slider;
	[HideInInspector]
	public bool dead;

	public BlockControl bControl;
	private Animator animator;
	private UnitMovement unitMove;
	private CommandPointsControl cPControl;
	private UnitController uCon;
	void Start()
	{
		uCon = GameController.instance.unitControls;
		cPControl = GameController.instance.gameStatsObj.GetComponent<CommandPointsControl>();
		unitMove = GetComponent<UnitMovement> ();
		animator = GetComponentInChildren<Animator> ();
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		//inst enemyhealth
		healthBar = Instantiate (GameController.instance.gameSettings.unitHealthUI, Vector3.zero, Quaternion.identity) as GameObject;
		healthBar.transform.SetParent (GameController.instance.uiCanvas.transform);
		slider = healthBar.GetComponent<Slider> ();
		
		startHealth = health;
	}
	void LateUpdate()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
		healthBar.transform.position = new Vector3(screenPos.x, screenPos.y+24.5f, 0);
		slider.value = health/startHealth;
		
	}
	void Update()
	{
		if(health <= 0 && !dead)
		{
			StartCoroutine(Die());
		}
	}
	
	IEnumerator Die()
	{
//		animator.SetBool ("Die", true);
//		animator.SetBool ("Shoot", false);
//
//		GetComponent<Rigidbody> ().useGravity = true;
//		GetComponent<Collider> ().enabled = false;
//		dead = true;
//		yield return new WaitForSeconds (0.5f);
//		healthBar.SetActive (false);
//		bControl.unitOccupiedBlocks.Remove (currentBlock);
//		bControl.unitsOnBlocks.Remove (this.gameObject);
//
//
//		Destroy (this.gameObject);
		uCon.UpdateUnitNumber(GetComponent<UnitShooting>().weaponType, false);
//		switch(GetComponent<UnitShooting>().weaponType)
//		{
//			case UnitShooting.WeaponType.FLAMER:
//			GameController.instance.unitControls.flamerAmt --;
//
//			break;
//			case UnitShooting.WeaponType.GRENADER:
//			GameController.instance.unitControls.grenaderAmt --;
//			break;
//			case UnitShooting.WeaponType.MACHINEGUN:
//			GameController.instance.unitControls.gunnerAmt --;
//			break;
//		}
//		Debug.Log ("DIE1");
		cPControl.UpdateCommandPoints();
		animator.SetBool("Melee",false);
		animator.SetBool ("InPosition", false);
		animator.SetBool ("Shoot", false);
        animator.SetFloat ("DeathFloat", Random.Range(0.1f,0.9f));
		animator.SetBool ("Die", true);


//		enmMovement.t.Kill ();
		unitMove.t.Kill ();
//		GetComponent<Rigidbody> ().useGravity = true;
//		GetComponent<Collider> ().enabled = false;
		dead = true;
		bControl.unitOccupiedBlocks.Remove (unitMove.currentBlock);
		bControl.unitOccupiedBlocks.Remove (currentBlock);

		bControl.unitsOnBlocks.Remove (this.gameObject);
		yield return new WaitForSeconds (3f);
		bControl.UpdateOccupiedBlocks ();
		healthBar.SetActive (false);

				GetComponent<Rigidbody> ().useGravity = true;
				GetComponent<Collider> ().enabled = false;
		yield return new WaitForSeconds (0.5f);


		Destroy (this.gameObject);
		//		GetComponent<Rigidbody> ().isKinematic = true;
	}
}
