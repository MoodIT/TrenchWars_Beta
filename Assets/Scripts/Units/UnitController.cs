using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
public class UnitController : MonoBehaviour {

	private GameObject hitObj;
	public List<GameObject> units = new List<GameObject>();
	public List<Block> unitPositions = new List<Block> ();
	public List<int> unitAttackLanes = new List<int>();

	public List<GameObject> startUnits = new List<GameObject> ();
	private BlockControl bControl;
	private GameStats gStats;
	private CommandPointsControl cPControl;
	private UIControl uICon;
	private FindPath fPath;
	private SelectBlocks sBlocks;
	public Vector2 startBlock = new Vector2 (1, 3);
//	[HideInInspector]
	public GameObject selectedUnit;
	public bool spawnUnit;
	[HideInInspector]
	public int flamerAmt;
	[HideInInspector]
	public int gunnerAmt;
	[HideInInspector]
	public int grenaderAmt;

//	[HideInInspector]
	public bool diggerOutDigging;
//	private bool chosenSideToAimAt;
	public enum MovingStates{SELECTING_UNIT, CHOSING_END, CHOSING_SIDE_TO_AIM, SIDE_TO_AIM_CHOSEN};
	public MovingStates movingStates;

	public delegate void SelectedUnitUpdate(bool enbl);
	public event SelectedUnitUpdate OnSelectedUnitUpdate;

	private Block tempBlock;

	public GameObject[] arrows = new GameObject[]{};
//	private enum GameObject{GUNNER, FLAMER, GRENADER};
//	private GameObject unitType;
	public bool selectBlocksShow;

	private GameObject tempObj;
	[HideInInspector]
	private Color tempColor;
	[HideInInspector]
	public List<GameObject> tempObjs;
	[HideInInspector]
	public List<Color> tempColors;
//	[HideInInspector]
	public GameObject spawningUnitType;
	public UnitShooting.WeaponType unitType;
	[HideInInspector]
		public float cost;
	public GameObject selectingCircle;
	public bool selectingUnitPosition;
	public bool checkOccupied;
	void Start()
	{
		Application.targetFrameRate = 30;
		movingStates = MovingStates.SELECTING_UNIT;
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		gStats = GameController.instance.gameStats;
		cPControl = GameController.instance.gameStatsObj.GetComponent<CommandPointsControl> ();
		uICon = GameController.instance.uiControls;
		fPath = GetComponent<FindPath>();
		sBlocks = bControl.GetComponent<SelectBlocks>();
//		bControl.interactableBlockStates[(int)startBlock.x,(int)startBlock.y] = 0; //done in block control atm
//		arrows = new

		//SKAL SKRIVES OM TIL MERE MODULÆRT(TYPER AF UNITS ETC!!!
//		GameObject spawnButtonGunner = Instantiate (GameController.instance.gameSettings.spawnUnitButton, Vector3.zero, Quaternion.identity) as GameObject;
//		spawnButtonGunner.transform.SetParent (GameController.instance.uiCanvas.transform);
//		RectTransform rT = spawnButtonGunner.GetComponent<RectTransform> ();
//		RectTransform rTPrefab = GameController.instance.gameSettings.spawnUnitButton.GetComponent<RectTransform> ();
//		rT.sizeDelta = rTPrefab.sizeDelta;
//		rT.anchoredPosition = rTPrefab.anchoredPosition;
//		rT.localScale = rTPrefab.localScale;
//		spawnButtonGunner.GetComponent<Button>().onClick.AddListener(() => SpawnUnit());


		//instantiate comander - skal laves om så man selv kan placere ham(sammen med level editor stuff)

		if(startUnits.Count > 0)
		{
			for(int i = 0; i < startUnits.Count; i++)
			{

			}
		}
	} 


	void RayCastHit()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		bool occupied = false;
		if (Physics.Raycast(ray, out hit, 100))
		{
			MovingStateSelecting();
			hitObj = hit.collider.gameObject;
		}
	}
	void MovingStateSelecting()
	{
		if(movingStates == MovingStates.SELECTING_UNIT)
		{
			UnitInPosition(false);
		}
	}


	void UnitInPosition(bool spawnUnit)
	{
//		Debug.Log ("NUGE");
		for(int i = 0; i < units.Count; i++)
		{
			if((hitObj == units[i] && units[i].GetComponent<UnitMovement>().inPosition) || spawnUnit)
			{
				SelectingUnitPlacementActivate();
				selectingUnitPosition = true;
//				if(!spawnUnit)
//				{
//					for(int c = (int)units[i].transform.position.x-1; c<= (int)units[i].transform.position.x+1; c++)
//					{
//						for(int c2 = (int)units[i].transform.position.z-1; c2 <= (int)units[i].transform.position.z+1; c2++)
//						{
//							AddAimAtBlocks(c,c2,i);
//						}
//					}
//				
//					arrows = new GameObject[bControl.aimAtBlocks.Count];
//					for(int i2 = 0; i2 < bControl.aimAtBlocks.Count; i2++)
//					{
//	//					Debug.Log(bControl.aimAtBlocks[i].obj);
//						arrows[i2] = Instantiate(GameController.instance.gameSettings.arrowPrefab, new Vector3(bControl.aimAtBlocks[i2].mObj.transform.position.x, 0.65f, bControl.aimAtBlocks[i2].mObj.transform.position.z), Quaternion.identity) as GameObject;
//						Vector3 dir = units[i].transform.position - arrows[i2].transform.position;
//						dir.y = 90;//								dir.x= 0;
//						arrows[i2].transform.forward = dir;
//						
//					}
//				}
				selectedUnit = hitObj;
				selectedUnit.GetComponent<Renderer>().material.color = Color.blue;
				bControl.GridToggle(true);
				movingStates = MovingStates.CHOSING_END;
				
				if(OnSelectedUnitUpdate != null)
					OnSelectedUnitUpdate(true);
				
				break;
			}
		}
	}

	void AddAimAtBlocks(int c, int c2, int i)
	{
		//if block is inside bounds of level
		if(c2 >= 0 && c >= 0 && c2 < bControl.length && c < bControl.width && bControl.interactableBlockStates[c,c2] != 0 && (c == (int)units[i].transform.position.x || c2 == (int)units[i].transform.position.z)) //&& c <= bControl.length && c2 <= bControl.width
		{
			tempBlock = bControl.interactableBlocks[(int)units[i].transform.position.x,(int)units[i].transform.position.z];
			bControl.aimAtBlocks.Add(bControl.interactableBlocks[c,c2]);
		}
	}


	void MovingUnit()
	{
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		RaycastHit hit;
		bool occupied = false;
//		Debug.Log (hitObj);
//		if(!Physics.Raycast(ray, out hit, 100)) return;
		GameObject moveObj = null;
//		if(bControl.interactableBlockStates[(int)hitObj.transform.position.x,(int)hitObj.transform.position.z] == 0)
//		{
		int aroundRange = 1;
			for(int i2 = 0; i2< bControl.interactableBlockObjs.GetLength(0); i2++)
			{				
//				Debug.Log ("JIOIOIJIO2222");

				for(int i3 = 0; i3<bControl.interactableBlockObjs.GetLength(1); i3++)
				{
//					Debug.Log ("JIOIOIJIO");
	                if(hitObj != bControl.interactableBlockObjs[i2,i3]) continue; //didnt hit a interactable block
									
	                
					if(selectedUnit == null) return; // no selected unit
//					Debug.Log ("BGUGBUEIGBUEGUOSEGIOJ");
					if(movingStates != MovingStates.CHOSING_END) return; //not the correct state
//					Debug.Log ("JIOIOIJIO2222");
	//				if(selectedUnit.GetComponent<UnitMovement>().currentBlock.width == i2) return; //diagonal block
				if(GetComponent<FindPath>().Patherize(selectedUnit.GetComponent<UnitMovement>().currentBlock, new Block{width = i2, length = i3}, new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)) != null)
					{

	//					if(!checkOccupied)
	//					{
						foreach(Block b in bControl.unitOccupiedBlocks)
						{
							if(b.width == i2 && b.length == i3)//occupied = true;
							{

								foreach(GameObject uO in bControl.unitsOnBlocks)
								{
									if(uO != null && (int)uO.transform.position.x == i2 && (int)uO.transform.position.z == i3)
									{

									for(int bI = i2-aroundRange; bI <= i2+aroundRange; bI++)
										{
										for(int bI2 = i3-aroundRange; bI2 <= i3+aroundRange; bI2++)
											{
												if(bControl.interactableBlockStates[bI,bI2] == 0) // fix så det også irker på udenfor bane
												{
													
	                                                if(bI != i2 || bI2 != i3)
	                                                {

//														Debug.Log (bI + " " + bI2);
														if(uO != selectedUnit)
														{
//														    Debug.Log ("BUEGUIEGUIEGUEUIEGUIEUIEUIUIUIUIUIUIUIUIUIUIUI22222222");
//															Debug.Log (selectedUnit.transform.position + " " +  uO.transform.position);
															tempBlock = bControl.interactableBlocks[bI,bI2];
//		                                                    Debug.Log (bI + " " + bI2);
															occupied = true;
															checkOccupied = true;
		//													return;
															moveObj = uO;
		//														uO.GetComponent<UnitMovement>().MoveOnPath(uO.transform, GetComponent<FindPath>().Patherize(uO.GetComponent<UnitMovement>().currentBlock, tempBlock, new int[1]{0}, 0));
															break;
														}
	                                                }
	                                            }
	                                        }
	                                        
	                                    }
	                                    //									
	                                    //                                            
	//												//
	//												occupied = true;
	//                                                tempBlock = bControl.interactableBlocks[i2,i3];
	//												Debug.Log ("BOEIOEUIOTUIETUIETUITUIE");
	//												uO.GetComponent<UnitMovement>().MoveOnPath(uO.transform, GetComponent<FindPath>().Patherize(uO.GetComponent<UnitMovement>().currentBlock, tempBlock, new int[1]{0}, 0));
	//												break;
	//											}
	//										}
	//									}
	                                }
	                            }

							}
						}
						if(moveObj != null)
						{
							moveObj.GetComponent<UnitMovement>().lookAtBlock = bControl.interactableBlockObjs[i2+1, i3].transform;				
							moveObj.GetComponent<UnitMovement>().MoveOnPath(moveObj.transform, GetComponent<FindPath>().Patherize(moveObj.GetComponent<UnitMovement>().currentBlock, tempBlock, new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)), true);
//							Debug.Log(moveObj.GetComponent<UnitMovement>().currentBlock.mWidth + " " + moveObj.GetComponent<UnitMovement>().currentBlock.mLength + " " + tempBlock.mWidth + " " + tempBlock.mLength);
						}
	//					}
	//					if(occupied)continue;//the clicked block is occupied 
	//					{
	//
	//					}
						tempBlock = bControl.interactableBlocks[i2,i3];
//						Debug.Log(selectedUnit.transform.position.x + " " + selectedUnit.transform.position.z + " " + tempBlock.mWidth + " " + tempBlock.mLength);
						if((int)selectedUnit.transform.position.x != tempBlock.mWidth || (int)selectedUnit.transform.position.z != tempBlock.mLength)
						{					
//							Debug.Log(selectedUnit.transform.position.x + " " + selectedUnit.transform.position.z + " " + tempBlock.mWidth + " " + tempBlock.mLength + " lalalalal");
						if(GetComponent<FindPath>().Patherize(selectedUnit.GetComponent<UnitMovement>().currentBlock, tempBlock, new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)) != null)
                        {
	                        	selectedUnit.GetComponent<UnitMovement>().lookAtBlock = bControl.interactableBlockObjs[i2+1, i3].transform;				
							selectedUnit.GetComponent<UnitMovement>().MoveOnPath(selectedUnit.transform, GetComponent<FindPath>().Patherize(selectedUnit.GetComponent<UnitMovement>().currentBlock, tempBlock, new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)), false);
							}
						}            			
						
					}
//					movingStates = MovingStates.SELECTING_UNIT;
					selectedUnit = null; //fix
					bControl.GridToggle(false);
//					//					selectingUnitPosition = false;
					movingStates = MovingStates.SELECTING_UNIT;
//					//                    SelectingUnitPlacementDeactivate(t, false);
					if(OnSelectedUnitUpdate != null)
	                    OnSelectedUnitUpdate(false);
//	                
	//                DisableAimtAtArrows();
	//				SelectingUnitPlacementDeactivate(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (), false);
	//				else if(bControl.aimAtBlocks.Contains(bControl.interactableBlocks[i2,i3]))
	//				{
	//					selectedUnit.GetComponent<UnitMovement>().LookAtBlock(bControl.interactableBlockObjs[i2,i3].transform.position);			
	//					selectedUnit.GetComponent<UnitMovement>().lookAtBlock = bControl.interactableBlockObjs[i2+1, i3].transform;
	//
	////					tempBlock = bControl.interactableBlocks[i2,i3];
	//
	//					selectedUnit = null; //fix
	//					bControl.GridToggle(false);
	//					
	//					if(OnSelectedUnitUpdate != null)
	//						OnSelectedUnitUpdate(false);
	//					
	//					DisableAimtAtArrows();	
	//
	//					StartCoroutine(SideChosen()); //making sure that player doesnt select a block while chosing direction
	//				}
				}
			}
//		}
//		Debug.Log ("BUEGUIO");
//		selectedUnit = null; //fix
//		bControl.GridToggle(false);
//		//					selectingUnitPosition = false;
//		movingStates = MovingStates.SELECTING_UNIT;
//		//                    SelectingUnitPlacementDeactivate(t, false);
//		if(OnSelectedUnitUpdate != null)
//			OnSelectedUnitUpdate(false);
	}
	
	void DisableAimtAtArrows()
	{
		if(bControl.aimAtBlocks.Count > 0)
		{
			for(int arr = 0; arr < bControl.aimAtBlocks.Count; arr++)
			{
				Destroy (arrows[arr]);				
			}
			arrows = new GameObject[]{};			
		}		
		bControl.aimAtBlocks.Clear();
	}
	void Update()
	{
		bool t = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (); // works on tablet? id might need to be -1 or 1 or 0 ?
//		Debug.Log (t);
		if(Input.GetMouseButtonDown(0) && !t) //&& bControl.bState == BlockControl.BState.directingUnits
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool occupied = false;
			if (Physics.Raycast(ray, out hit, 100))
			{
				hitObj = hit.collider.gameObject;
				selectedUnit = hitObj;
				MovingStateSelecting();
//				MovingUnit();
			}
		}
//		if(Input.GetMouseButtonUp(0) && !t) //&& bControl.bState == BlockControl.BState.directingUnits
//		{
//			MovingUnit();
//
//		}

//		if(Input.GetMouseButtonUp(0) && selectBlocksShow && t)
//		{
//			selectBlocksShow = false;
//			ToggleUnitSpawningMode(false);
//
//		}
		if(selectingUnitPosition)
		{
			SelectingUnitPlacement(false);
		}
		if(Input.GetMouseButtonUp(0))
		{
//			Debug.Log ("BUEGUEIGIUEB");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			//				bool occupied = false;

			if (Physics.Raycast(ray, out hit, 100))
			{
				Debug.Log (hit.collider.name);

				if(selectBlocksShow)
				{
					hitObj = hit.collider.gameObject;
					SelectingUnitPlacementDeactivate(t, true);
				}
				if(selectingUnitPosition)
				{

                    hitObj = hit.collider.gameObject;
					SelectingUnitPlacementDeactivate(t, false);
				}
			}
			//spawn unit if curselected object is 0 state and path != null
		}
		if(selectBlocksShow)
		{
			SelectingUnitPlacement(true);
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if (Physics.Raycast(ray, out hit, 100))
//			{
//				for(int i = 0; i < bControl.interactableBlockObjs.GetLength(0); i++)
//				{
//					for(int i2 = 0; i2 < bControl.interactableBlockObjs.GetLength(1); i2++)
//					{
//						if(hit.collider.gameObject != bControl.interactableBlockObjs[i,i2]) continue; //is not an interactable block
//
//						if(hit.collider.gameObject == tempObj) continue; // is the same as previous
//
////						if(tempObj != null && tempObj.GetComponent<Renderer>())
////						{
////							tempObj.GetComponent<Renderer>().material.color = tempColor;
////							for(int d = 0; d < tempObjs.Count; d++)
////							{
////								if(tempObjs[d] != null)
////								{
////									tempObjs[d].GetComponent<Renderer>().material.color = tempColors[d];
////								}
////							}
////							tempObjs.Clear();
////							tempColors.Clear();
////						}
////						else
////						{
////							tempObjs.Clear();
////							tempColors.Clear();
////						}
////						for(int c = i-1; c <= i+1; c++)
////						{
////							for(int c2 = i2-1; c2 <= i2+1; c2++)
////							{
////								if(c >= 0 && c2 >=0 ) //  && i <= pBlock.width-1 && i2 <= pBlock.length
////								{
////									if(c < bControl.interactableBlockStates.GetLength(0) && c2 < bControl.interactableBlockStates.GetLength(1))
////									{
////										if(bControl.interactableBlockStates[c,c2] != 0)
////										{
////											tempObjs.Add(bControl.interactableBlockObjs[c,c2]);
////											tempColors.Add(bControl.interactableBlockObjs[c,c2].GetComponent<Renderer>().material.color);
////										}
////									}
////								}									
////							}
////						}
////						tempColor = hit.collider.gameObject.GetComponent<Renderer>().material.color;
//						tempObj = hit.collider.gameObject;
//
//
//
////						if(bControl.interactableBlockStates[(int)tempObj.transform.position.x,(int) tempObj.transform.position.z] == 0)
////						{
////							for(int d2 = 0; d2 < tempObjs.Count; d2++)
////							{
////								if(tempObjs[d2] != null)
////								{
////									if(bControl.interactableBlockStates[(int)tempObjs[d2].transform.position.x,(int)tempObjs[d2].transform.position.z] == 0)
////									{
////										tempObjs[d2].GetComponent<Renderer>().material.color = new Color(GameController.instance.gameSettings.blockDragColorAroundPositive.r, GameController.instance.gameSettings.blockDragColorAroundPositive.g, GameController.instance.gameSettings.blockDragColorAroundPositive.b, 0.5f);
////									}
////									else
////									{
////										tempObjs[d2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.blockDragColorAroundPositive;
////									}
////								}
////							}
////							hit.collider.gameObject.GetComponent<Renderer>().material.color = GameController.instance.gameSettings.blockDragColorCenterPositive;
//
////						}
////						else
////						{
////							for(int d2 = 0; d2 < tempObjs.Count; d2++)
////							{
////								if(tempObjs[d2] != null)
////								{	if(bControl.interactableBlockStates[(int)tempObjs[d2].transform.position.x,(int)tempObjs[d2].transform.position.z] == 0)
////									{
////										tempObjs[d2].GetComponent<Renderer>().material.color = new Color(GameController.instance.gameSettings.blockDragColorAroundNegative.r, GameController.instance.gameSettings.blockDragColorAroundNegative.g, GameController.instance.gameSettings.blockDragColorAroundNegative.b, 0.5f);
////									}
////									else
////									{
////										tempObjs[d2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.blockDragColorAroundNegative;
////									}
////								}
////							}
////							hit.collider.gameObject.GetComponent<Renderer>().material.color = GameController.instance.gameSettings.blockDragColorCenterNegative;
//
////						}
//
//					}
//				}
//			}

		}
	}
	void SelectingUnitPlacementActivate()
	{
		selectingCircle = InstCircle();
		foreach(GameObject u in units)
		{
			if(u!= null)
			{
				u.GetComponent<Collider>().enabled = false;
			}
		}

	}
	void SelectingUnitPlacement(bool placingSpawn)
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
					
//					if(hit.collider.gameObject == tempObj) continue; // is the same as previous
					if(placingSpawn)
					{
//						Debug.Log(hit.collider.gameObject);
						tempObj = hit.collider.gameObject;
						hitObj = hit.collider.gameObject;
					}
				}
			}
		}
	}
	void SelectingUnitPlacementDeactivate(bool t, bool spawn)
	{
		foreach(GameObject u in units)
		{
			if(u != null)
			{
				u.GetComponent<Collider>().enabled = true;
			}
        }
		selectBlocksShow = false;
		selectingUnitPosition = false;
		ToggleUnitSpawningMode(false);
		Destroy (selectingCircle);
		if(!t)
		{
			if(spawn && tempObj != null)
			{
				if(bControl.interactableBlockStates[(int)tempObj.transform.position.x, (int)tempObj.transform.position.z] == 0)
				{
					Debug.Log (tempObj.transform.position.x + " " +  tempObj.transform.position.z);
					if(fPath.Patherize(bControl.barracksBlockPos, bControl.interactableBlocks[(int)tempObj.transform.position.x, (int)tempObj.transform.position.z], new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)) != null)
					{
						SpawnUnit(spawningUnitType, false, cost, new Block{mWidth = (int)tempObj.transform.position.x, mLength = (int)tempObj.transform.position.z});

//						MovingUnit();
						//					Debug.Log ("BNGUEIG");
						//					g.GetComponent<UnitMovement>().MoveOnPath(g.transform, fPath.Patherize(bControl.barracksBlockPos, new Block{mWidth = (int)tempObj.transform.position.x, mLength = (int)tempObj.transform.position.z}, new int[1]{0},0));
					}			
				}
			}
			else
			{
				MovingUnit();
			}
		}
//		else
//		{
		selectedUnit = null; //fix
		bControl.GridToggle(false);
		//					selectingUnitPosition = false;
		movingStates = MovingStates.SELECTING_UNIT;
		//                    SelectingUnitPlacementDeactivate(t, false);
		if(OnSelectedUnitUpdate != null)
			OnSelectedUnitUpdate(false);
//		}
//		for(int d2 = 0; d2 < tempObjs.Count; d2++)
//		{
//			if(tempObjs[d2] != null)
//			{
//				//				Debug.Log ("BUGE");
//				if(bControl.interactableBlockStates[(int)tempObjs[d2].transform.position.x, (int)tempObjs[d2].transform.position.z] == 0 || bControl.interactableBlockStates[(int)tempObjs[d2].transform.position.x, (int)tempObjs[d2].transform.position.z] == 2)
//				{
//					tempObjs[d2].GetComponent<Renderer>().material.color = Color.clear;
//				}
//				else
//				{
//					tempObjs[d2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.blockStartColor;
//				}
//			}
//		}
	}
	GameObject InstCircle()
	{
		GameObject circle = Instantiate(GameController.instance.gameSettings.unitPlacementCircle, Vector3.zero, GameController.instance.gameSettings.unitPlacementCircle.transform.rotation) as GameObject;
		return circle;
//		GameController.instance.gameSettings.unitPlacementCircle
	}
	IEnumerator SideChosen()
	{
		yield return new WaitForEndOfFrame ();
		movingStates = MovingStates.SELECTING_UNIT;

		
	}
	public void SendAttackUnits()
	{
		if(units.Count > 0)
		{
			for(int i = 0; i< units.Count; i++)
			{
				units[i].GetComponent<UnitMovement>().movementMode = UnitMovement.MovementMode.ATTACK;
				unitAttackLanes.Add(units[i].GetComponent<UnitMovement>().currentBlock.length);
				units[i].GetComponent<UnitMovement>().MoveOnPath(units[i].transform, GetComponent<FindPath>().Patherize(units[i].GetComponent<UnitMovement>().currentBlock, BlockClosestToEnemyBase(), new int[1]{0}, (1-GameController.instance.gameSettings.groundHeight)), false);
	//			u.GetComponent<UnitMovement>().currentBlock
			}
		}
		else
		{
			Debug.Log("No units to send attacking");
		}
		//find all units(that should attack, if you can choose them)
		//remember their lanes
		//find the walkable block cloest to the enemy base
		//send the units to the found block

		//in unit movement set to attack unit so they jump up on completion

	}

	Block BlockClosestToEnemyBase()
	{
		Block closestBlock = new Block{mWidth = 0};
		for(int i = 0; i < bControl.interactableBlockStates1D.Length; i++)
		{
			if(bControl.interactableBlockStates1D[i] != 0) continue;
			if(bControl.interactableBlocks1D[i].mWidth > bControl.width-2) continue;
			if(bControl.interactableBlocks1D[i].mWidth > closestBlock.mWidth)
			{
				closestBlock = bControl.interactableBlocks1D[i];	
			}
		}
		return closestBlock;
	}
	public void SendDigUnit()
	{
		diggerOutDigging = true;
		GameObject digger = SpawnUnit (GameController.instance.gameSettings.diggerUnit, true, 0, null);
		StartCoroutine(digger.GetComponent<UnitDigging> ().Digging ());
	}

	public void ToggleUnitSpawningMode(bool collidersOn)
	{
//		Debug.Log ("BGUEI");
		//(de)activate block colliders
		sBlocks.ResetSelectedBlocks();
		if(collidersOn)
		{

			bControl.UpdateDestroyedBlockColliders(true);
			selectBlocksShow = true;
			SelectingUnitPlacementActivate();

		}
		else
		{
			bControl.UpdateDestroyedBlockColliders(false);
			selectBlocksShow = false;
		}
//			for(int i = 0; i < bControl.interactableBlockObjs1D.Length; i++)
//			{
//				if(bControl.interactableBlockStates1D[i] == 0)
//				{
//					bControl.interactableBlockObjs1D[i].GetComponent<Collider>().enabled = true;
//				}
//			}
//		}
//		else
//		{
//			for(int i = 0; i < bControl.interactableBlockObjs1D.Length; i++)
//			{
//				if(bControl.interactableBlockStates1D[i] == 0)
//				{
//					bControl.interactableBlockObjs1D[i].GetComponent<Collider>().enabled = false;
//				}
//			}
//		}
	}
	public GameObject SpawnUnit(GameObject unitOb, bool digger, float cost, Block destinationB)
	{
		tempObj = null;
		if(digger)
		{
			Vector3 p = Vector3.zero;
			for(int i = bControl.selectedBlocks[0].mWidth-1; i<= bControl.selectedBlocks[0].mWidth+1; i++)
			{
				for(int i2 = bControl.selectedBlocks[0].mLength-1; i2 <= bControl.selectedBlocks[0].mLength+1; i2++)
				{
					if(i >= 0 && i2 >=0 ) //  && i <= pBlock.width-1 && i2 <= pBlock.length
					{
						if(i < bControl.interactableBlockStates.GetLength(0) && i2 < bControl.interactableBlockStates.GetLength(1))
						{
							if(bControl.interactableBlockStates[i,i2] == 0)
							{		
								if(i == bControl.selectedBlocks[0].mWidth || i2 == bControl.selectedBlocks[0].mLength)
								{
									p = new Vector3(bControl.interactableBlockObjs[i,i2].transform.position.x, (1-GameController.instance.gameSettings.groundHeight), bControl.interactableBlockObjs[i,i2].transform.position.z);
								}
							}
						}
//						
					}
					
				}
			}

//			GameObject unit = Instantiate(unitOb, new Vector3(bControl.barracksBlockPos.width, 0, bControl.barracksBlockPos.length), Quaternion.identity) as GameObject;
			GameObject unit = Instantiate(unitOb, p, Quaternion.identity) as GameObject;
			UnitDigging uDig = unit.GetComponent<UnitDigging>();

//			unit.transform.position = new Vector3(unit.GetComponent<UnitDigging>().blocksToDig[
			unit.GetComponent<UnitMovement>().currentBlock = new Block{width = (int)bControl.barracksBlockPos.width, length = (int)bControl.barracksBlockPos.length};
			unit.GetComponent<UnitMovement> ().lastPointWidth = bControl.barracksBlockPos.width;
			unit.GetComponent<UnitMovement> ().lastPointLength = bControl.barracksBlockPos.length - 1;
//			bControl.selectedBlocks.Clear();
			sBlocks.ResetSelectedBlocks();
			return unit;
		}
		else
		{

			if(gStats.commandPoints >= cost)
			{
				GameObject unit = Instantiate(unitOb, new Vector3(bControl.barracksBlockPos.width, (1-GameController.instance.gameSettings.groundHeight), bControl.barracksBlockPos.length), Quaternion.identity) as GameObject;
				
				unit.GetComponent<UnitMovement>().currentBlock = new Block{width = (int)bControl.barracksBlockPos.width, length = (int)bControl.barracksBlockPos.length};
				unit.GetComponent<UnitMovement> ().lastPointWidth = bControl.barracksBlockPos.width;
				unit.GetComponent<UnitMovement> ().lastPointLength = bControl.barracksBlockPos.length - 1;

				units.Add (unit); 
//				Debug.Log (GetComponent<FindPath>().Patherize(unit.GetComponent<UnitMovement>().currentBlock, destinationB, new int[1]{0}, 0));
//				if(unitType == UnitShooting.WeaponType.FLAMER)
//				{
//					flamerAmt++;
//					GameController.instance.uiControls.spawnFlamerObj.GetComponent<SpawnUnitButton>().maxUnitsText.GetComponent<Text>().text = flamerAmt+"/"+GameController.instance.gameSettings.maxFlamers;
//				}
				UpdateUnitNumber(unitType, true);

//				Debug.Log (unit.GetComponent<UnitMovement>().MoveOnPath(unit.transform, fPath.Patherize(bControl.barracksBlockPos, destinationB, new int[1]{0}, 0)));
//				unit.GetComponent<UnitMovement>().MoveOnPath(unit.transform, fPath.Patherize(bControl.barracksBlockPos, destinationB, new int[1]{0},0));
//				unit.transform.DOPath(fPath.Patherize(bControl.barracksBlockPos, destinationB, new int[1]{0}, 0), 2).SetSpeedBased();
//				unitPositions.Add(new Block{width = (int)bControl.barracksBlockPos.width, length = (int)bControl.barracksBlockPos.length});
//				unit.transform.DOMove (new Vector3 (bControl.barracksBlockPos.width, 0, bControl.barracksBlockPos.length - 1), 0.5f).OnComplete(unit.GetComponent<UnitMovement> ().InPosition );
//				unit.transform.DOLookAt (new Vector3 (bControl.barracksBlockPos.width, 0, bControl.barracksBlockPos.length - 1), 0.01f);
				gStats.commandPoints -= cost;
				cPControl.UpdateCommandPoints();
//				hitObj = unit.gameObject;
				UnitInPosition(false);
				StartCoroutine(MoveAfterSec(unit, unit.GetComponent<UnitMovement>().currentBlock, destinationB));

//				GameController.instance.uiControls.sp
				return unit;
			}
			else
			{
				return null;
			}
		}

	}

	public void UpdateUnitNumber(UnitShooting.WeaponType unitType, bool addRemove) //true is add
	{
		switch(unitType)
		{
		case UnitShooting.WeaponType.FLAMER:
			if(addRemove)
			{
				flamerAmt++;
			}
			else
			{
				flamerAmt--;
			}
			GameController.instance.uiControls.spawnFlamerObj.GetComponent<SpawnUnitButton>().maxUnitsText.GetComponent<Text>().text = flamerAmt+"/"+GameController.instance.gameSettings.maxFlamers;
			break;
		case UnitShooting.WeaponType.GRENADER:
			if(addRemove)
			{
				grenaderAmt++;
			}
			else
			{
				grenaderAmt--;
			}
			GameController.instance.uiControls.spawnGrenaderObj.GetComponent<SpawnUnitButton>().maxUnitsText.GetComponent<Text>().text = grenaderAmt+"/"+GameController.instance.gameSettings.maxGrenaders;
			break;
		case UnitShooting.WeaponType.MACHINEGUN:
			if(addRemove)
			{
				gunnerAmt++;
			}
			else
			{
				gunnerAmt--;
			}
			GameController.instance.uiControls.spawnGunnerObj.GetComponent<SpawnUnitButton>().maxUnitsText.GetComponent<Text>().text = gunnerAmt+"/"+GameController.instance.gameSettings.maxGunners;
			break;
		}
	}
	IEnumerator MoveAfterSec(GameObject unit, Block sBlock, Block eBlock)
	{
		yield return new WaitForEndOfFrame();
		movingStates = MovingStates.CHOSING_END;
		selectedUnit = unit;
		MovingUnit();
//		unit.GetComponent<UnitMovement>().lookAtBlock = bControl.interactableBlockObjs[eBlock.mWidth+1, eBlock.mLength].transform;				

//		unit.GetComponent<UnitMovement>().MoveOnPath(unit.transform, GetComponent<FindPath>().Patherize(sBlock, eBlock, new int[2]{0,2}, 0));

	}
}
