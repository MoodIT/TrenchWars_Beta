using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

[ExecuteInEditMode]
[System.Serializable]
public class BlockControl : MonoBehaviour {
	[HideInInspector]
	public UnitController uCon;

	private GameStats gStats;
	private SelectBlocks sBlocks;
	private CommandPointsControl cmPControl;
	private EnemySpawning enmSpawning;
	[HideInInspector]
	[SerializeField]
	public int[,] interactableBlockStates; // 0 destroyed & moveable, 1 not destroyed & diggable, 2 destroyed & not movable, 3 not destroyed && not diggable but movable, 4 not destroyed && not diggable && not movable(obstacle) // 5 end game block
	[SerializeField]
	public int[] interactableBlockStates1D;
	[HideInInspector]
	[SerializeField]
	public GameObject[,] interactableBlockObjs;
	public GameObject[] interactableBlockObjs1D;

	public GameObject[] intObjs;
//	[HideInInspector]
	public List<Block> unitOccupiedBlocks;
	public List<GameObject> unitsOnBlocks;
	public List<Block> unitMovingOccupiedBlocks;
	public List<Block> enemyOccupiedBlocksTop;
	public List<Block> enemyOccupiedBlocksTrench;
	[SerializeField]
	public Block[,] interactableBlocks;
	public Block[] interactableBlocks1D;
	[SerializeField]
	public GameObject[,] interactableGrids;
	public GameObject[] interactableGrids1D;

	public List<Block> aimAtBlocks;

	//	[HideInInspector]
	public List<Block> selectedBlocks = new List<Block> ();
	public List<Block> toBeDugBlocks = new List<Block>();
	public int width = 7;
	public int length = 10;
	[HideInInspector]
	public int[,] nonInteractableBlockStates;

	public GameObject[,] nonInteractableBlockObjects;
//	width and length of the ground(below the interactable tiles) and non interactable tiles
	public int extraGroundWidth = 15;
	public int extraGroundLength = 10;
//	public List<GameObject> blocks;
	public bool spawned;
//	[Space(10)]
	public Block barracksBlockPos = new Block {width = 1, length = 5};
	public Block enemyBarracksBlockPos = new Block {width = 12, length = 5};
//	public Block enemyBarracksBlockPosT = new Block {width = 12, length = 5};
	[Header("Holders")]
	public GameObject interactableHolder;
	public GameObject nonInteractableHolder;
	public GameObject groundHolder;
	public GameObject gridHolder;
	[HideInInspector]
	public enum BState{selectingBlocks, directingUnits};
	[HideInInspector]
	public BState bState = BState.selectingBlocks;

	public Vector2 startBlock = new Vector2 (1, 3);

	public delegate void DigUpdate();
	public event DigUpdate OnDigUpdate;

	public delegate void OccupiedBlocksUpdate();
	public event OccupiedBlocksUpdate OnOccupiedBlocksUpdate;


	[HideInInspector]
	[SerializeField]
	public GameObject editorBlockAdd;
//	[HideInInspector]
	[SerializeField]
	public GameObject[] editorBlockPrefabs;
//	[HideInInspector]
	[SerializeField]
	public int[] editorBlockStates;

	public enum BlockType{GRASS, MUD, ICE};
	public BlockType blockType;

	void Awake()
	{
//		if(!spawned)
		if(!GameController.instance.blockControllerInstantiated)
			BuildBlocks ();
		else
		{
			PopulateArrays();
		}

//		enemyBarracksBlockPos.width = width;
//		Debug.Log (interactableBlockStates.GetLength (0));
	}


	void PopulateArrays()
	{
		interactableBlockObjs = new GameObject[width, length];
		interactableBlocks = new Block[width, length];
		interactableBlockStates = new int[width, length];
		interactableGrids = new GameObject[width, length];

		for(int i = 0; i< interactableBlockObjs.GetLength(0); i++)
		{
			for(int i2 = 0; i2 < interactableBlockObjs.GetLength(1); i2++)
			{
//				Debug.Log (i *interactableBlockObjs.GetLength(0));
				interactableBlockObjs[i,i2] = interactableBlockObjs1D[(i2*interactableBlockObjs.GetLength(0)) + i];
				interactableBlocks[i,i2] = interactableBlocks1D[(i2*interactableBlocks.GetLength(0)) + i];
				interactableBlockStates[i,i2] = interactableBlockStates1D[(i2*interactableBlockStates.GetLength(0)) + i];
				interactableGrids[i,i2] = interactableGrids1D[(i2*interactableGrids.GetLength(0)) + i];
				if(i < 16 && i != 0) //i != 0 && 
				{
					interactableGrids[i,i2].GetComponent<Renderer>().enabled = true;
				}
				if(i == 1 && interactableBlockStates[i,i2] == 4)
				{
					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
				}
			}
		}
//		for(int i = 0; i< interactableBlockObjs.GetLength(0); i++)
//		{
//			for(int i2 = 0; i2 < interactableBlockObjs.GetLength(1); i2++)
//			{
//
//			}

//		}
		uCon = GameController.instance.unitControls;
	}
	void Start()
	{
		uCon = GameController.instance.unitControls;
		enmSpawning = GameController.instance.enemySpawner;
		if(uCon != null)
		uCon.OnSelectedUnitUpdate += UpdateDestroyedBlockColliders;

		gStats = GameController.instance.gameStats;
		sBlocks = GetComponent<SelectBlocks> ();
//		GridToggle(true);
//		cmPControl = gStats.gameObject.GetComponent<CommandPointsControl> ();
	}
	void OnEnable()
	{
//		uCon = GameController.instance.unitControls;
//		uCon.OnSelectedUnitUpdate += UpdateDestroyedBlockColliders;
	}
	void OnDisable()
	{
		if(uCon != null)
		uCon.OnSelectedUnitUpdate -= UpdateDestroyedBlockColliders;


	}
//	void Update()
//	{
//		for(int i4 = 0; i4 < interactableBlockObjs.GetLength(0); i4++)
//		{
//			intObjs[i4] = interactableBlockObjs[i4,0];
//		}
		//if bstate = selecting blocks, destroyed object colliders should be disabled, and vice versa?
//		if(bState == BState.selectingBlocks)
//		{
//			if(selectedBlocks.Count > 0 && !uCon.diggerOutDigging)
//			{
//				if(GameController.instance != null)
//					GameController.instance.uiControls.digButton.SetActive(true);
//			}
//			else
//			{
//				if(GameController.instance != null)
//					GameController.instance.uiControls.digButton.SetActive(false);
//			}


//			if(interactableBlocks[interactableBlocks.GetLength(0), interactableBlocks.GetLength(1)].GetComponent<Collider>().enabled)
//			{
//			for(int i = 0; i < interactableBlocks.GetLength(0); i++)
//			{
//				for(int i2 = 0; i2 < interactableBlocks.GetLength(1); i2++)
//				{
//					interactableBlocks[i,i2].GetComponent<Collider>().enabled = true;
//				}
//			}
//			}
//		}
//		else
//		{

//			GameController.instance.uiControls.digButton.SetActive(false);
			//IF UNIT IS NOT SELECTED!! AND WHAT ELSE??
//			for(int i = 0; i < interactableBlocks.GetLength(0); i++)
//			{
//				for(int i2 = 0; i2 < interactableBlocks.GetLength(1); i2++)
//				{
//					interactableBlocks[i,i2].GetComponent<Collider>().enabled = false;
//				}
//			}
//		}
//	}

	public void UpdateDestroyedBlockColliders(bool enbl)
	{
		for(int i = 0; i< interactableBlockStates.GetLength(0); i++)
		{
			for(int i2 = 0; i2 < interactableBlockStates.GetLength(1); i2++)
			{
				if(interactableBlockStates[i,i2] == 0)
				{
					if(!enbl)
					{
						interactableBlockObjs[i,i2].GetComponent<Collider>().enabled = false;
					}
					else
					{
						interactableBlockObjs[i,i2].GetComponent<Collider>().enabled = true;
					}
				}
			}

		}
	}

	public void UpdateOccupiedBlocks()
	{
		if (OnOccupiedBlocksUpdate != null)
			OnOccupiedBlocksUpdate ();
	}
	public void AddSelected(Block b)
	{
		selectedBlocks.Add (b);
		switch(blockType)
		{
		case BlockType.GRASS:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockSelectedColorGrass.r, GameController.instance.gameSettings.blockSelectedColorGrass.g, GameController.instance.gameSettings.blockSelectedColorGrass.b, 0);
			break;
		case BlockType.ICE:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockSelectedColorIce.r, GameController.instance.gameSettings.blockSelectedColorIce.g, GameController.instance.gameSettings.blockSelectedColorIce.b, 0);			
			break;
		case BlockType.MUD:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockSelectedColorMud.r, GameController.instance.gameSettings.blockSelectedColorMud.g, GameController.instance.gameSettings.blockSelectedColorMud.b, 0);			
			break;
		}
//		interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockSelectedColor.r, GameController.instance.gameSettings.blockSelectedColor.g, GameController.instance.gameSettings.blockSelectedColor.b, 0);
//		Debug.Log ("langlangpik " + b.width + " " + b + " " + selectedBlocks.Count);

	}
	public void RemoveSelected(Block b)
	{
		selectedBlocks.Remove (b);
		switch(blockType)
		{
		case BlockType.GRASS:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockStartColorGrass.r,GameController.instance.gameSettings.blockStartColorGrass.g, GameController.instance.gameSettings.blockStartColorGrass.b, 0);
			break;
		case BlockType.ICE:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockStartColorIce.r,GameController.instance.gameSettings.blockStartColorIce.g, GameController.instance.gameSettings.blockStartColorIce.b, 0);
			break;
		case BlockType.MUD:
			interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockStartColorMud.r,GameController.instance.gameSettings.blockStartColorMud.g, GameController.instance.gameSettings.blockStartColorMud.b, 0);
			break;
			
		}
//		interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = new Color(GameController.instance.gameSettings.blockStartColorGrass.r,GameController.instance.gameSettings.blockStartColorGrass.g, GameController.instance.gameSettings.blockStartColorGrass.b, 0);
	}

	public void DigBlock()
	{

		if(selectedBlocks.Count > 0 && sBlocks.expectedCost <= gStats.commandPoints)
		{
			gStats.commandPoints -= sBlocks.expectedCost;
			gStats.GetComponent<CommandPointsControl>().UpdateCommandPoints();
			for(int i = 0; i<selectedBlocks.Count; i++)
			{
				toBeDugBlocks.Add(selectedBlocks[i]);
			}
	//		Debug.Log (toBeDugBlocks.Count + " lala " + selectedBlocks.Count);
//			selectedBlocks.Clear ();
	//		StartCoroutine(uCon.SendDigUnit ());
//			uCon.SendDigUnit ();
			GameController.instance.uiControls.digButton.SetActive(false);
		}
		for(int i = 0; i<selectedBlocks.Count; i++)
		{
			interactableBlockStates [selectedBlocks[i].width, selectedBlocks[i].length] = 0;
            StartCoroutine(DigOutBlock(selectedBlocks[i], i*0.55f));
		}
		selectedBlocks.Clear ();
////		StartCoroutine(StartDigUpdate ());

		UpdateBlocks();
	}
	public void UpdateBlocks()
	{
		Resources.UnloadUnusedAssets();
		if(OnDigUpdate != null)
			OnDigUpdate ();
	}
    IEnumerator	StartDigUpdate()
	{
		yield return new WaitForSeconds(1.5f);
//		if(OnDigUpdate != null)
//			OnDigUpdate ();

	}
	IEnumerator DigOutBlock(Block b, float timer) 
	{

		GameObject g = null;
		UpdateDestroyedBlockColliders (false);
		yield return new WaitForSeconds (timer);
		interactableBlockObjs [b.width, b.length].transform.DOMove (interactableBlockObjs [b.width, b.length].transform.position + new Vector3 (0f, -1f, 0f), 1).SetEase(Ease.InQuad);
//		LeanTween.move( interactableBlocks[b.width,b.length], interactableBlocks[b.width,b.length].transform.position + new Vector3(0f, -1f, 0f), 1f).setEase(LeanTweenType.easeInQuad);
		if(GameController.instance.gameSettings.diggingEffect != null)
		{
			if(blockType == BlockType.ICE)
				g = Instantiate(GameController.instance.gameSettings.diggingEffectIce, interactableBlockObjs[b.width, b.length].transform.position, GameController.instance.gameSettings.diggingEffectIce.transform.rotation) as GameObject;
			else
				g = Instantiate(GameController.instance.gameSettings.diggingEffect, interactableBlockObjs[b.width, b.length].transform.position, GameController.instance.gameSettings.diggingEffect.transform.rotation) as GameObject;
		}
		yield return new WaitForSeconds (1);
		Destroy (interactableBlockObjs [b.width, b.length]);
		interactableBlockObjs[b.width,b.length] = Instantiate(GameController.instance.gameSettings.destroyedBlock, new Vector3(b.width,0,b.length), Quaternion.identity) as GameObject;
		interactableBlockObjs[b.width,b.length].transform.SetParent(interactableHolder.transform);
//		UpdateDestroyedBlockColliders (false);
		yield return new WaitForSeconds(5);
		Destroy(g);
		yield break;
//		if(OnDigUpdate != null)
//			OnDigUpdate ();
	}
	public void ChangeState()
	{
//		if (bState == BState.directingUnits)
//		{
//			bState = BState.selectingBlocks;
//			GameController.instance.uiControls.selectedStateB.GetComponentInChildren<Text> ().text = "Selecting Blocks";
//		}
//		else
//		{
//			bState = BState.directingUnits; 
//
//			GameController.instance.uiControls.selectedStateB.GetComponentInChildren<Text> ().text = "Control Units";
//
//		}
	}

	void PlaceBarracks()
	{
		if (!spawned) 
		{
//			barracksBlockPos = new Block{width = barracksBlockPos.width, length = (int)length/2};
			GameObject b = Instantiate (GameController.instance.gameSettings.friendlyBarracks, new Vector3 (barracksBlockPos.width, 0, barracksBlockPos.length), GameController.instance.gameSettings.friendlyBarracks.transform.rotation) as GameObject;
			b.transform.SetParent (this.transform);

			for(int i = 0; i< interactableBlockObjs.GetLength(0); i++)
			{
				for(int i2 = 0; i2 < interactableBlockObjs.GetLength(1); i2++)
				{
					if(i == 1 || i == 0)
					{
						interactableBlockStates[i,i2] = 4;
					}
				}
			}
//			GameObject eB = Instantiate (GameController.instance.gameSettings.enemyBarracks, new Vector3 (width-2, 0, enemyBarracksBlockPos.length), GameController.instance.gameSettings.enemyBarracks.transform.rotation) as GameObject;
//			eB.transform.SetParent (this.transform);
		}
	}
	public void BuildBlocks()
	{
//		Debug.Log ((int)Mathf.Floor(length/2) + " " + Mathf.Floor(length/2));
		barracksBlockPos.length = (int)Mathf.Floor(length/2)-1;

		interactableBlockStates = new int[width, length];
		interactableBlockStates1D = new int[width * length];
		interactableBlocks = new Block[width, length];
		interactableBlocks1D = new Block[width* length];
		interactableBlockObjs = new GameObject[width, length];
		interactableBlockObjs1D = new GameObject[width * length];
		interactableGrids = new GameObject[width, length];
		interactableGrids1D = new GameObject[width * length];
//		intObjs = new GameObject[width];
		for (int b = 0; b < interactableBlockStates.GetLength(0); b++) 
		{
			for(int c = 0; c < interactableBlockStates.GetLength(1); c++) 
			{
				interactableBlockStates[b,c] = 1;
			}
		}
		BuildAround ();

//		interactableBlockStates[(int) startBlock.x, (int)startBlock.y] = 0;



		PlaceBarracks ();
		interactableBlockStates [(int)barracksBlockPos.width, (int)barracksBlockPos.length] = 2;
		interactableBlockStates [(int)barracksBlockPos.width, (int)barracksBlockPos.length+1] = 2;

//		interactableBlockStates [(int)width-2, (int)enemyBarracksBlockPos.length] = 2;
//		interactableBlockStates [(int)width-2, (int)enemyBarracksBlockPos.length + 1] = 2;

//		interactableBlockStates [(int)barracksBlockPos.width+1, (int)barracksBlockPos.length] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width-1, (int)barracksBlockPos.length] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width+1, (int)barracksBlockPos.length+1] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width-1, (int)barracksBlockPos.length+1] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width-1, (int)barracksBlockPos.length+2] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width, (int)barracksBlockPos.length+2] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width+1, (int)barracksBlockPos.length+2] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width+7, (int)barracksBlockPos.length+2] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width+6, (int)barracksBlockPos.length+3] = 4;
//		interactableBlockStates [(int)barracksBlockPos.width+6, (int)barracksBlockPos.length+1] = 4;

//		for(int bT = 0; bT < interactableBlockStates.GetLength(1); bT++)
//		{
//			interactableBlockStates[0, bT] = 5;
//			interactableBlockStates[width-1, bT] = 0;
//		}
		
		interactableBlockStates [(int)barracksBlockPos.width+1, (int)barracksBlockPos.length] = 0;//move new units here
		interactableBlockStates [(int)barracksBlockPos.width+1, (int)barracksBlockPos.length+1] = 0; 
		interactableBlockStates [(int)barracksBlockPos.width+2, (int)barracksBlockPos.length] = 0;//move new units here
		interactableBlockStates [(int)barracksBlockPos.width+2, (int)barracksBlockPos.length+1] = 0; //then here?
		interactableBlockStates [(int)barracksBlockPos.width+2, (int)barracksBlockPos.length+2] = 0;
		interactableBlockStates [(int)barracksBlockPos.width+2, (int)barracksBlockPos.length-1] = 0;
//		interactableBlockStates [(int)barracksBlockPos.width-1, (int)barracksBlockPos.length-1] = 0; //path to back trench


		
		for(int i = 0; i< interactableBlockStates.GetLength(0); i++)
		{
			for(int i2 = 0; i2 < interactableBlockStates.GetLength(1); i2++)
			{


				if(interactableBlockStates[i,i2] == 1)
				{
					if(GameController.instance.gameSettings.standardBlock != null)
					switch(blockType)
					{
						case BlockType.GRASS:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.standardBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
						case BlockType.ICE:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.standardBlockIce, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
						case BlockType.MUD:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.standardBlockMud, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;

					}

					interactableBlockObjs[i,i2].transform.SetParent(interactableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};
//					interactableBlockObjs[i,i2].GetComponent<Renderer>().sharedMaterial.color = GameController.instance.gameSettings.blockStartColor;
//					if(i != 0)
//					{
						interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
						interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
						interactableGrids[i,i2].transform.SetParent(gridHolder.transform);
//					}
//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear; 

					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
				else if(interactableBlockStates[i,i2] == 0)
				{
					interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.destroyedBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
					interactableBlockObjs[i,i2].transform.SetParent(interactableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};
					interactableBlockObjs[i,i2].GetComponent<Collider>().enabled = false;


					interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
					interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
					interactableGrids[i,i2].transform.SetParent(gridHolder.transform);

//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear;
					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
				
				else if(interactableBlockStates[i,i2] == 2)
				{
					interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.destroyedBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
					interactableBlockObjs[i,i2].transform.SetParent(nonInteractableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};

					interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
					interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
					interactableGrids[i,i2].transform.SetParent(gridHolder.transform);
//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear;
					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
				else if(interactableBlockStates[i,i2] == 3)
				{
//					interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
					switch(blockType)
					{
					case BlockType.GRASS:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
					case BlockType.ICE:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlockIce, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
					case BlockType.MUD:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlockMud, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
						
					}
					interactableBlockObjs[i,i2].transform.SetParent(nonInteractableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};

					interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
					interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
					interactableGrids[i,i2].transform.SetParent(gridHolder.transform);
//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear;
					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
				else if(interactableBlockStates[i,i2] == 4)
				{
//					interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonWalkableBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
					switch(blockType)
					{
					case BlockType.GRASS:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonWalkableBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
					case BlockType.ICE:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonWalkableBlockIce, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
					case BlockType.MUD:
						interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.nonWalkableBlockMud, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
						break;
						
					}
					interactableBlockObjs[i,i2].transform.SetParent(nonInteractableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};

					interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
					interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
					interactableGrids[i,i2].transform.SetParent(gridHolder.transform);
//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear;
					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
				else if(interactableBlockStates[i,i2] == 5)
				{
					interactableBlockObjs[i,i2] = Instantiate(GameController.instance.gameSettings.endGameBlock, new Vector3(i,0,i2), Quaternion.identity) as GameObject;
					interactableBlockObjs[i,i2].transform.SetParent(interactableHolder.transform);
					interactableBlocks[i,i2] = new Block{width = i, length = i2, mObj = interactableBlockObjs[i,i2]};
//					interactableBlockObjs[i,i2].GetComponent<Renderer>().sharedMaterial.color = GameController.instance.gameSettings.blockStartColor;
					
					interactableGrids[i,i2] = Instantiate(GameController.instance.gameSettings.gridObj, new Vector3(i, 0.51f, i2), Quaternion.identity) as GameObject;
					interactableGrids[i,i2].transform.rotation = GameController.instance.gameSettings.gridObj.transform.rotation;
					interactableGrids[i,i2].transform.SetParent(gridHolder.transform);
					//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
					//					interactableGrids[i,i2].GetComponent<Renderer>().sharedMaterial.color = Color.clear; 
					
					interactableBlockObjs1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockObjs[i,i2];
					interactableBlocks1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlocks[i,i2];
					interactableBlockStates1D[i2*interactableBlockStates.GetLength(0)+i] = interactableBlockStates[i,i2];
					interactableGrids1D[i2*interactableBlockStates.GetLength(0)+i] = interactableGrids[i,i2];
				}
			}
		}





	}

	void BuildAround()
	{
		nonInteractableBlockStates = new int[width + extraGroundWidth, length + extraGroundLength];
		nonInteractableBlockObjects = new GameObject[width + extraGroundWidth, length + extraGroundLength];

		for(int i = 0; i< nonInteractableBlockStates.GetLength(0); i++)
		{
			for(int i2 = 0; i2< nonInteractableBlockStates.GetLength(1); i2++)
			{
				//groundblockss
				int iGroundWidth = i-extraGroundWidth/2;
				int i2GroundLength = i2-extraGroundLength/2;

				if(GameController.instance.gameSettings.groundBlock != null)
				{
					GameObject groundBlock = null;
//					groundBlock = Instantiate(GameController.instance.gameSettings.groundBlock, new Vector3(iGroundWidth,-GameController.instance.gameSettings.groundHeight,i2GroundLength), Quaternion.identity) as GameObject;
					switch(blockType)
					{
					case BlockType.GRASS:
						groundBlock = Instantiate(GameController.instance.gameSettings.groundBlock, new Vector3(iGroundWidth,-GameController.instance.gameSettings.groundHeight,i2GroundLength), Quaternion.identity) as GameObject;
						break;
					case BlockType.ICE:
						groundBlock = Instantiate(GameController.instance.gameSettings.groundBlockIce, new Vector3(iGroundWidth,-GameController.instance.gameSettings.groundHeight,i2GroundLength), Quaternion.identity) as GameObject;
						break;
					case BlockType.MUD:
						groundBlock = Instantiate(GameController.instance.gameSettings.groundBlockMud, new Vector3(iGroundWidth,-GameController.instance.gameSettings.groundHeight,i2GroundLength), Quaternion.identity) as GameObject;
						break;
						
					}
					groundBlock.transform.SetParent(groundHolder.transform);
				}

				if(iGroundWidth >= width || iGroundWidth<= -1 || i2GroundLength >= length || i2GroundLength <= -1)
				{
					GameObject nonIntBlock = null;
//					nonIntBlock = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlock, new Vector3(iGroundWidth,0,i2GroundLength), Quaternion.identity) as GameObject;
					switch(blockType)
					{
					case BlockType.GRASS:
						nonIntBlock = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlock,new Vector3(iGroundWidth,0,i2GroundLength), Quaternion.identity) as GameObject;
						break;
					case BlockType.ICE:
						nonIntBlock = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlockIce, new Vector3(iGroundWidth,0,i2GroundLength), Quaternion.identity) as GameObject;
						break;
					case BlockType.MUD:
						nonIntBlock = Instantiate(GameController.instance.gameSettings.nonInteractableStandardBlockMud, new Vector3(iGroundWidth,0,i2GroundLength), Quaternion.identity) as GameObject;
						break;
						
					}
					nonIntBlock.transform.SetParent(nonInteractableHolder.transform);
					nonInteractableBlockObjects[i,i2] = nonIntBlock;

					if(iGroundWidth == width || iGroundWidth == -1 || i2GroundLength == length || i2GroundLength == -1)
					{
						nonIntBlock.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					}
				}


				
			}
		}
	}

	public void GridToggle(bool on)
	{
		for(int i = 0; i < interactableGrids.GetLength(0); i++)
		{
			for(int i2 = 0; i2 < interactableGrids.GetLength(1); i2++)
			{
				if(on)
				{
					if(i != 0)
					{
						interactableGrids[i,i2].GetComponent<Renderer>().enabled = true;
						interactableGrids[i,i2].GetComponent<Renderer>().material.color = Color.white;
//						if(interactableBlockStates[i,i2] == 0)
//						{
//							interactableGrids[i,i2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.gridColorWalkable;
//						}
//						foreach(Block b in unitOccupiedBlocks)
//						{
//							if(b.width == i && b.length == i2)
//							{
//								interactableGrids[i,i2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.gridColorOccupied;
//							}
//						}
					}
//					foreach(Block b in int)
//					{
//						if(b.width == i && b.length == i2)
//						{
//							interactableGrids[i,i2].GetComponent<Renderer>().material.color = Color.red;
//						}
//					}
				}
				else
				{
//					interactableGrids[i,i2].GetComponent<Renderer>().enabled = false;
					interactableGrids[i,i2].GetComponent<Renderer>().material.color = GameController.instance.gameSettings.gridColorStandard;
				
				}

			}
		}
//		foreach(GameObject g in interactableGrids)
//		{
//			if(on)
//			{
//				foreach(Block b in unitOccupiedBlocks)
//				{
////					if(b.width == g.
//				}
//
//				g.GetComponent<Renderer>().material.color = Color.white;
//			}
//			else
//			{
//				g.GetComponent<Renderer>().material.color = Color.clear;
//			}
//		}
	}

	void OnDrawGizmos()
	{
		for(int i = 0; i < interactableBlockStates1D.Length; i++)
		{
			if(interactableBlockStates1D[i] == 4)
			{
				Gizmos.DrawWireCube(interactableBlockObjs1D[i].transform.position + new Vector3(0,0.5f,0), Vector3.one);
//				#if UNITY_EDITOR
//				UnityEditor.Handles.Label(interactableBlockObjs1D[i].transform.position + new Vector3(-0.5f,1.5f,0), "!walk !dig");
//				#endif
			}
			if(interactableBlockStates1D[i] == 3)
			{
				Gizmos.DrawWireSphere(interactableBlockObjs1D[i].transform.position + new Vector3(0,0.5f,0), 0.5f);
//				Gizmos.draw
//				Handles.
//				#if UNITY_EDITOR
//				UnityEditor.Handles.Label(interactableBlockObjs1D[i].transform.position + new Vector3(-0.5f,1.5f,0), "walk !dig");
//				#endif
			}
			if(interactableBlockStates1D[i] == 1)
			{
				#if UNITY_EDITOR
				if(interactableBlockObjs1D[i] != null)
				{
					UnityEditor.Handles.Label(interactableBlockObjs1D[i].transform.position + new Vector3(0,0.75f,0), "1");
				}
				#endif
			}
		}
	}

}
