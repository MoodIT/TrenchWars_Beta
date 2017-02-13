using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
public class SelectBlocks : MonoBehaviour {

	private BlockControl bControl;
	private UnitController uCon;
	private GameStats gStats;
	private CommandPointsControl cmPControl;
	[HideInInspector]
	public Text digCostText;
	[HideInInspector]
	public bool dragging;
	private GameObject hitObj;

	[HideInInspector]
	public float expectedCost = 0;
	int interactBlockLength0;
	int interactBlockLength1;
	void Start()
	{
		uCon = GameController.instance.unitControls;
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		interactBlockLength0 = bControl.interactableBlockStates.GetLength(0);
		interactBlockLength1 = bControl.interactableBlockStates.GetLength(1);
		cmPControl = GameController.instance.gameStatsObj.GetComponent<CommandPointsControl> ();
		gStats = GameController.instance.gameStats;
	}

	public void ResetSelectedBlocks()
	{
		foreach(Block b in bControl.selectedBlocks) 
		{
			switch(bControl.blockType)
			{
			case BlockControl.BlockType.GRASS:
				bControl.interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = GameController.instance.gameSettings.blockStartColorGrass;
				break;
			case BlockControl.BlockType.ICE:
				bControl.interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = GameController.instance.gameSettings.blockStartColorIce;
				break;
			case BlockControl.BlockType.MUD:
				bControl.interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = GameController.instance.gameSettings.blockStartColorMud;
				break;
				
			}
//			bControl.interactableBlockObjs [b.width, b.length].GetComponent<Renderer> ().material.color = GameController.instance.gameSettings.blockStartColorGrass;
		}
		expectedCost = 0;
		bControl.selectedBlocks.Clear();
		cmPControl.UpdateCommandPoints();
	}

	void ClickedOnButtonCheck()
	{
		bool t = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
		if(t) return;
		if(uCon.movingStates != UnitController.MovingStates.SELECTING_UNIT) return;
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) 
		{
			OnGameObject();
		}
		else
		{
			dragging = true;
//			GameController.instance.uiControls.digButton.SetActive(true);
		}
		if(!dragging) return;

		ResetSelectedBlocks();
	}

	void OnGameObject()
	{
		if (IsPointerOverGameObject (Input.GetTouch (0).fingerId)) {
			Debug.Log("Hit UI, Ignore Touch");
		} else {
			Debug.Log("Handle Touch");
			dragging = true;
//			GameController.instance.uiControls.digButton.SetActive(true);
		}
	}

	void RaycastCheck()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Block pBlock = null;
		if (Physics.Raycast(ray, out hit, 100))
		{
			hitObj = hit.collider.gameObject;
			InteractBlockCheck();
		}
	}
	void InteractBlockCheck()
	{
		Block pBlock = null;
//		var matches = from item in bControl.interactableBlockObjs
//			where criteria.All(criterion=>criterion.IsMetBy(item))
//				select item;
//		match = matches.FirstOrDefault();

		for(int i = 0; i< interactBlockLength0; i++)
		{

			for(int i2 = 0; i2< interactBlockLength1; i2++)
			{
				if(bControl.interactableBlockObjs[i,i2] == hitObj)
				{
					pBlock = bControl.interactableBlocks[i,i2];
				}
			}
		}
		if(pBlock == null) return;

		BlockSelectionCheck(pBlock);

	}

	void FromWhereSelect(Block pBlock, int p, int p2)
	{

//		var stages = (from p in 
	}
	void BlockSelectionCheck(Block pBlock)
	{
//		var matches = from item in items
//			where criteria.All(criterion=>criterion.IsMetBy(item))
//				select item;
//		match = matches.FirstOrDefault();
//		int[] tWidth = new int[3]{pBlock.width-1, pBlock.width, pBlock.width+1};
//		IEnumerable<int> t = from tW in tWidth
//				where tW >= 0
//				where tW < interactBlockLength0
//				where tW == pBlock.width
//				select tW;
//		int p = t.FirstOrDefault();


		for(int p = pBlock.width-1; p<= pBlock.width+1; p++)
		{
			for(int p2 = pBlock.length-1; p2<= pBlock.length+1; p2++)
			{
				if(p < 0 ) continue; // not outside on one end
				if(p2 < 0) continue; 
				if(p >= interactBlockLength0) continue;
				if(p2 >= interactBlockLength1) continue;

				if(p != pBlock.width && p2 != pBlock.length)continue; //not diagonal

				if(bControl.interactableBlockStates[p, p2] != 0 && !bControl.selectedBlocks.Contains(bControl.interactableBlocks[p,p2]) ) continue;

				if(bControl.interactableBlockStates[pBlock.width,pBlock.length] == 4) continue;
				if(bControl.interactableBlockStates[pBlock.width,pBlock.length] == 0) continue;
				if(bControl.interactableBlockStates[pBlock.width,pBlock.length] == 2) continue;
				if(bControl.interactableBlockStates[pBlock.width,pBlock.length] == 3) continue;
				if(bControl.selectedBlocks.Contains(bControl.interactableBlocks[pBlock.width, pBlock.length])) continue;

				if((bControl.selectedBlocks.Count+1) * GameController.instance.gameSettings.blockCost > gStats.commandPoints) continue;
				if(uCon.movingStates == UnitController.MovingStates.SELECTING_UNIT && pBlock.mWidth != 0)
					{
						bControl.AddSelected(bControl.interactableBlocks[pBlock.width, pBlock.length]);
						expectedCost += GameController.instance.gameSettings.blockCost;
						cmPControl.UpdateCommandPoints();
						return;
					}
			}
		}
	}

	void InsideBlocksArea()
	{

	}
	void Update () 
	{
		//for mobile must implement
			       
		bool t = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (); // works on tablet? id might need to be -1 or 1 or 0 ?

		if(Input.GetMouseButtonDown(0)) ClickedOnButtonCheck();

		if(bControl.selectedBlocks.Count < 1)
		{
			GameController.instance.uiControls.digButton.SetActive(false);
		}

		if(dragging && Input.GetMouseButtonUp(0))
		{
			dragging = false;
			if(bControl.selectedBlocks.Count > 0)
			{
				GameController.instance.uiControls.digButton.SetActive(true);
				GameController.instance.uiControls.digButton.GetComponentInChildren<Text>().text = ""+ bControl.selectedBlocks.Count * GameController.instance.gameSettings.blockCost;
				Vector3 screenPos = Camera.main.WorldToScreenPoint (bControl.interactableBlockObjs[bControl.selectedBlocks[bControl.selectedBlocks.Count-1].width, bControl.selectedBlocks[bControl.selectedBlocks.Count-1].length].transform.position) ;
				GameController.instance.uiControls.digButton.transform.position = new Vector3(screenPos.x, screenPos.y+70, 0);
			}
		}
//		if(dragging && Input.GetMouseButtonUp(0) && )
//		{
//			GameController.instance.uiControls.digButton.SetActive(true);
//		}


		if(dragging && !Camera.main.GetComponent<CameraControl>().movingCam)
		{
			RaycastCheck();
		}
	}







	bool IsPointerOverGameObject( int fingerId )
	{
		EventSystem eventSystem = EventSystem.current;
		return ( eventSystem.IsPointerOverGameObject( fingerId )
		        && eventSystem.currentSelectedGameObject != null );
	}
	void UpdateDigCost()
	{
		digCostText = GameController.instance.uiControls.digButton.transform.Find("Cost").GetComponent<Text>();

		digCostText.text = "Cost: " + expectedCost;

	}

}
