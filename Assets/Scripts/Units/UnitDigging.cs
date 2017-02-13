using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
public class UnitDigging : MonoBehaviour {
	public Block currentDiggingBlock;
	public List<Block> blocksToDig;
	private UnitController uCon;
	private GameObject uConObj;
	private BlockControl bControl;
	private UnitMovement uMove;

//	private BlockControl bControl;
	void Awake() {
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		uConObj = GameController.instance.unitControllerObj;
		uMove = GetComponent<UnitMovement> ();

		uCon = GameController.instance.unitControls;
		blocksToDig = bControl.toBeDugBlocks;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator Digging()
	{

//		Debug.Log ("BGE");


		yield return new WaitForSeconds (0.1f);

		Block b = new Block{width = (int)this.transform.position.x, length = (int)this.transform.position.z};
		uMove.digging = true;
		uMove.movementMode = UnitMovement.MovementMode.DIGGING;
//		Debug.Log ("SSEND DIG UNIT");
//		foreach(Block blo in blocksToDig)
//		{
		for(int c = 0; c < blocksToDig.Count; c++)
		{
			for(int i = blocksToDig[c].width-1; i <=blocksToDig[c].width+1; i++)
			{
				for(int i2 = blocksToDig[c].length-1; i2 <= blocksToDig[c].length+1; i2++)
				{
					if(blocksToDig[c].width == i || blocksToDig[c].length == i2)//non diagonal
					{
						if(i >= 0 && i2 >=0 ) //  && i <= pBlock.width-1 && i2 <= pBlock.length
						{
							if(i < bControl.interactableBlockStates.GetLength(0) && i2 < bControl.interactableBlockStates.GetLength(1))							
							{
								if(bControl.interactableBlockStates[i, i2] == 0)
								{
									currentDiggingBlock = new Block{width = blocksToDig[c].width, length = blocksToDig[c].length};

									if(b.width == i && b.length == i2)
									{
										ReachedBlock();
									}
									else
									{
										GetComponent<UnitMovement>().MoveOnPath(this.transform, uConObj.GetComponent<FindPath>().Patherize(b, new Block{width = i, length = i2}, new int[1]{0}, 0), false);
									}
								}

							}

						}
					}
				}
			}
		}
//			
	}

	public void ReachedBlock()
	{
		StartCoroutine(DigOutBlock (currentDiggingBlock, 0.1f));
//		Debug.Log ("BGUE");
	}
	IEnumerator DigOutBlock(Block b, float timer) 
	{
		if(GameController.instance.gameSettings.diggingEffect != null)
		{
			Instantiate(GameController.instance.gameSettings.diggingEffect, bControl.interactableBlockObjs[b.width, b.length].transform.position, GameController.instance.gameSettings.diggingEffect.transform.rotation);
		}
//		yield return new WaitForSeconds (timer);
		if(uCon.movingStates != UnitController.MovingStates.CHOSING_END && !uCon.selectBlocksShow)
		{
			bControl.UpdateDestroyedBlockColliders (false);
		}
		this.transform.DOLookAt (bControl.interactableBlockObjs [b.width, b.length].transform.position, 0.1f);

		bControl.interactableBlockObjs [b.width, b.length].transform.DOMove (bControl.interactableBlockObjs [b.width, b.length].transform.position + new Vector3 (0f, -1f, 0f), 1).SetEase(Ease.InQuad);
		yield return new WaitForSeconds (1);
		Destroy (bControl.interactableBlockObjs [b.width, b.length]);
		bControl.interactableBlockObjs[b.width,b.length] = Instantiate(GameController.instance.gameSettings.destroyedBlock, new Vector3(b.width,0,b.length), Quaternion.identity) as GameObject;
		bControl.interactableBlockObjs[b.width,b.length].transform.SetParent(bControl.interactableHolder.transform);
		bControl.interactableBlockStates [currentDiggingBlock.width, currentDiggingBlock.length] = 0;
		bControl.interactableBlockStates1D[b.length * bControl.interactableBlockStates.GetLength(0) + b.width] = 0;
		//		i2*interactableBlockStates.GetLength(0)+i
//		if(uCon.movingStates == UnitController.MovingStates.CHOSING_END)
//		{
//			bControl.interactableBlockObjs[b.width,b.length].GetComponent<BoxCollider>().enabled = true;
//		}
		blocksToDig.RemoveAt (0);
		if(blocksToDig.Count > 0)
		{
			StartCoroutine(Digging());
		}
		else
		{
			GetComponent<UnitMovement>().MoveOnPath(this.transform, uConObj.GetComponent<FindPath>().Patherize(new Block{width = (int)this.transform.position.x, length = (int)this.transform.position.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, 0), false);
			uMove.doneDigging = true;
		}
		bControl.UpdateBlocks ();
		//		UpdateDestroyedBlockColliders (false); bControl.barracksBlockPos.width, 0, bControl.barracksBlockPos.length - 1
		//		if(OnDigUpdate != null)
		//			OnDigUpdate ();
	}

}
