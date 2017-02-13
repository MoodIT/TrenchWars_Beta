using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Flag : MonoBehaviour {
	private BlockControl bControl;
	private GameStats gStats;
	private ObjectiveController objectiveCon;
	//check for adjacent blocks on dig update
	//if there is a block that is digged(0) on an adjacent block, activate this flag(effect)
	//if player clicks on it(swipe eventually?) 
	private bool active;
	[HideInInspector]
	public bool nearbyDug;
	public GameObject activateObject;

	private bool latent = true;

	[HideInInspector]
	public List<Block> nearbyDugBlocks;

	public bool optional;
	public int flagNrInOrder;
	public GameObject flagObj;
	void Start () 
	{
		gStats = GameController.instance.gameStats;
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		bControl.OnDigUpdate += WhenBlockIsDug;
		objectiveCon = GameController.instance.objectiveController;
//		bControl.OnOccupiedBlocksUpdate += CheckJumpOrShoot ;
	}


//	void OnEnable()
//	{
//		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
//		bControl.OnDigUpdate += WhenBlockIsDug;
//	}

	void OnDisable()
	{
		bControl.OnDigUpdate -= WhenBlockIsDug ;
	}

	void WhenBlockIsDug()
	{
		StartCoroutine(WaitTime());
	}
	void CheckForDug()
	{
		if(!active && latent)
		{
			int x = (int)this.transform.parent.position.x;
			int z = (int)this.transform.parent.position.z;
			
			for(int i = x-1; i <=  x+1; i++)
			{
				for(int i2 = z-1; i2 <=  z+1; i2++)
				{
					if(i < 0 || i2 < 0 )  continue; 				
					if(i >= bControl.interactableBlockStates.GetLength(0) || i2 >= bControl.interactableBlockStates.GetLength(1)) continue;
					
					if(bControl.interactableBlockStates[i,i2]!= 0) continue; //not dug out
					if(i != x && i2 != z) continue; //not on same lane
//					Activate ();
					nearbyDug = true;
					nearbyDugBlocks.Add(bControl.interactableBlocks[i,i2]);
					break;
				}
			}
		}
	}
	IEnumerator WaitTime()
	{
		yield return new WaitForSeconds(1);
		CheckForDug();
	}
	void Activate()
	{
		active = true;
		activateObject.SetActive(true);
		GetComponent<CapsuleCollider>().enabled = true;
	}
	void Deactivate(bool sucess)
	{

		active = false;
		activateObject.SetActive(false);
		GetComponent<CapsuleCollider>().enabled = false;

		if(flagObj != null)
		{
			flagObj.GetComponent<Renderer>().material.color = GameController.instance.gameSettings.flagTakeOverColor;
		}
		if(sucess && !gStats.gameComplete)
		{
			gStats.WinGame();
//			latent = false;
		}
	}
	void OnMouseDown()
	{
		if(active && latent && objectiveCon.completedFlags == objectiveCon.flagCount-1)
		{
			Deactivate(true);
		}
		else if( active && latent)
		{
			Deactivate(false);
			objectiveCon.completedFlags++;
			latent = false;
		}
	}

	public void MovementUpdate()
	{
//		for(int i = 0; i < nearbyDugBlocks; i++)
//		{
//			if(bControl.unitOccupiedBlocks[b.mWidth,b.mLength] != null)

//		}
		if(latent)
		{
			foreach(Block b in nearbyDugBlocks)
			{
				foreach(Block b2 in bControl.unitOccupiedBlocks)
				{
					if(b2.mWidth == b.mWidth && b2.mLength == b.mLength)
					{
	//					Debug.Log ("IOEUIOETUNLOPOPIO");
						Activate ();
						return;
					}



				}
	//			Debug.Log (b.mWidth + " " + b.length);
	//			if(bControl.unitOccupiedBlocks[b.mWidth,b.mLength] != null)
	//			{
	//				Debug.Log ("gursguos");
	//			}
			}
//			Debug.Log ("FLAG CLICKED?/()57634");
//			Deactivate(false);
		}
	}
//	void Update ()
//	{
//	
//	}
}
