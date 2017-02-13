using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class UnitShooting : MonoBehaviour {
	public enum WeaponType{MACHINEGUN, FLAMER, GRENADER};
	public WeaponType weaponType;

	private UnitMovement movementC;
	private UnitMelee unitMelee;
	private BlockControl bControl;
	private UnitStats unitStats;
	private AudioController audioCon;

	public GameObject shotObject;
	private bool lookingAtDirection;
	public Vector3 lookDir;
	public Vector3 lookDirSnapped;
	public float timeBetweenShots =0.5f;

	[Header("Burst Shot")]
	public bool burstFire;
	public float timeBetweenBursts = 2;
	public float burstAmt  = 5;

	private Transform targetPos;
	private bool startShoot;

	public bool mayShoot ;
	public bool usingRangeParams;
	public int[] ranges = new int[2]{3,2};
	private bool shooting;	
	private AudioSource flamerAudio;
	bool enmInTrench = false;
	public ShootingCollider[] shootingColliders;
	public List<GameObject> enemiesInSight;

	public List<GameObject> removeFromSight;
	private Animator animator;
	void Start () 
	{
		animator = GetComponentInChildren<Animator> ();
//		animator.SetBool ("Shoot", true);
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		movementC = GetComponent<UnitMovement> (); 
//		shootingColliders
		shootingColliders = gameObject.GetComponentsInChildren<ShootingCollider>();
//		foreach(ShootingCollider sC 
//		StartCoroutine (LookForEnemiesWithinRange ());
		StartCoroutine (CheckForEnemies ());
		unitStats = GetComponent<UnitStats>();
		unitMelee = GetComponent<UnitMelee>();
		audioCon = GameController.instance.audioController;
	}
	

	void Update () 
	{
		if(movementC.inPosition && !startShoot && mayShoot)
		{
			startShoot = true;

//			Debug.Log ("GUE");
//			Debug.DrawRay (this.transform.position, new Vector3(1,0,0), Color.red);
//			this.transform.rotation = Quaternion.Euler(new Vector3(1,0,0));
			//in position to shoot
			StartCoroutine(ShotInterval());
		}

		if (!movementC.inPosition)
			mayShoot = false;

		if(startShoot && !mayShoot)
		{
//			Debug.Log ("GUEPPP");
			startShoot = false;
			StopCoroutine(ShotInterval());
			StopCoroutine(WaitBetweenBursts());
		}
	}
	IEnumerator CheckForEnemies()
	{
		while(true)
		{
			if(unitMelee != null && !unitMelee.fighting)
			{
				if(enemiesInSight.Count > 0 && FindClosestEnemy() != null)
				{
					mayShoot = true;

					targetPos = FindClosestEnemy();
				}
				else
				{
					animator.SetBool("Shoot", false);
					mayShoot = false;
				}

				foreach(GameObject g in enemiesInSight)
				{
					if(g != null)
					{
						if(g.GetComponent<EnemyStats>().dead)
						{

								removeFromSight.Add(g);
						}
					}
				}

				if(removeFromSight.Count > 0)
				{
					foreach(GameObject g2 in removeFromSight)
					{
						if(g2 != null)
						
						enemiesInSight.Remove(g2);
					}
				}
//			foreach(ShootingCollider sC in shootingColliders)
//			{
//				foreach(GameObject enm in sC.enemiesInRange)
//				{
//					if(enm != null)
//					{
//						if(!enemiesInSight.Contains(enm))
//						{
//							enemiesInSight.Add(enm);
//						}
//						
////					
////						mayShoot = true;
////
//////						targetPos = 
//					}
//				}
//			}
			}
			yield return new WaitForSeconds(0.3f);
		}
	}

	Transform FindClosestEnemy() { 
		// Find all game objects with tag Enemy 
//		GameObject[] gos;
//		gos = GameObject.FindGameObjectsWithTag("Enemy"); 

		GameObject closest = null;
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		// Iterate through them and find the closest one 
		foreach (GameObject go in enemiesInSight) 
		{ 
			if(go != null)
			{
				Vector3 diff = (go.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					closest = go; 
					distance = curDistance; 
				} 
			}
		} 
		if(closest != null)
		{
			return closest.transform; 
		}
		return null;
	}

	IEnumerator WaitBetweenBursts()
	{
		yield return new WaitForSeconds(timeBetweenBursts);
		if(movementC.inPosition && mayShoot)
			StartCoroutine(ShotInterval ());
		else
		{
			yield break;
		}
	}
	IEnumerator ShotInterval()
	{
		bool on = true;
		float mTime = 0;
		int shotCount = 0;
		while(true)
		{
			mTime += Time.deltaTime;
			yield return null;
			if(movementC.inPosition && mayShoot && !unitStats.dead)
			{
//				if(!shooting)
//					shooting = true;

				if(mTime > timeBetweenShots)
				{

					Shoot ();
					mTime = 0;
					shotCount ++;
				}
			}
			else
			{
				if(shooting)
				{
					shooting = false;
					if(flamerAudio != null)
					{
						audioCon.DestroyAudio(flamerAudio);
//						flamerAudio.Stop();
					}
				}

                on = false;
				yield break;
			}
			if(shotCount >= burstAmt && burstFire)
			{
				on = false;
				StartCoroutine(WaitBetweenBursts());
				yield break;
			}



//			yield return new WaitForSeconds(timeBetweenShots);


		}
	}

	void Shoot()
	{
		if(weaponType == WeaponType.MACHINEGUN && movementC.inPosition)
		{
			if(targetPos != null)
			{
				GameObject shot = Instantiate(shotObject, this.transform.position + new Vector3(0,0.7f,0), shotObject.transform.rotation) as GameObject;
//			if(enmInTrench)
//			{
//				shot.GetComponent<ShotBehaviour>().direction = (new Vector3(targetPos.position.x, targetPos.position.y -0.5f, targetPos.position.z) - this.transform.position).normalized;
//			}
//			else
//			{
				StartCoroutine(ShootAnim(weaponType, 0.5f));
//				animator.SetBool("Shoot", true);
//				animator.SetFloat("ShootFlame", 0);
				shot.GetComponent<ShotBehaviour>().direction = (targetPos.position - (this.transform.position+ new Vector3(0,0.7f,0))).normalized;
				shot.GetComponent<ShotBehaviour>().shotBy = "Unit";
				audioCon.StartPlay(GameController.instance.gameSettings.shotSound, true, 0.3f);
			}
//			Debug.Log((targetPos.position - this.transform.position).normalized);
//			Debug.Log (targetPos.position - this.transform.position).normalized);

//			}
		}
		if(weaponType == WeaponType.FLAMER && movementC.inPosition)
		{
			if(targetPos != null)
			{
				if(!shooting)
				{
					animator.SetBool("Shoot", true);
					shooting = true;
					flamerAudio = GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.flamerSound, true, 0.2f, true);
					animator.SetFloat("ShootGrenadeFlame", 1);
				}
				GameObject shot = Instantiate(shotObject, this.transform.position + new Vector3(0,0.7f,0), shotObject.transform.rotation) as GameObject;

				shot.GetComponent<ShotBehaviour>().direction = (targetPos.position - (this.transform.position+ new Vector3(0,0.7f,0))).normalized;
				shot.GetComponent<ShotBehaviour>().shotBy = "Unit";
			}
		}
		if(weaponType == WeaponType.GRENADER && movementC.inPosition)
		{
			if(targetPos != null)
			{
				StartCoroutine(ShootAnim(weaponType, 0.2f));
				GameObject shot = Instantiate(shotObject, this.transform.position + new Vector3(0,0.7f,0), shotObject.transform.rotation) as GameObject;
//				animator.SetBool("Shoot", true);
//				animator.SetFloat("ShootFlame", 0);
//				shot.GetComponent<GrenadeBehaviour>().direction = (targetPos.position - (this.transform.position+ new Vector3(0,0.7f,0))).normalized;
				shot.GetComponent<GrenadeBehaviour>().shotBy = "Unit";
				shot.GetComponent<GrenadeBehaviour>().target = targetPos.position;

			}
		}
	}

	IEnumerator ShootAnim(WeaponType type, float dur)
	{
		animator.SetBool("Shoot", true);
		if(type == WeaponType.GRENADER)
		{
			animator.SetFloat("ShootGrenadeFlame", 0.5f);
		}
		else if(type == WeaponType.MACHINEGUN)
		{
			animator.SetFloat("ShootGrenadeFlame", 0);
		}
		yield return new WaitForSeconds(dur);

		animator.SetBool("Shoot", false);
	}




}
