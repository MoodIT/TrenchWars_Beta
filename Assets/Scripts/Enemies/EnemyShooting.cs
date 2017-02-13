using UnityEngine;
using System.Collections;
using DG.Tweening;
public class EnemyShooting : MonoBehaviour {
	public enum WeaponType{GRENADE, MACHINEGUN, FLAMETHROWER};
	public WeaponType weaponType;

	[HideInInspector]
	public bool shooting;
	public float shotCooldown = 0.8f;

	public float shootDistance = 5;
	[HideInInspector]
	public Vector3 targetPos;

	private EnemyMovement enmMovement;
	private BlockControl bControl;
	private UnitController uCon;
	private EnemyStats enmStats;
	private EnemyMelee enmMelee;
	private bool startShoot;
	public bool mayShoot;
	public bool burstFire;
	public float timeBetweenBursts = 2;
	public float burstAmt;
	public float burstDuration;
	public float timeBetweenShots = 0.5f;

	public int shootRangeOnTop = 8;
	public float stopTimeBetweenShots = 0.4f;
	public bool shootWhenMoving;
	public bool moveWhenShooting;
	public bool slowMoveWhenShooting;
	public float timeBetweenShotsWhenMoving = 3f; 

	private Animator animator;
	public bool pausedMove;
	void Start () 
	{
		uCon = GameController.instance.unitControls;
		animator = GetComponentInChildren < Animator> ();
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		enmMovement = GetComponent<EnemyMovement> ();
		enmStats = GetComponent<EnemyStats>();
		enmMelee = GetComponent<EnemyMelee>();
//		StartCoroutine (LookForUnitsInTrench ());
		StartCoroutine(CheckIfUnitInFront());
		if(weaponType == WeaponType.GRENADE)
		{
			animator.SetBool("Grenader", true);
		}
	}

	void Update()
	{
		if(!startShoot && mayShoot && !enmStats.dead)
		{
			startShoot = true;
			//in position to shoot
			StartCoroutine(ShotInterval());
			transform.DOLookAt(new Vector3(targetPos.x, this.transform.position.y, targetPos.z), 0.1f);
//			StartCoroutine(MovementPause());
		}

		if((startShoot && !mayShoot) || enmStats.dead )
		{
			startShoot = false;
			StopCoroutine(ShotInterval());
			StopCoroutine(WaitBetweenBursts());
		}
	}

	IEnumerator WaitBetweenBursts()
	{
		yield return new WaitForSeconds(timeBetweenBursts);
		if(mayShoot)
			StartCoroutine(ShotInterval ());
		else
		{
			yield break;
		}
	}
	IEnumerator MovementPause()
	{
		enmMovement.t.Pause();
		pausedMove = true;
		animator.SetBool ("Shoot", true);
		yield return new WaitForSeconds(stopTimeBetweenShots);
		animator.SetBool ("Shoot", false);
		enmMovement.t.Play();
		pausedMove = false;
//		yield return new WaitForSeconds(timeBetweenShotsWhenMoving- stopTimeBetweenShots);
//		StartCoroutine(MovementPause());
//		yield break;
	}


	IEnumerator ShotInterval()
	{
		bool on = true;
		float mTime = 0;
		int shotCount = 0;
		bool shot = false;
		bool stopMove = false;
		while(true)
		{
			mTime += Time.deltaTime;
			yield return null;
			if(mayShoot)
			{
				if(!shot)
				{
					if(slowMoveWhenShooting)
						enmMovement.ToggleSpeed(0.5f);

					if(!pausedMove && !moveWhenShooting)
						StartCoroutine(MovementPause());
					Shoot ();
					shotCount++;
					shot = true;
				}
//				if(mTime <= 0)
//				{
//					Shoot ();
//					shotCount ++;
//				}
				if(mTime > timeBetweenShotsWhenMoving)
				{
//					if(!moveWhenShooting)
//					{
//						enmMovement.t.Pause();
//					}
					if(!pausedMove && !moveWhenShooting)
						StartCoroutine(MovementPause());


					Shoot ();
					shotCount ++;
					mTime = 0;
				}

			}

			else if(!mayShoot)
			{
				on = false;
				if(slowMoveWhenShooting)
					enmMovement.ToggleSpeed(1f);
				yield break;
			}
			else if(enmStats.dead)
			{
				mayShoot = false;
				on = false;
				yield break;
			}

			if(shotCount != 0 && shotCount >= burstAmt && burstFire)
			{
				if(slowMoveWhenShooting)
					enmMovement.ToggleSpeed(1f);
				on = false;
				StartCoroutine(WaitBetweenBursts());
//				if(!moveWhenShooting)
//				{
//					enmMovement.t.Pause();
//				}
				yield break;
			}
//			if(burstDuration != 0 && burstFire && )
//			{
//				on = false;
//				StartCoroutine(WaitBetweenBursts());
//				yield break;
//			}
			
			
			
			//			yield return new WaitForSeconds(timeBetweenShots);
//			if(mTime >= 0)
//			{
//				if(!moveWhenShooting)
//													{
//														enmMovement.t.Pause();
//														yield return new WaitForSeconds(stopTimeBetweenShots);
//														animator.SetBool ("Shoot", false);
//														enmMovement.t.Play ();
//													}
//			}
			
		}
	}


//	public void StartShooting(Transform tPos)
//	{
//		if(!shooting)
//		{
//			targetPos = tPos;
//			shooting = true;
//			StartCoroutine(ShootCooldown());
//		}
//	}
//	public void StopShooting()
//	{
//		if(shooting)
//		{
//			shooting = false;
//		}
//	}

//	IEnumerator ShootCooldown ()
//	{
//		bool on = true;
//		float mTime = 0;
//		while(on)
//		{
//			if(shooting)
//			{
//				if(mTime < shotCooldown)
//				{
//					mTime += Time.deltaTime;
//
//				}
//				else
//				{
//					mTime = 0;
//					Shoot();
//				}
//			}
//			else
//			{
//				yield break; 
//			}
//
//			yield return null;
//		}
//	}

	public void Shoot()
	{
		if(!enmStats.dead)
		{
			if(weaponType == WeaponType.GRENADE)
			{
				if(targetPos != null)
				{
					GameObject obj = Instantiate(GameController.instance.gameSettings.enemyGrenade, this.transform.position + new Vector3(-0.3f, 0, 0), GameController.instance.gameSettings.enemyGrenade.transform.rotation) as GameObject;
					obj.GetComponent<GrenadeBehaviour>().shotBy = "Enemy";
					obj.GetComponent<GrenadeBehaviour>().target = targetPos;
//					Debug.Log (targetPos);
				}
			}

			if(weaponType == WeaponType.MACHINEGUN)
			{
				if(targetPos != null)
				{
					GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.shotSound, true, 0.3f, false);
					GameObject obj = Instantiate(GameController.instance.gameSettings.enemyGunShot, this.transform.position + new Vector3(-0.3f, 0, 0), GameController.instance.gameSettings.enemyGunShot.transform.rotation) as GameObject;
					obj.GetComponent<ShotBehaviour>().shotBy = "Enemy";
	//				obj.GetComponent<ShotBehaviour>().target = targetPos;
					obj.GetComponent<ShotBehaviour>().direction = (targetPos - (this.transform.position+ new Vector3(0,0.7f,0))).normalized;
				}
			}

			if(weaponType == WeaponType.FLAMETHROWER)
			{
				if(targetPos != null)
				{
//					Debug.Log ("BGUEI");
					GameObject obj = Instantiate(GameController.instance.gameSettings.enemyFlameBullet, this.transform.position + new Vector3(-0.3f, 0, 0), GameController.instance.gameSettings.enemyGunShot.transform.rotation) as GameObject;
					obj.GetComponent<ShotBehaviour>().shotBy = "Enemy";
					//				obj.GetComponent<ShotBehaviour>().target = targetPos;
					obj.GetComponent<ShotBehaviour>().direction = (targetPos - (this.transform.position+ new Vector3(0,0.7f,0))).normalized;
				}
			}
		}
	}

	IEnumerator LookForUnitsInTrench() // move to shooting
	{
		while(!enmStats.dead)
		{
			if(enmMovement.inPlayerTrench)
			{
				for(int i = 0; i < bControl.interactableBlockStates.GetLength(0); i++)
				{
					for(int i2 = 0; i2 < bControl.interactableBlockStates.GetLength(1); i2++)
					{
						if(i == enmMovement.currentBlock.width || i2 == enmMovement.currentBlock.length) //iterate through blocks on current lanes
						{
							foreach(Block b in bControl.unitOccupiedBlocks)
							{
								if(b.width == enmMovement.currentBlock.width || b.length == enmMovement.currentBlock.length)
								{
									bool visCheck = VisibilityCheck(b, enmMovement.currentBlock);

									if(visCheck)
									{								
										if(enmMovement.t.IsPlaying())
										{
											if(enmMovement.transform.position.x <= enmMovement.currentBlock.width + 0.1f && enmMovement.transform.position.x >= enmMovement.currentBlock.width - 0.1f)
											{
												if(enmMovement.transform.position.z <= enmMovement.currentBlock.length + 0.1f && enmMovement.transform.position.z >= enmMovement.currentBlock.length - 0.1f)
												{
													//IMPLEMENT A RANGE !!!
													animator.SetBool ("Shoot", true);
													mayShoot = true;
													targetPos = bControl.interactableBlockObjs[b.width, b.length].transform.position;
													enmMovement.t.Pause();
												}
											}

										}
									}
									//									Debug.Log("Same lane width : " + currentBlock.width);
								}
								else
								{
//									StopShooting();
									mayShoot = false;
									animator.SetBool ("Shoot", false);

									if(!enmMovement.t.IsPlaying())
									{
										enmMovement.t.Play(); 
									}
								}
								//								if(b.length == currentBlock.length)
								//								{
								//									if(t.IsPlaying())
								//									{
								//										t.Pause();
								//									}
								////									Debug.Log("Same lane length : " + currentBlock.length);
								//								}
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.3f);
		}
		
	}

	bool VisibilityCheck(Block occupiedBlock, Block curB)
	{
		bool vC = true;
		if(occupiedBlock.width >= curB.width && occupiedBlock.length >= curB.length)
		{
//			Debug.Log ("BNEU1");
			for(int i = curB.width; i <= occupiedBlock.width; i++)
			{
				for(int i2 = curB.length; i2<= occupiedBlock.length; i2++)
				{
					if(bControl.interactableBlockStates[i,i2] != 0)
					{
						vC = false;
						return vC;
					}
				}
			}
		}
		else if(occupiedBlock.width <= curB.width && occupiedBlock.length <= curB.length)
		{
//			Debug.Log ("BNEU2");
			for(int i = occupiedBlock.width; i <= curB.width; i++)
			{
				for(int i2 = occupiedBlock.length; i2<= curB.length; i2++)
				{
					if(bControl.interactableBlockStates[i,i2] != 0)
					{
						vC = false;
						return vC;
					}
				}
			}
		}
		else if(occupiedBlock.width >= curB.width && occupiedBlock.length <= curB.length)
		{
//			Debug.Log ("BNEU3");
			for(int i = curB.width; i <= occupiedBlock.width; i++)
			{
				for(int i2 = occupiedBlock.length; i2<= curB.length; i2++)
				{
					if(bControl.interactableBlockStates[i,i2] != 0)
					{
						vC = false;
						return vC;
					}
				}
			}
		}
		else 
		{
//			Debug.Log ("BNEU4");
			for(int i = occupiedBlock.width; i <= curB.width; i++)
			{
				for(int i2 = curB.length; i2<= occupiedBlock.length; i2++)
				{
					if(bControl.interactableBlockStates[i,i2] != 0)
					{
						vC = false;
						return vC;
					}
				}
			}
		}
		return vC;
	}

	IEnumerator CheckIfUnitInFront()
	{
		int s = 0;
		while(true)
		{			
			if(!enmMelee.fighting && !enmMovement.inPlayerTrench)
			{
			for(int i = 0; i < bControl.interactableBlocks1D.Length; i++)
			{
				if(bControl.interactableBlocks1D[i].mLength != enmMovement.currentBlock.mLength) continue;
//				if(bControl.interactableBlocks1D[i].mLength == enmMovement.currentBlock.mLength)
//				{
//					if(bControl.interactableBlockStates1D[i] == 4) s = bControl.interactableBlocks1D[i].mWidth;
//				}
				for(int i2 = 0; i2< bControl.unitOccupiedBlocks.Count; i2++)
				{

					if(bControl.unitOccupiedBlocks[i2].mLength == bControl.interactableBlocks1D[i].mLength)
					{
						if(bControl.interactableBlockObjs1D[i] != null)
						{
//							Debug.Log (bControl.unitOccupiedBlocks[i2].mWidth + " " +  enmMovement.currentBlock.mWidth);
							if((enmMovement.currentBlock.mWidth - bControl.unitOccupiedBlocks[i2].mWidth) < shootRangeOnTop)
							{
//								if(s <  bControl.unitOccupiedBlocks[i2].mWidth || s > enmMovement.currentBlock.mWidth)
//								{
//								Debug.Log (i);
//								targetPos = bControl.interactableBlockObjs1D[i].transform.position;
//								targetPos = new Vector3(bControl.unitOccupiedBlocks[i2];
//								targetPos= new Vector3(targetPos.x, 1.2f, targetPos.z);
								targetPos = new Vector3(bControl.unitOccupiedBlocks[i2].mWidth, 1.2f, bControl.unitOccupiedBlocks[i2].mLength);
//								Debug.Log (bControl.interactableBlockObjs1D[i].transform.position.y);
								//						targetPos.position = new Vector3(bControl.unitOccupiedBlocks[i2].mWidth, 0, bControl.unitOccupiedBlocks[i2].mLength);
//								Shoot();
//								if(!startShoot)
//								{
//								if(!mayShoot)
									
//									startShoot = true;
								
								//			Debug.Log ("GUE");
								//			Debug.DrawRay (this.transform.position, new Vector3(1,0,0), Color.red);
								//			this.transform.rotation = Quaternion.Euler(new Vector3(1,0,0));
								//in position to shoot

//									StartCoroutine(ShotInterval());
//								}
//								enmMovement.t.Pause();
//								}

//								if(!moveWhenShooting)
//								{
//									enmMovement.t.Pause();
//									yield return new WaitForSeconds(stopTimeBetweenShots);
//									animator.SetBool ("Shoot", false);
//									enmMovement.t.Play ();
//								}
								mayShoot = true;

							}
								else
								{
									mayShoot = false;

								}
							}
							else
								{
									mayShoot = false;
								}
//						yield return new WaitForSeconds(timeBetweenShotsWhenMoving-0.5f);
//						enmMovement.t.Play ();
					}
						else
						{
//							mayShoot = false;
						}
				}
			}
			}
			else
			{
				mayShoot = false;
			}
			yield return new WaitForSeconds(0.2f);
		}
	}
}
