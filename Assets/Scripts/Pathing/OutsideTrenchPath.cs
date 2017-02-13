using UnityEngine;
using System.Collections;

public class OutsideTrenchPath : MonoBehaviour {
	BlockControl bControl;
	FindPath fPath;
//	Block curBlockG;
	bool unitMoving;
	void Start()
	{
		fPath = GetComponent<FindPath>();
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
	}

	public Vector3[] FindPath(int[]bTypes, int level, int cLane, Block curBlock, bool unit)
	{
//		unitMoving = unit;
//		curBlockG = curBlock;
//		int lane = cLane;
		int blockLane = cLane;
//		int closest = 0;
		Block targetBlock = new Block{};
//		int checkLane = cLane;
//
//		int i = lane;
//		int addInt = 0;
		
//		if(unit)
//			addInt = -1;
//		else
//			addInt = 1;
//
//		int laneEnd = 0;
//
//		if(unit)
//			laneEnd = bControl.width-1;

//		Debug.Log (laneEnd);
//		while (i>= 0 && i < bControl.length) 
//		{ 
//
//
//			int closest0 = FindClosest(i, 0, unit, curBlock);
//			int closest1 = FindClosest(i, 1, unit, curBlock);
//			int closest2 = FindClosest(i, 2, unit, curBlock);
//			int closest4 = FindClosest(i, 4, unit, curBlock);
//			if(FindClosest(i, 2, unit, curBlock) == laneEnd && FindClosest(i, 0, unit, curBlock) == laneEnd &&  FindClosest(i, 2, unit, curBlock) == laneEnd)
//			{
////				Debug.Log (closest0+addInt);
//				closest = (FindClosest(i, 0, unit, curBlock)+addInt);
//				blockLane = i;
//				break;
//			}
////			else if (closest0 > closest4 && closest0 > closest2) 
////			{
////				closest = (closest0 + addInt);
////				blockLane = i;
////				break;
////			}
////			else if (closest4 > closest0 && closest4 > closest2) 
////			{
////				//					
////				//					Debug.Log (FindClosest(i,4));
////				//					for(int b = (FindClosest(i, 4)+1); b< bControl.width; b++)
////				//					{
////				//						Debug.Log (FindClosest(i,4)+b-FindClosest(i, 4));
////				//						if(bControl.interactableBlockStates[FindClosest(i,4)+b-FindClosest(i, 4) , i-1] == 0)
////				//                        {
////				//							closest = (FindClosest(i, 4)+1);
////				//							blockLane = i;
////				//							break;
////				//                        }
////				//					}
////				if (bControl.interactableBlockStates [closest4 + addInt, i - 1] == laneEnd) 
////				{ // do so it checks all fromcurrent block
////					closest = (closest4 + addInt);
////					blockLane = i;
////					break;
////				}
////				if (bControl.interactableBlockStates [closest4 + addInt*2, i - 1] == laneEnd) 
////				{ // do so it checks all fromcurrent block
////					closest = (closest4 + addInt*2);
////					blockLane = i;
////					break;
////				}
////				//					if(bControl.interactableBlockStates[FindClosest(i,4)+2, i-1] == 0) // do so it checks all fromcurrent block
////				//					{
////				//						closest = (FindClosest(i, 4)+1);
////				//						blockLane = i;
////				//						break;
////				//					}
////				//					if(bControl.interactableBlockStates[FindClosest(i,4)+3, i-1] == 0) // do so it checks all fromcurrent block
////				//					{
////				//						closest = (FindClosest(i, 4)+1);
////				//						blockLane = i;
////				//						break;
////				//					}
////			}
////			
////			
////			else if(closest2 > closest4 && closest2 > closest0)
////			{
////				closest = (closest2 + addInt);
////				blockLane = i;
////				break;
////			}
//			
//			
//			checkLane++;
//			if(lane > bControl.barracksBlockPos.length)
//			{
//				i--;
//			}
//            else
//            {
//                i++;
//            }	
//		}
//		
		
		targetBlock = new Block{width = 14 , length = blockLane, mWidth = 14, mLength =  blockLane};
		//		return closest;
		//		Debug.Log (targetBlock.width + " " + targetBlock.length);
		return fPath.Patherize(curBlock, targetBlock, bTypes, level);
		//		return null;
	}


	int FindClosest(int currentLane, int type, bool unit, Block curBlockG)
	{
		int closestStopBlock = 0;
		if(unit)
			closestStopBlock = bControl.width-1;
		//		Debug.Log (currentLane);
		for(int i = 0; i< bControl.interactableBlockStates.GetLength(0); i++)
		{
			for(int i2 = 0; i2< bControl.interactableBlockStates.GetLength(1); i2++)
			{
				if(i2 == currentLane) // looping through all blocks on current lane
				{
					 if(bControl.interactableBlockStates[i,i2] == type) 
					{
//						Debug.Log (i);
						if(!unit)
						{
							if(i < curBlockG.width)
							{
								int tempClosest = i;
								if(tempClosest > closestStopBlock) // er den tættere på end alle andre stop blocke
								{
									closestStopBlock = i;
								}
							}
						}
						else
						{
							if(i > curBlockG.width)
							{
								int tempClosest = i;
								if(tempClosest < closestStopBlock) // er den tættere på end alle andre stop blocke
								{
									closestStopBlock = i;
								}
							}
						}
					}
					
				}
			}
			
		}
		return closestStopBlock;
	}


}
