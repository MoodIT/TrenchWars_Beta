using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
[System.Serializable]
public class FindPath : MonoBehaviour {

	private BlockControl blockControls;

	public Vector3 startPos = new Vector3(1f, 0f, 3f);
	public Vector3 pos;
	public int[] t = new int[2] {1,3};

	public List<int[,]> pathList = new List<int[,]>();

//	public int pathVars;
	public List<Block> reachableBlocks = new List<Block>();
	public List<Block> newReachableBlocks = new List<Block> ();
	public List<Block> exploredBlocks = new List<Block>();

	public List<Block> pathBlocks = new List<Block> ();
	public Block destinationBlock;
	List<Block> path = new List<Block> ();
	public Vector3[] pathV = new Vector3[0];
	public Vector3[] pathV3 = new Vector3[0];
//	public 
	public Vector2 destinationBlockV = new Vector2 (4, 5);

	private UnitController unitController;
	private UnitMovement unitMove;
//	public class PrevBlock
//	{
//		public int width{ get; set;}
//		public int length{get; set;}
//	}
	void Start () 
	{
		destinationBlock = new Block{width = (int)destinationBlockV.x, length = (int)destinationBlockV.y};
		blockControls = GameController.instance.blockController.GetComponent<BlockControl>();
//		unitController = GameController.instance.unitControls;unitMove = GameController.instance.u
//		pos = blockControls.interactableBlockObjs [(int)startPos.x, (int)startPos.z].transform.position;
//		transform.position = new Vector3(pos.x, 0, pos.z);

//		Patherize (new Block{width = 1, length = 3}, destinationBlock);
//		Move (unitController.units[0]);

	}
	void Update()
	{		
		if(path != null)
		{
			for(int i = 0; i< path.Count-1; i++)
			{
				Debug.DrawLine (blockControls.interactableBlockObjs [path [i].width, path [i].length].transform.position, blockControls.interactableBlockObjs [path [i+1].width, path [i+1].length].transform.position);
			}
		}
//		
	}
//	void Move(GameObject obj)
//	{
//		obj.transform.DOPath (pathV, 2).SetEase(Ease.Linear).SetSpeedBased();
//	}
	void OnDrawGizmos()
	{
//		Gizmos.DrawCube (blockControls.interactableBlocks [path [0].width, path [0].length].transform.position, Vector3.one);
	}
	public Vector3[] Patherize(Block i, Block i2, int[] blockTypes, float height)
	{	
		ClearLists ();
		path = CalcPath (i, i2, blockTypes);
//		Debug.Log (path);
		if(path == null)
		{
			Debug.Log ("path is null" + i.width + " " + i.length + " " + i2.width + " " + i2.length);
			return null;
		}
		else
		{
			pathV = new Vector3[path.Count];
//			Debug.Log ("moving to point" + i.width + " " + i.length + " " + i2.width + " " + i2.length);
			for(int pI = 0; pI< path.Count;pI++)
			{
					pathV[pI] = new Vector3(path[pI].width, height, path[pI].length);
			}
			System.Array.Reverse (pathV);
			return pathV;
		}
	}
	void ClearLists()
	{
		if (path != null) 
		{
			path.Clear ();
		}
		exploredBlocks.Clear ();
		reachableBlocks.Clear ();
	}
	List<Block> CalcPath(Block startBlock, Block endBlock, int[] blockTypes)
	{
		reachableBlocks.Add (startBlock);
//		destinationBlock = new Block{width = 4, length = 5};
//		CheckSurroundingBlocks (new Block{width = 1, length = 3});
		//for each block in considered blocks find the nearest to the destination and add it to closed

		//stop when destinatin block is in closed or consideredblocks is empty
		bool on = true;
		while(reachableBlocks.Count > 0)
		{

			Block block = ChooseBlock(startBlock);// change to find the nearest to destination
//			if(!reachableBlocks.Contains(destinationBlock))
//			{
//			Debug.Log (block.width + " " + block.length);
			if(block.width == endBlock.width && block.length == endBlock.length)
			{
//				Debug.Log (block.prevBlock.width);
				endBlock.prevBlock = block.prevBlock;
				return BuildPath(endBlock);
			}
				//ChooseNode;

				//find block to check and see if its the same as destination block
				//if is is build path
				//if not:
				// Don't repeat ourselves.
			reachableBlocks.Remove(block);
			exploredBlocks.Add(block);
//				Debug.Log (block.width + " " + block.length);
			newReachableBlocks = GetAdjacentBlocks(block, blockTypes); 
			List<Block> toRemove = new List<Block>();
			foreach(Block b in newReachableBlocks)
			{ 
//				Debug.Log (b.width + " " + b.length);
				foreach(Block b2 in exploredBlocks)
				{
					if(b.width == b2.width && b.length == b2.length)
					{
//						Debug.Log("BEUG");
//						newReachableBlocks.Remove(b);
						toRemove.Add(b);
					}
				}
//					if(exploredBlocks.Contains(b))
//					{
//						newReachableBlocks.Remove(b);
//					}
			}
			foreach(Block r in toRemove)
			{
				if(newReachableBlocks.Contains(r))
				{
//					Debug.Log ("BYEGGEUI");
					newReachableBlocks.Remove(r);
				}
			}//minus explored
			toRemove.Clear();
//			Debug.Log(newReachableBlocks.Count);
			foreach(Block adjacent in newReachableBlocks)
			{
//				Debug.Log (block.cost +1);
//				Debug.Log(newReachableBlocks.Count);
//				Debug.Log(block.length + " " + block.width);
				if(!reachableBlocks.Contains(adjacent))
				{
//					Debug.Log (adjacent.length + " " + adjacent.width);
					reachableBlocks.Add(adjacent);
//					adjacent.prevBlock = block;
//					Debug.Log (adjacent.prevBlock.width + " " + adjacent.prevBlock.length);

//					adjacent.cost = block.cost +1;
//					Debug.Log (blockControls.interactableBlocks[adjacent.width, adjacent.length].transform.position);
				}
				if(block.cost +1 > adjacent.cost)
				{
					adjacent.prevBlock = block;
					adjacent.prevBlock.length = block.length;
					adjacent.cost = block.cost +1;
//					Debug.Log (adjacent.cost);
				}
			}


		}
		return null;
//				CheckSurroundingBlocks(t[0], t[1]);

//		Debug.Log (CheckSurroundingBlocks(new Vector2(pos.x, pos.y)));
//		blockControls.interactableBlocks[startPos.x, startPos.y].transform.position;
	}

	List<Block> BuildPath(Block destinationBlock)
	{
		List<Block> path = new List<Block> ();
		Block b = destinationBlock;
		while(b != null)
		{
				path.Add(b);
				b = b.prevBlock;

		}

		return path; 
	}
	Block ChooseBlock(Block b)
	{
//		function choose_node (reachable):
		Block bestBlock = null;
		float minCost = Mathf.Infinity;
		float multip = 1.1f;
//			best_node = None
		foreach(Block b2 in reachableBlocks)
		{
//			Debug.Log(b2.cost);
//			Debug.Log (blockControls.interactableBlocks[b2.width, b2.length].transform.position);
			int costStartToBlock = b2.cost;
//			if(bestBlock != null && b2.mLength == bestBlock.mLength)
//			{
//				multip = 5;
//			}
			float costBlockToGoal = multip*(Mathf.Abs(b2.width - destinationBlock.width) + Mathf.Abs(b2.length-destinationBlock.length));
//			int costBlockToGoal = multip*Mathf.Abs(b2.length-destinationBlock.length)); //(Mathf.Abs(b2.width - destinationBlock.width) + 
//			int costBlockToGoal = Vector3.Distance(blockControls.interactableBlocks[b2.width, b2.length], blockControls.interactableBlocks[desti
			float totaltCost = costStartToBlock + costBlockToGoal;
//			if(bestBlock != null && b2.mLength == bestBlock.mLength && b2.mLength > bestBlock.mLength)//
//			{
//				Debug.Log (b2.mLength + " " + bestBlock.mLength);
//				bestBlock = b2;
//			}

			if(minCost > totaltCost)
			{
				minCost = totaltCost;
				bestBlock = b2;
			}
		}
		return bestBlock;
				
	}
	List<Block> GetAdjacentBlocks(Block pBlock, int[] blockTypes)
	{
		List<Block> blocks = new List<Block> ();
		for (int i = pBlock.width -1; i <= pBlock.width +1; i++) 
		{
			for (int i2 = pBlock.length -1; i2 <= pBlock.length+1; i2++) 
			{
				if(pBlock.width == i || pBlock.length == i2)
				{
					if(blockControls.interactableBlockStates[i,i2] != null)
					{	
						if(i >= 0 && i2 >=0 ) //  && i <= pBlock.width-1 && i2 <= pBlock.length
						{
							if(i < blockControls.interactableBlockStates.GetLength(0) && i2 < blockControls.interactableBlockStates.GetLength(1))
							{
								foreach(int iT in blockTypes)
								{
									if(blockControls.interactableBlockStates[i,i2] == iT)
									{
										blocks.Add(new Block{width = i, length = i2});
									}
								}
							}
						}
					}
//					else
//					{
//						Debug.Log ("BUE");
//					}
				}
			}
		}
		return blocks;
	}

}
