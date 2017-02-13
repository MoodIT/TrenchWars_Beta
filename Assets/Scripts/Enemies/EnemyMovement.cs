using UnityEngine;
using System.Collections;
using DG.Tweening;
public class EnemyMovement : MonoBehaviour {

	// Use this for initialization
	private EnemyStats enmStats;

	private EnemyShooting enmShooting;
	[HideInInspector]
	public BlockControl bControl;

	public float movementSpeed = 0.5f;
	public float movementSlowScale = 0.3f;
//	[HideInInspector]
	 public Block currentBlock;
	public Block startBlock;
    public Block startBlockGoTo;
	public Block nextBlock;
	public int pointOnPath;
	public Vector3[] currentPath;
	[HideInInspector]
	public float distToUnit = 10;
	public float stopDistToUnit = 5;

	Transform target;
	private bool unitInFront;

	public Tweener t = null;
	private bool moving;
	private bool idling;
	public bool inPosition;
	public int lastPointWidth;
	public int lastPointLength;

	public Block prevOccupiedBlock;

	private FindPath fPath;
	private OutsideTrenchPath outTrenchPath;
	public int width;
	public int length;
	public bool update;
	[HideInInspector]
	public bool inPlayerTrench = false;
	private bool jumpUpWhenEndMove;
	private bool inEnemyTrench;
	[HideInInspector]
	public bool movingToTrench;

//	[HideInInspector]
	public bool movingToEnd;

	private Animator animator;

	private GameStats gameStats;

	private GameObject uConObj;
	private Block jumpToBlock;
	private bool jumping;
	void Start () 
	{

		gameStats = GameController.instance.gameStats;
		animator = GetComponentInChildren<Animator> ();
		fPath = GameController.instance.unitControllerObj.GetComponent<FindPath>();
		enmStats = GetComponent<EnemyStats> ();
		enmShooting = GetComponent<EnemyShooting> ();
		uConObj = GameController.instance.unitControllerObj;
//		StartCoroutine (CheckDist ());
//		StartCoroutine (CheckWalkTo ());
		currentBlock = startBlock;
		StartCoroutine (CheckCurrentBlock ());
		StartCoroutine (CheckNextBlock ());
//		Debug.Log(uConObj.GetComponent<FindPath>());
//		inEnemyTrench = true;
		jumpUpWhenEndMove = true;
//		MoveOnPath(this.transform, uConObj.GetComponent<FindPath>().Patherize(startBlock, startBlockGoTo, new int[1]{0}, 0));

		outTrenchPath = fPath.gameObject.GetComponent<OutsideTrenchPath>();
		DoMove (FindPath (new int[2]{1,3},1, startBlock.length));
//		Debug.Log (FindClosest (startBlock.length, 4));
//		FindPath ();
//		CalculatePath ();
//		StartCoroutine (LookForUnitsInTrench ());
//		bControl.OnDigUpdate += UpdatePath ;
	}

	void OnEnable()
	{
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		bControl.OnDigUpdate += UpdatePath ;
		bControl.OnOccupiedBlocksUpdate += CheckJumpOrShoot ;
	}
	void OnDisable()
	{
		bControl.OnDigUpdate -= UpdatePath ;
		bControl.OnOccupiedBlocksUpdate -= CheckJumpOrShoot ;
	}
	// Update is called once per frame




	public void MoveOnPath(Transform trans, Vector3[] path)
	{
		//		if(!moving)
		//		{
		if(t != null)
		{
			if(t.IsPlaying())
			{
				t.Kill();
//				currentBlock = new Block{width = lastPointWidth, length = lastPointLength};
				currentBlock.width = lastPointWidth;
				currentBlock.length = lastPointLength;
			}
		}
		moving = true;
		inPosition = false;
		if (path != null) 
		{
//			if(prevOccupiedBlock != null)
//				bControl.unitOccupiedBlocks.Remove (prevOccupiedBlock); 
//			path[0] = this.transform.position;
			//find a way to smooth from current transform position to bath start!! 

			t = trans.DOPath (path, movementSpeed).SetEase (Ease.Linear).SetSpeedBased ().OnComplete(OnCompleteMove).SetLookAt (0.01f);	

			lastPointWidth = (int)path [path.Length-1].x;
			lastPointLength = (int)path [path.Length-1].z;
		}
	}
	public void DoMove(Vector3[] path)
	{
		currentPath = path;
		MoveOnPath(this.transform, path);
	}

//	void Update()
//	{
////		if(goSlow) t.timeScale = 0.2f;
//	}

	public void ToggleSpeed(float speedScale)
	{
		t.timeScale = speedScale;
	}
	void UpdatePath()
	{
		if(!inPlayerTrench && !jumping)
		{
			Vector3[] p = FindPath (new int[2]{1,3},1, currentBlock.length); //WHY DOES TI SOMETIME RETURN NULL????
			DoMove (p);
		}	
	}


	IEnumerator CheckNextBlock()
	{
		while(true)
		{
			if(t != null)
			{
				if(!inPlayerTrench && currentPath != null)
				{
					for(int i = 0; i< currentPath.Length; i++)
					{ 						
						if(currentBlock.width == currentPath[i].x && currentBlock.length == currentPath[i].z)
						{
							pointOnPath = i;
						}
					}

					if(CheckNextB() && !idling)
					{
						t.Pause();
						animator.SetBool("Idle", true);
						idling = true;
//						t.
					}
					else if(idling)
					{
						t.Play();
						animator.SetBool("Idle", false);
//						DoMove (FindPath (new int[1]{1},1, currentBlock.length));
					}


				}
//				if(currentPath[pointOnPath+1] 
			}
			yield return null;// new WaitForSeconds(0.1f);
		}
	}
	bool CheckNextB()
	{

		foreach(Block b in bControl.enemyOccupiedBlocksTop) 
		{
			if(pointOnPath < currentPath.Length-1)
			{
				if(b.width == currentPath[pointOnPath+1].x && b.length == currentPath[pointOnPath+1].z)
				{
					return true;
				}
			}
			//else hvis det er på sidste point??
			
		}
		return false;
	}

	void OnCompleteMove()
	{
		if(inPlayerTrench)
		{
			gameStats.EndGame ();
//			Debug.Log ("BHEU");
			
		}
		if(movingToEnd)
		{
			gameStats.EndGame ();
		}
		inPosition = true;
		CheckJumpOrShoot ();
//		if(jumpUpWhenEndMove && inTrench)
//		{
//
//		}
	}
	void CheckJumpOrShoot()
	{
		bool jump = false;

//		Debug.Log ("lalelelloooo");
        if(!inPlayerTrench && !inEnemyTrench && inPosition && !movingToEnd && !jumping)
		{
//			Debug.Log ("BNEU");
	//		Debug.Log (currentBlock.width + " " +  currentBlock.length);
			if(bControl.interactableBlockStates[lastPointWidth-1,lastPointLength] == 0 )
			{
//				Debug.Log(SameOccupied(lastPointWidth-1, lastPointLength) + " " + (lastPointWidth-1) + " " + lastPointLength);
//				Debug.Log(SameOccupied(lastPointWidth, lastPointLength-1));
//				Debug.Log ("LALAL");
				if(SameOccupied(lastPointWidth-1, lastPointLength))
				{
//										Debug.Log ("LALAL11");
                    StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[lastPointWidth-1,lastPointLength].transform.position, true));
					jumpToBlock = bControl.interactableBlocks[lastPointWidth-1,lastPointLength];
					jumping = true;
//					enmShooting.StopShooting();
					enmShooting.mayShoot = false;
				}
				else
				{
//					Debug.Log ("LALAL12");
					StartCoroutine(IntoTrenchCheck(bControl.interactableBlocks[lastPointWidth-1,lastPointLength]));
					enmShooting.mayShoot = true;
					enmShooting.targetPos = bControl.interactableBlockObjs[lastPointWidth-1, lastPointLength].transform.position;
					animator.SetBool("Shoot", true);

				}
//				enmShooting.StartShooting(bControl.interactableBlockObjs[lastPointWidth-1, lastPointLength].transform);

			}
			else if(bControl.interactableBlockStates[lastPointWidth,lastPointLength-1] == 0)
			{
//				Debug.Log ("LALAL222");
				if(SameOccupied(lastPointWidth, lastPointLength-1))
				{
//					Debug.Log ("LALAL222111");
					StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[lastPointWidth,lastPointLength-1].transform.position, true));
					jumpToBlock = bControl.interactableBlocks[lastPointWidth,lastPointLength-1];
//					enmShooting.StopShooting();
					enmShooting.mayShoot = false;
					jumping = true;
				}
				else
				{
//					Debug.Log ("LALAL222222");
//					enmShooting.StartShooting(bControl.interactableBlockObjs[lastPointWidth, lastPointLength-1].transform);
					StartCoroutine(IntoTrenchCheck(bControl.interactableBlocks[lastPointWidth,lastPointLength-1]));
					enmShooting.mayShoot = true;
					enmShooting.targetPos = bControl.interactableBlockObjs[lastPointWidth, lastPointLength-1].transform.position;
					animator.SetBool("Shoot", true);
				}
			}	
			else if(bControl.interactableBlockStates[lastPointWidth,lastPointLength+1] == 0)
			{
//				Debug.Log ("LALAL333");
				if(SameOccupied(lastPointWidth, lastPointLength+1))
				{
					StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[lastPointWidth,lastPointLength+1].transform.position, true));
					jumpToBlock = bControl.interactableBlocks[lastPointWidth,lastPointLength+1];
//					enmShooting.StopShooting();
					enmShooting.mayShoot = false;
					jumping = true;
				}
				else
				{
					StartCoroutine(IntoTrenchCheck(bControl.interactableBlocks[lastPointWidth,lastPointLength+1]));
					enmShooting.mayShoot = true;
					enmShooting.targetPos = bControl.interactableBlockObjs[lastPointWidth, lastPointLength+1].transform.position;
					animator.SetBool("Shoot", true);
					//					transform.DOLookAt(enmShooting.targetPos, 0.1f);
				}
//					enmShooting.StartShooting(bControl.interactableBlockObjs[lastPointWidth, lastPointLength+1].transform);
			}
//			else
//			{
//				StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[startBlockGoTo.width-1,startBlockGoTo.length].transform.position, true));
//			}
		}

//		if(inEnemyTrench && inPosition && jumpUpWhenEndMove)
//		{
//			StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[startBlockGoTo.width-1,startBlockGoTo.length].transform.position, false));
//		}
	}
	bool SameOccupied(int width, int length)
	{
		if(bControl.unitOccupiedBlocks.Count > 0)
		{
//			foreach(Block b in bControl.unitOccupiedBlocks)
//			foreach(Block b in bControl.unitMovingOccupiedBlocks)
//			{
//				if(b.width == width && b.length == length)
//				{
//					return false;
//				}
//				else
//				{
//					return true;
//				}
//			}
			foreach(Block b in bControl.unitOccupiedBlocks)
			{
//				Debug.Log (b.width + " " + b.length + " , " + width + " " + length);
				if(b.width == width && b.length == length)
				{
					return false;
					enmShooting.mayShoot = true;
				}
//				else
//				{
//					continue;
//				}
			}
		}
//		else
//		{
//			return true;
//		}
		return true;
	}

	IEnumerator IntoTrenchCheck(Block b)
	{
//		transform.DOLookAt(e, 0.1f);
		while(true)
		{
			if(SameOccupied(b.width, b.length))
			{
				StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[b.width,b.length].transform.position, true));

				enmShooting.mayShoot = false;
				yield break;

			}
			else
			{

				yield return null;
			}
			yield return new WaitForSeconds(0.1f);

		}
	}


	int FindClosest(int currentLane, int type)
	{
		int closestStopBlock = -1;
//		Debug.Log (currentLane);
		for(int i = 0; i< bControl.interactableBlockStates.GetLength(0); i++)
		{
			for(int i2 = 0; i2< bControl.interactableBlockStates.GetLength(1); i2++)
			{
				if(i2 == currentLane) // looping through all blocks on current lane
				{
					if(bControl.interactableBlockStates[i,i2] == type) 
					{
						if(i < currentBlock.width)
						{
							int tempClosest = i;
							if(tempClosest > closestStopBlock) // er den tættere på end alle andre stop blocke
							{
								closestStopBlock = i;
							}
						}
					}

				}
			}

		}
		return closestStopBlock;
	}
	Vector3[] FindPath(int[]bTypes, int level, int cLane)
	{
		int lane = cLane;
		int blockLane = cLane;
		int closest = 0;
		Block targetBlock = new Block{};
		int checkLane = cLane;
//		Debug.Log ("BNGUEG");
//		if (FindClosest (lane, 4) != 0 || FindClosest (lane, 0) != 0  || FindClosest(lane,2) != 0) {

		int p = lane;
		while(p >= 0 && p <= bControl.interactableBlockStates.GetLength(1))
		{

			int goThrough = Random.Range(0,2);
			if(goThrough == 1 && bControl.interactableBlockStates[FindClosest(p, 0)+1, p] == 1)
			{
				movingToEnd = false;
				closest = (FindClosest(p, 0)+1);
				blockLane = p;
//				continue;
				break;
			}
			else
			{
				if(FindClosest(p,4) == -1 && FindClosest(p,0) == -1 &&  FindClosest(p,2) == -1)
				{
	//									Debug.Log ("BNUEI22444532");
					movingToEnd = true;
					
					closest = (FindClosest(p, 0)+1);
					blockLane = p;
					break;
				}
				else if (FindClosest (p, 0) > FindClosest (p, 4) && FindClosest (p, 0) > FindClosest (p, 2)) 
				{
	//									Debug.Log (FindClosest(p,0));
					movingToEnd = false;
					closest = (FindClosest (p, 0) + 1);
					blockLane = p;
					break;
				}
				else if (FindClosest (p, 4) > FindClosest (p, 0) && FindClosest (p, 4) > FindClosest (p, 2)) 
				{
					//										movingToEnd = false;

	//				Debug.Log (FindClosest(p,4));
					//					for(int b = (FindClosest(i, 4)+1); b< bControl.width; b++)
					//					{
					//						Debug.Log (FindClosest(i,4)+b-FindClosest(i, 4));
					//						if(bControl.interactableBlockStates[FindClosest(i,4)+b-FindClosest(i, 4) , i-1] == 0)
					//                        {
					//							closest = (FindClosest(i, 4)+1);
					//							blockLane = i;
					//							break;
					//                        }
					//					}
					if(p !=0)
					{
	//					p++;
					
						if (bControl.interactableBlockStates [FindClosest (p, 4) + 1, p - 1] == 0) 
						{ // do so it checks all fromcurrent block
							movingToEnd = false;
							closest = (FindClosest (p, 4) + 1);
							blockLane = p;
							break;
						}
						else if (bControl.interactableBlockStates [FindClosest (p, 4) + 2, p - 1] == 0) 
						{ // do so it checks all fromcurrent block
							movingToEnd = false;
							closest = (FindClosest (p, 4) + 2);
							blockLane = p;
							break;
						}

					}


					//					if(bControl.interactableBlockStates[FindClosest(i,4)+2, i-1] == 0) // do so it checks all fromcurrent block
					//					{
					//						closest = (FindClosest(i, 4)+1);
					//						blockLane = i;
					//						break;
					//					}
					//					if(bControl.interactableBlockStates[FindClosest(i,4)+3, i-1] == 0) // do so it checks all fromcurrent block
					//					{
					//						closest = (FindClosest(i, 4)+1);
					//						blockLane = i;
					//						break;
					//					}
				}
				
				
				else if(FindClosest (p, 2) > FindClosest (p, 4) && FindClosest (p, 2) > FindClosest (p, 0))
				{
					movingToEnd = false;
					
	//				Debug.Log ("NBGUEI");
					closest = (FindClosest (p, 2) + 1);
					blockLane = p;
					break;
					//					if (bControl.interactableBlockStates [FindClosest (i, 2) + 1, i - 1] == 0) 
					//					{ // do so it checks all fromcurrent block
					//						closest = (FindClosest (i, 2) + 1);
					//						blockLane = i;
					//						break;
					//					}
					//					if (bControl.interactableBlockStates [FindClosest (i, 2) + 2, i - 1] == 0) 
					//					{ // do so it checks all fromcurrent block
					//                        closest = (FindClosest (i, 2) + 2);
					//                        blockLane = i;
					//                        break;
					//					}
				}
				if(p != 0 && p != bControl.interactableBlockStates.GetLength(1)-1)
				{
					int r = Random.Range(0,2);
	//				Debug.Log (r);
					if(r==0)
					{
						p++;
					}
					else
					{
						p--;
					}
				}
				else if(p ==0)
				{
					p++;
				}
				else if(p >= bControl.interactableBlockStates.GetLength(1)-1)
				{
					p--;
				}
			}

		}
//		if(lane > bControl.barracksBlockPos.length)
//		{
////			Debug.Log ("BNUE");
//
//			for (int i = lane; i> -1; i--) {
////			Debug.Log (lane);
////			Debug.Log (FindClosest(i,4) + " " + "4");
////			Debug.Log (FindClosest(i,0) + " " + "0");
////				if(FindClosest (i, 5) < FindClosest (i, 0) && FindClosest (i, 5) < FindClosest (i, 2) && FindClosest (i, 5) < FindClosest (i, 4))
////				{
////					Debug.Log ("BNUEI");
////					closest = (FindClosest(i, 5));
////					blockLane = i;
////					break;
////				}
//				if(FindClosest(i,4) == -1 && FindClosest(i,0) == -1 &&  FindClosest(i,2) == -1)
//				{
////					Debug.Log ("BNUEI22444532");
//					movingToEnd = true;
//
//					closest = (FindClosest(i, 0)+1);
//					blockLane = i;
//					break;
//				}
//				else if (FindClosest (i, 0) > FindClosest (i, 4) && FindClosest (i, 0) > FindClosest (i, 2)) 
//				{
//					//					Debug.Log (FindClosest(i,0));
//					movingToEnd = false;
//					closest = (FindClosest (i, 0) + 1);
//					blockLane = i;
//					break;
//				}
//				else if (FindClosest (i, 4) > FindClosest (i, 0) && FindClosest (i, 4) > FindClosest (i, 2)) 
//				{
//					//										movingToEnd = false;
//
//					Debug.Log (FindClosest(i,4)+1);
////					for(int b = (FindClosest(i, 4)+1); b< bControl.width; b++)
////					{
////						Debug.Log (FindClosest(i,4)+b-FindClosest(i, 4));
////						if(bControl.interactableBlockStates[FindClosest(i,4)+b-FindClosest(i, 4) , i-1] == 0)
////                        {
////							closest = (FindClosest(i, 4)+1);
////							blockLane = i;
////							break;
////                        }
////					}
//				if (bControl.interactableBlockStates [FindClosest (i, 4) + 1, i - 1] == 0) 
//				{ // do so it checks all fromcurrent block
//					closest = (FindClosest (i, 4) + 1);
//					blockLane = i;
//					break;
//				}
//				if (bControl.interactableBlockStates [FindClosest (i, 4) + 2, i - 1] == 0) 
//				{ // do so it checks all fromcurrent block
//					closest = (FindClosest (i, 4) + 2);
//					blockLane = i;
//					break;
//				}
////					if(bControl.interactableBlockStates[FindClosest(i,4)+2, i-1] == 0) // do so it checks all fromcurrent block
////					{
////						closest = (FindClosest(i, 4)+1);
////						blockLane = i;
////						break;
////					}
////					if(bControl.interactableBlockStates[FindClosest(i,4)+3, i-1] == 0) // do so it checks all fromcurrent block
////					{
////						closest = (FindClosest(i, 4)+1);
////						blockLane = i;
////						break;
////					}
//				}
//				
//
//				else if(FindClosest (i, 2) > FindClosest (i, 4) && FindClosest (i, 2) > FindClosest (i, 0))
//				{
//					movingToEnd = false;
//
//					Debug.Log ("NBGUEI");
//					closest = (FindClosest (i, 2) + 1);
//					blockLane = i;
//                    break;
//                    //					if (bControl.interactableBlockStates [FindClosest (i, 2) + 1, i - 1] == 0) 
////					{ // do so it checks all fromcurrent block
////						closest = (FindClosest (i, 2) + 1);
////						blockLane = i;
////						break;
////					}
////					if (bControl.interactableBlockStates [FindClosest (i, 2) + 2, i - 1] == 0) 
////					{ // do so it checks all fromcurrent block
////                        closest = (FindClosest (i, 2) + 2);
////                        blockLane = i;
////                        break;
////					}
//				}
//				
//				
//				checkLane++;
//
//			}
//		}
//		else
//		{
//			for (int i = lane; i<= bControl.length; i++) {
////				if(FindClosest (i, 5) < FindClosest (i, 0) && FindClosest (i, 5) < FindClosest (i, 2) && FindClosest (i, 5) < FindClosest (i, 4))
////				{
////
////					closest = (FindClosest(i, 5));
////					blockLane = i;
////					break;
////				}
//				if(FindClosest(i,4) == -1 && FindClosest(i,0) == -1 && FindClosest(i,2) == -1)
//				{
//					Debug.Log ("BGUEIGBEI");
//					movingToEnd = true;
//					closest = (FindClosest(i, 0)+1);
//					blockLane = i;
//					break;
//				}
//				else if (FindClosest (i, 0) > FindClosest (i, 4) && FindClosest (i, 0) > FindClosest (i, 2)) 
//				{
//					movingToEnd = false;
//
//					Debug.Log (i + " " + FindClosest(i,0) + " " + FindClosest(i,4));
//					closest = (FindClosest (i, 0) + 1);
//					blockLane = i;
//					break;
//				}
//				else if (FindClosest (i, 4) > FindClosest (i, 0) && FindClosest (i, 4) > FindClosest (i, 2)) 
//				{
//					movingToEnd = false;
//
//					if(i != 0)
//					{
//						if (bControl.interactableBlockStates [FindClosest (i, 4) + 1, i - 1] == 0) 
//						{ // do so it checks all fromcurrent block
//							closest = (FindClosest (i, 4) + 1);
//							blockLane = i;
//							break;
//						}
//						if (bControl.interactableBlockStates [FindClosest (i, 4) + 2, i - 1] == 0) 
//						{ // do so it checks all fromcurrent block
//							closest = (FindClosest (i, 4) + 2);
//							blockLane = i;
//							break;
//						}
//					}
//
//				}
//				
//				
//				else if(FindClosest (i, 2) > FindClosest (i, 4) && FindClosest (i, 2) > FindClosest (i, 0))
//				{
//					movingToEnd = false;
//
////					Debug.Log ("NBGUE222I");
////					if (bControl.interactableBlockStates [FindClosest (i, 2) + 1, i - 1] == 0) 
////					{ // do so it checks all fromcurrent block
////						closest = (FindClosest (i, 2) + 1);
////						blockLane = i;
////						break;
////					}
////					if (bControl.interactableBlockStates [FindClosest (i, 2) + 2, i - 1] == 0) 
////					{ // do so it checks all fromcurrent block
////						closest = (FindClosest (i, 2) + 2);
////                        blockLane = i;
////                        break;
////                    }
//					closest = (FindClosest (i, 2) + 1);
//					blockLane = i;
//					break;
//				}
//				
//				
//				checkLane++;
//				
//			}
//		}
		targetBlock = new Block{width = closest , length = blockLane, mWidth = closest, mLength =  blockLane};
//		Debug.Log (targetBlock.width + " " + targetBlock.length);

//		return closest;
//		Debug.Log (targetBlock.width + " " + targetBlock.length);
		return fPath.Patherize(currentBlock, targetBlock, bTypes, level);
//		return null;
	}


	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget) {
		// calculate vectors
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;
		toTargetXZ.y = 0;
		
		// calculate xz and y
		float y = toTarget.y;
		float xz = toTargetXZ.magnitude;
		
		// calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
		// where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
		// so xz = v0xz * t => v0xz = xz / t
		// and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
		float t = timeToTarget;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;
		
		// create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
		result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
		result.y = v0y;                                // set y to v0y (starting speed of y plane)
		
		return result;
	}

	IEnumerator JumpToTarget(Vector3 target, bool down)
	{
		animator.SetBool("Shoot", false);

		animator.SetBool ("Jump", true);
		GetComponent<Rigidbody> ().useGravity = true;
		if(down)
		{
			GetComponent<Rigidbody> ().AddForce (calculateBestThrowSpeed (this.transform.position, target, 1), ForceMode.VelocityChange);
		}
		else
		{
			GetComponent<Rigidbody> ().AddForce (calculateBestThrowSpeed (this.transform.position, new Vector3(target.x, 1, target.z), 1), ForceMode.VelocityChange);            
        }
//		yield return new WaitForSeconds (1f);
		bool on = true;
//		currentBlock.width = (int)target.x;
//		currentBlock.length = (int)target.z;
		bool endNextY = false;
		transform.DOLookAt(new Vector3(target.x, this.transform.position.y, target.z), 0.1f);
		while(on)
		{
			if(transform.position.y <= (1-GameController.instance.gameSettings.groundHeight) && down)
			{
				jumping = false;
				animator.SetBool ("Jump", false);
				GetComponent<Rigidbody> ().useGravity = false;
				GetComponent<Rigidbody> ().isKinematic = true;
				GetComponent<Rigidbody> ().isKinematic = false;
				transform.position = new Vector3(transform.position.x, (1-GameController.instance.gameSettings.groundHeight), transform.position.z);
				on = false;
//				DoMove(CalculatePath(0,0);
				inPlayerTrench = true;
//				bControl.enemyOccupiedBlocksTop.Remove(currentBlock);
				bControl.enemyOccupiedBlocksTrench.Add(currentBlock);
//				for(int i = currentBlock.mWidth; i < 0; i--)
//				{
////					for(int i2 = 
//					if(bControl.interactableBlockStates[i,currentBlock.mLength] == 0  && bControl.interactableBlockStates[i-1, currentBlock.mLength] == 1)
//					{
////						if(FindPath(new int[1]{1}, 1, currentBlock.mLength) 
//						if(fPath.Patherize(bControl.interactableBlocks[i-1, currentBlock.mLength], , new int[1]{1}, 1) != null)
//						{
//							Debug.Log ("LALA");
//						}
//					}
//				}
				//find the lowest block of state 0 on this block length(lane), see if any of the adjacent blocks are 1, if true then if that block can find a path to the backline
//				DoMove(fPath.Patherize(new Block{mWidth = (int)transform.position.x, mLength = (int)transform.position.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, (1-GameController.instance.gameSettings.groundHeight)));
//				currentBlock = new Block{mWidth = (int)this.transform.position.x, mLength = (int)this.transform.position.z};
				DoMove(fPath.Patherize(new Block{mWidth = (int)target.x, mLength = (int)target.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, (1-GameController.instance.gameSettings.groundHeight)));
//				transform.loo
				yield break;
			}
//			if(this.transform.position == target)
//			{
//				Debug.Log ("BGUE");
//			}
//			if(Mathf.Approximately(transform.position.y, 1) && !down && !endNextY)
//			{
//				endNextY = true;
//			}
//			if(transform.position.x <= target.x && !down) //transform.position.y == (int)1 transform.position.y >= (int)1 && 
//            {
//				jumping = false;
//				animator.SetBool ("Jump", false);
//				GetComponent<Rigidbody> ().useGravity = false;
//				GetComponent<Rigidbody> ().isKinematic = true;
//				GetComponent<Rigidbody> ().isKinematic = false;
//				transform.position = new Vector3(transform.position.x, 1, transform.position.z);
//				on = false;
//				//				DoMove(CalculatePath(0,0);
////				inPlayerTrench = true;
//				inEnemyTrench = false;
////				currentBlock = new Block{mWidth = startBlockGoTo.mWidth-1, mLength = startBlockGoTo.mLength};
//				currentBlock = new Block{mWidth = startBlockGoTo.mWidth-1, mLength = startBlockGoTo.mLength};
//				DoMove (FindPath (new int[2]{1,3},1, startBlockGoTo.mLength));
////				Debug.Log(FindPath (new int[1]{1},1, startBlockGoTo.length)[0]);
////				DoMove(outTrenchPath.FindPath(new int[1]{1},1, startBlockGoTo.length, currentBlock, false));
//				//				bControl.enemyOccupiedBlocksTop.Remove(currentBlock);
////				bControl.enemyOccupiedBlocksTrench.Add(currentBlock);
////				DoMove(fPath.Patherize(new Block{width = (int)transform.position.x, length = (int)transform.position.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, 0));
//                yield break;
//			}

			yield return null;
		}

	}
	IEnumerator CheckCurrentBlock()
	{
		bControl.enemyOccupiedBlocksTop.Add (currentBlock);
		while(true)
		{
			if(t != null)
			{
//				Debug.Log(t.PathGetPoint(
				width = currentBlock.width;
				length = currentBlock.length;
				if(t.PathGetPoint(t.ElapsedPercentage()) != Vector3.zero) //  || (currentBlock.width == lastPointWidth && currentBlock.length == lastPointLength)
				{
//					currentBlock = new Block{width = (int)t.PathGetPoint(t.ElapsedPercentage()).x, length = (int)t.PathGetPoint(t.ElapsedPercentage()).z};
					currentBlock.width = (int)t.PathGetPoint(t.ElapsedPercentage()).x;
					currentBlock.length = (int)t.PathGetPoint(t.ElapsedPercentage()).z;
				}
				else
				{
//					currentBlock = new Block{width = lastPointWidth, length = lastPointLength};
					currentBlock.width = lastPointWidth;
					currentBlock.length = lastPointLength;
				}
				if(!t.IsPlaying())
				{
					moving = false;
//					t.Kill();
				}
			}
			yield return new WaitForSeconds(0.1f);

		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("EnemyMoveSlow"))
		{
			ToggleSpeed(movementSlowScale);
		}
	}
	void OnTriggerExit(Collider col)
	{
		if(col.CompareTag("EnemyMoveSlow"))
		{
			ToggleSpeed(1);
		}
	}

//	IEnumerator LookForUnitsInTrench() // move to shooting
//	{
//		while(true)
//		{
//			if(inTrench)
//			{
//				for(int i = 0; i < bControl.interactableBlockStates.GetLength(0); i++)
//				{
//					for(int i2 = 0; i2 < bControl.interactableBlockStates.GetLength(1); i2++)
//					{
//						if(i == currentBlock.width || i2 == currentBlock.length) //iterate through blocks on current lanes
//						{
//							foreach(Block b in bControl.occupiedBlocks)
//							{
//								if(b.width == currentBlock.width || b.length == currentBlock.length)
//								{
//									//TODOimplement visibility check
//									enmShooting.StartShooting(bControl.interactableBlockObjs[b.width, b.length].transform);
//									if(t.IsPlaying())
//									{
//										t.Pause();
//									}
////									Debug.Log("Same lane width : " + currentBlock.width);
//								}
//								else
//								{
//									enmShooting.StopShooting();
//								}
////								if(b.length == currentBlock.length)
////								{
////									if(t.IsPlaying())
////									{
////										t.Pause();
////									}
//////									Debug.Log("Same lane length : " + currentBlock.length);
////								}
//							}
//						}
//					}
//				}
//			}
//			yield return new WaitForSeconds(0.3f);
//		}
//
//	}
//	IEnumerator CheckDist()
//	{
//
//		while(true)
//		{
//			if(bControl.occupiedBlocks.Count > 0)
//			{
//				foreach(Block b in bControl.occupiedBlocks)
//				{
//					Vector3 heading = bControl.interactableBlockObjs[b.width, b.length].transform.position - transform.position;
//					float dot = Vector3.Dot(heading, this.transform.forward);
//
//					if(b.length == currentBlock.length && dot >=0)
//					{
//							distToUnit = Vector3.Distance(bControl.interactableBlockObjs[b.width, b.length].transform.position, this.transform.position);
//							target = bControl.interactableBlockObjs[b.width, b.length].transform;
//							unitInFront = true;
//					}
//					else
//					{
//						unitInFront = false;
//					}
//
//				}
//			}
//			else
//			{
//				unitInFront = false;
//			}
//
//			yield return new WaitForSeconds(0.2f);
//		}
//	}
}
