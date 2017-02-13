using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class UnitShooting_legacy : MonoBehaviour {
	public enum WeaponType{MACHINEGUN, FLAMETHROWER};
	public WeaponType weaponType;

	private UnitMovement movementC;
	private BlockControl bControl;
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

	private bool mayShoot ;
	public bool usingRangeParams;
	public int[] ranges = new int[2]{3,2};

	bool enmInTrench = false;
	public ShootingCollider[] shootingColliders;
	void Start () 
	{
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		movementC = GetComponent<UnitMovement> (); 
//		shootingColliders
		shootingColliders = gameObject.GetComponentsInChildren<ShootingCollider>();
//		foreach(ShootingCollider sC 
//		StartCoroutine (LookForEnemiesWithinRange ());
	}
	

	void Update () 
	{
		if(movementC.inPosition && !startShoot && mayShoot)
		{
			startShoot = true;
			Debug.DrawRay (this.transform.position, new Vector3(1,0,0), Color.red);
			this.transform.rotation = Quaternion.Euler(new Vector3(1,0,0));
			//in position to shoot
			StartCoroutine(ShotInterval());
		}
		if(!movementC.inPosition && startShoot)
		{
			startShoot = false;
			StopCoroutine(ShotInterval());
			StopCoroutine(WaitBetweenBursts());
		}
	}
	Transform FindClosestEnemy() { 
		// Find all game objects with tag Enemy 
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Enemy"); 
		GameObject closest = null;
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		// Iterate through them and find the closest one 
		foreach (GameObject go in gos) 
		{ 
			var diff = (go.transform.position - position); 
			var curDistance = diff.sqrMagnitude; 
			if (curDistance < distance) { 
				closest = go; 
				distance = curDistance; 
			} 
		} 
		return closest.transform; 
	}
	IEnumerator LookForEnemiesWithinRange()
	{
		float dist = Mathf.Infinity;
		Vector3 pos = Vector3.zero;
		Vector3 distanceCheck = Vector3.zero;   
		float currentDistance = Mathf.Infinity;
//		Vector3 distanceCheck = currentObject.transform.position - position;    
//		float currentDistance = distanceCheck.sqrMagnitude;
		while(true)
		{
			if(movementC.inPosition)
			{


				if(EnemyWithinRange(1, new int[2]{10,5} ,1) != null)
				{
//					if(Vector3.Distance(this.transform.position, EnemyWithinRange(1, new int[2]{10,5} ,1)
//					Debug.Log(EnemyWithinRange(1, new int[2]{10,5} ,1).position);
//					Debug.Log ("UNITS INSIDE");
					mayShoot = true;
					targetPos = EnemyWithinRange(1, new int[2]{10,5} ,1); 
				}
				if(EnemyWithinRange(1, new int[2]{10,5} ,2) != null) // && EnemyWithinRange(1, new int[2]{10,5} ,3) == null
				{
//					Debug.Log ("UNITS INSIDE");

					mayShoot = true;
//					Transform t2= EnemyWithinRange(1, new int[2]{10,5} ,2);
//					Transform t3 = EnemyWithinRange(1, new int[2]{10,5} ,3);
//					if(Vector3.Distance(this.transform.position, t2.position) < Vector3.Distance(this.transform.position, t3.position))
//					{
//					Debug.Log("GBNEUG");
//						targetPos = EnemyWithinRange(1, new int[2]{10,5} ,2); 
//					}
//					else
//					{
//					if(!startShoot)
//					{
						targetPos = EnemyWithinRange(1, new int[2]{10,5} ,2); 
//						Debug.Log (targetPos.position);

//					}
//					}
					//find closest
				}
				if(EnemyWithinRange(1, new int[2]{10,5} ,3) != null)
				{
					mayShoot = true;
					targetPos = EnemyWithinRange(1, new int[2]{10,5} ,3); 
				}
//				else if( EnemyWithinRange(1, new int[2]{10,5} ,3) != null && EnemyWithinRange(1, new int[2]{10,5} ,2) != null)
//				{
//					mayShoot = true;
//					Transform t2= EnemyWithinRange(1, new int[2]{10,5} ,2);
//					Transform t3 = EnemyWithinRange(1, new int[2]{10,5} ,3);
//					if(Vector3.Distance(this.transform.position, t2.position) < Vector3.Distance(this.transform.position, t3.position))
//					{
//						targetPos = EnemyWithinRange(1, new int[2]{10,5} ,2); 
//					}
//					else
//					{
//						targetPos = EnemyWithinRange(1, new int[2]{10,5} ,3); 
//					}
//				}
				if(EnemyWithinRange(1, new int[2]{10,5} ,3) == null && EnemyWithinRange(1, new int[2]{10,5} ,2) == null && EnemyWithinRange(1, new int[2]{10,5} ,1) == null)
				{
//					Debug.Log ("UNITS INSIDE NULL");

					mayShoot = false;

				}
				//check forward either by map width or by ranges [0]
				//if ranges > 1 in length check forward +-1 by ranges[1]
//				foreach(Block b in bControl.enemyOccupiedBlocksTop) 
//				{
////					for(int i = movementC.currentBlock.width; i < movementC.currentBlock.width + ranges[0];i++)
////					{
//					if(b.length == movementC.currentBlock.length)
//					{
//						if(b.width > movementC.currentBlock.width)
//						{
//							if(usingRangeParams)
//							{	
//								if(b.width < movementC.currentBlock.width + ranges[0])
//								{
//									//start shooting
//									mayShoot = true;
//									targetPos = bControl.interactableBlockObjs[b.width,b.length].transform;
//									break;
//								}
//
//							}
//							else
//							{
//								mayShoot = true;
//								targetPos = bControl.interactableBlockObjs[b.width,b.length].transform;
//								break;
//								//start shooting
//							}
////								Debug.Log ("INSIDE RANGE");
//							this.transform.DOLookAt(bControl.interactableBlockObjs[b.width, b.length].transform.position, 0.01f);
//						}
//						if(ranges.Length > 1) //if there is a "side range"
//						{
//
//						}
//					}
//					else if((b.length == movementC.currentBlock.length+1 || b.length == movementC.currentBlock.length-1) && usingRangeParams)
//					{
//						if(ranges.Length > 1) //if there is a "side range"
//						{
//							if(b.width < movementC.currentBlock.width + ranges[1])
//							{
//																mayShoot = true;
//								targetPos = bControl.interactableBlockObjs[b.width,b.length].transform;
//								//								if(!shooting)
//								//								{
////								StartShooting(bControl.interactableBlockObjs[b.width, b.length].transform);
//								//								}
//								this.transform.DOLookAt(bControl.interactableBlockObjs[b.width, b.length].transform.position, 0.01f);
//																break;
//							}
//							
//						}
//					}
//					else
//					{
//						mayShoot = false;
//					}
//
////					}
//				}



				//if enemy is besides or in front IN TRENCH check if they are inside ranges[0] /or if !usingRangeparams shoot immediately
			}
			else
			{
				targetPos = null;
			}
			yield return new WaitForSeconds(0.3f);
//			yield return null;
		}
	}

	Transform EnemyWithinRange(int width, int[] ranges, int dir)//dir 1 = front, 2, right, 3 left, 4, back  //List<int> dirs // ranges[0] = front [1] =sides etc
	{
//		for(int i = 0; i < dirs.Count; i++)
//		{
		Transform t = null;
		switch(dir)
		{
		case 1:
			for(int i2 = -width; i2<= width; i2++)
		    {
//					ranges[i2] == range to check
				foreach(Block b in bControl.enemyOccupiedBlocksTop)
				{
//					Debug.Log(b.length + " " + i2 + " " + b.length +i2);
					if(b.length == movementC.currentBlock.length + i2)
					{
						if(b.width < movementC.currentBlock.width + ranges[Mathf.Abs(i2)])
						{


							if(b.length == movementC.currentBlock.length)
							{
								Debug.Log(b.length + " " + i2);
//								return bControl.interactableBlockObjs[b.width, b.length].transform;
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
								foreach(Block b2 in bControl.enemyOccupiedBlocksTrench)
								{
									if(b2.width == b.width && b2.length == b.length)
									{
										enmInTrench = true;
										break;
									}
									else
									{
										enmInTrench = false;
										break;
									}

								}
								break;
							}
							else 
							{
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
//								return bControl.interactableBlockObjs[b.width, b.length].transform;
							}
						}
					}
				}
			}
			break;

		case 2:
			for(int i2 = -width; i2< width; i2++)
			{
				foreach(Block b in bControl.enemyOccupiedBlocksTop)
				{
					if(b.width == movementC.currentBlock.width + i2)
					{
//						Debug.Log ("BNGUUENUU");
						if(b.length < movementC.currentBlock.length + ranges[Mathf.Abs(i2)])
						{ 
							if(b.width == movementC.currentBlock.width)
							{

//								Debug.Log("GBNEUG");
								//								return bControl.interactableBlockObjs[b.width, b.length].transform;
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
								foreach(Block b2 in bControl.enemyOccupiedBlocksTrench)
								{
									if(b2.width == b.width && b2.length == b.length)
									{
										enmInTrench = true;
										break;
										//										t.position = new Vector3(t.position.x, t.position.y-0.5f, t.position.z);
									}
									else
									{
										enmInTrench = false;
										break;
									}
								}
								break;
							}
							else 
							{
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
								//								return bControl.interactableBlockObjs[b.width, b.length].transform;
							}
						}
					}
				}
			}
			break;

		case 3:
			for(int i2 = -width; i2< width; i2++)
			{
				foreach(Block b in bControl.enemyOccupiedBlocksTop)
				{
					if(b.width == movementC.currentBlock.width + i2)
					{
						//						Debug.Log ("BNGUUENUU");
						if(b.length > movementC.currentBlock.length - ranges[Mathf.Abs(i2)])
						{ 
							if(b.width == movementC.currentBlock.width)
							{
								
								//								Debug.Log("GBNEUG");
								//								return bControl.interactableBlockObjs[b.width, b.length].transform;
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
								foreach(Block b2 in bControl.enemyOccupiedBlocksTrench)
								{
									if(b2.width == b.width && b2.length == b.length)
									{
										enmInTrench = true;
										break;
										//										t.position = new Vector3(t.position.x, t.position.y-0.5f, t.position.z);
									}
									else
									{
										enmInTrench = false;
										break;
									}
								}
								break;
							}
							else 
							{
								t = bControl.interactableBlockObjs[b.width, b.length].transform;
								//								return bControl.interactableBlockObjs[b.width, b.length].transform;
							}
						}
					}
				}
			}
			break;

		case 4:
			break;

		default:
			break;
		}
//		}
//		foreach(Block b in bControl.enemyOccupiedBlocksTop) 
//		{
//			if(b.length == movementC.currentBlock.length)
//			{
//				//if dirs !contain 4
//				if(b.width > movementC.currentBlock.width) //make sure not
//				{
//					return true;
//				}
//
//			}
//		}
//		if(!enmInTrench)
//		{
//			Debug.Log (t);
			return t;
//		}
//		else
//		{
//			Transform t2 = t;
//			t2.position = new Vector3(t.position.x, t.position.y -0.5f, t.position.z);
////			t.position = new Vector3(t.position.x, t.position.y -0.5f, t.position.z);
//			return t2;
//		}
	}
	IEnumerator Raycast()
	{
		RaycastHit hit;
		//		float distanceToGround = 0;
		
		while(true) //
		{
			if(!Physics.Raycast(transform.position + new Vector3(0,1,0), new Vector3(1,0,0), out hit, bControl.interactableBlockStates.GetLength(0)))
			{
				mayShoot = false;
				yield return new WaitForSeconds(0.5f);
			}
			else if(Physics.Raycast(transform.position + new Vector3(0,1,0), new Vector3(1,0,0), out hit,bControl.interactableBlockStates.GetLength(0)))
			{
				mayShoot = true;
				yield return new WaitForSeconds(1f);
			}
//			Debug.DrawRay(transform.position + new Vector3(0,1,0), new Vector3(1,0,0) * bControl.interactableBlockStates.GetLength(0), Color.red);

		}
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
		while(on)
		{
			mTime += Time.deltaTime;
			yield return null;
			if(movementC.inPosition && mayShoot)
			{
				if(mTime > timeBetweenShots)
				{
					Shoot ();
					mTime = 0;
					shotCount ++;
				}
			}
			else
			{
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

			GameObject shot = Instantiate(shotObject, this.transform.position + new Vector3(0,0.7f,0), Quaternion.identity) as GameObject;
			if(enmInTrench)
			{
				shot.GetComponent<ShotBehaviour>().direction = (new Vector3(targetPos.position.x, targetPos.position.y -0.5f, targetPos.position.z) - this.transform.position).normalized;
			}
			else
			{
				shot.GetComponent<ShotBehaviour>().direction = (targetPos.position - this.transform.position).normalized;

			}
		}
	}




}
