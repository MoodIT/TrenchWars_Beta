using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class EnemyStats : MonoBehaviour {
//	[HideInInspector]
	public float health;
	private float startHealth;
	private GameObject healthBar;
	private Slider slider;
	[HideInInspector]
	public bool dead;
	private BlockControl bControl;
	private EnemyMovement enmMovement;
	private CommandPointsControl cPContol;

	private Animator animator;
	private EnemySpawning enmSpawn;

	void Start()
	{
		//inst enemyhealth
		enmSpawn = GameController.instance.enemySpawner;
		animator = GetComponentInChildren<Animator> ();
		enmMovement = GetComponent<EnemyMovement> ();
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		healthBar = Instantiate (GameController.instance.gameSettings.enemyHealthUI, Vector3.zero, Quaternion.identity) as GameObject;
		healthBar.transform.SetParent (GameController.instance.uiCanvas.transform);
		slider = healthBar.GetComponent<Slider> ();
		cPContol = GameController.instance.gameStats.gameObject.GetComponent<CommandPointsControl> ();	
		startHealth = health; 
	}
	void LateUpdate()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
		healthBar.transform.position = new Vector3(screenPos.x, screenPos.y+25f, 0);
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
		enmSpawn.enemiesInGame.Remove(this.gameObject);
		cPContol.PointsFlyUp(this.transform.position, 5);
		animator.SetBool("Melee",false);
		animator.SetBool ("Die", true);
		animator.SetFloat ("DeathFloat", Random.Range(0,1));
		animator.SetBool ("Shoot", false);
		enmMovement.t.Kill ();
//		GetComponent<Rigidbody> ().useGravity = true;
//		GetComponent<Collider> ().enabled = false;
		dead = true;
		GetComponent<Collider>().enabled = false;
		bControl.enemyOccupiedBlocksTop.Remove (enmMovement.currentBlock); 
		yield return new WaitForSeconds (3f);
		healthBar.SetActive (false);

				GetComponent<Rigidbody> ().useGravity = true;
				GetComponent<Collider> ().enabled = false;
		yield return new WaitForSeconds (0.5f);

        
        Destroy (this.gameObject);

//		GetComponent<Rigidbody> ().isKinematic = true;
	}
}
