using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UnitMovement : MonoBehaviour {
	[HideInInspector]
	public UnitController unitControl;

	[HideInInspector]
	public BlockControl bControl;

	[HideInInspector]
	public UnitStats unitStats;

	private ObjectiveController objectiveCon;
	private bool moving;
//	[HideInInspector]
	public Block currentBlock;
	public int width;
	public int length;
	public Tweener t = null;
	public int lastPointWidth;
	public int lastPointLength;
//	[HideInInspector]
	public bool inPosition;

	public Block prevOccupiedBlock = null;

	[HideInInspector]
	public Animator animator;
//	[HideInInspector]
	public Transform lookAtBlock;
	[HideInInspector]
	public bool digging;
	[HideInInspector]
	public bool doneDigging;

	[HideInInspector]
	public bool attacking;
	[HideInInspector]
	public enum MovementMode{DEFENSE, ATTACK, DIGGING};
	[HideInInspector]
	public MovementMode movementMode;

	private OutsideTrenchPath outTrenchPath;

	private FindPath fPath;

	private GameObject[] flags;
	void Start()
	{
//		flags = new GameObject[GameObject.FindObjectsOfType<Flag>().Length];


		animator = GetComponentInChildren<Animator> ();
		StartCoroutine(CheckCurrentBlock()); 
		inPosition = true;
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		unitStats = GetComponent<UnitStats> ();
		unitControl = GameController.instance.unitControls;
		outTrenchPath = unitControl.GetComponent<OutsideTrenchPath>();
		fPath = unitControl.GetComponent<FindPath>();
		objectiveCon = GameController.instance.objectiveController;
	}
	public void MoveOnPath(Transform trans, Vector3[] path, bool usePosAsFirst)
	{
//		Debug.Log ("MOVENGUEG");
		animator.SetBool("Shoot", false);
		animator.SetBool("InPosition", false);
//		if(!moving)
//		{
		if(t != null)
		{
			if(t.IsPlaying())
			{
				t.Kill();
				currentBlock = new Block{width = lastPointWidth, length = lastPointLength};
			}
		}

		if (path != null) 
		{
			moving = true;
			inPosition = false;
			if(prevOccupiedBlock != null)
			{
				bControl.unitOccupiedBlocks.Remove (prevOccupiedBlock); 
				bControl.unitsOnBlocks.Remove(this.gameObject);
			}
//			path[0] = this.transform.position;
			t = trans.DOPath (path, 2).SetEase (Ease.Linear).SetSpeedBased ().SetLookAt (0.01f).OnComplete (InPosition);		
			lastPointWidth = (int)path [path.Length-1].x;
			lastPointLength = (int)path [path.Length-1].z;
		}
	}

	public void InPosition()
	{

		if(movementMode == MovementMode.DIGGING)
		{
			
			if(!doneDigging)
			{
				GetComponent<UnitDigging>().ReachedBlock();
			}
			else
			{
				//				unitControl.units.Remove(this.gameObject);
//				Debug.Log (doneDigging);
				unitControl.diggerOutDigging = false;
				Destroy(this.gameObject);
				digging = false;
//				movementMode == MovementMode.DIGGING
				doneDigging = true;
				bControl.toBeDugBlocks.Clear();

			}
		}
		else if(movementMode == MovementMode.DEFENSE)
		{
			bControl.unitOccupiedBlocks.Add (prevOccupiedBlock = new Block{width = lastPointWidth, length = lastPointLength}); 
			bControl.unitsOnBlocks.Add (this.gameObject);
			animator.SetBool("InPosition", true);

		}
		else if(movementMode == MovementMode.ATTACK)
		{
			StartCoroutine(JumpToTarget(bControl.interactableBlockObjs[lastPointWidth+1, lastPointLength].transform.position, false));
		}
		inPosition = true;

		unitStats.currentBlock = prevOccupiedBlock;
//		transform.DORotate (new Vector3 (1, 1, 0), 0.1f);
		if(lookAtBlock != null)
			transform.DOLookAt (new Vector3(lookAtBlock.position.x, (1-GameController.instance.gameSettings.groundHeight), lookAtBlock.position.z), 0.1f);
//		transform.rotation = Quaternion.Euler (0, 0, 0);
		objectiveCon.UnitMovementEnd();
	}

	public void LookAtBlock (Vector3 pos)
	{
		transform.DOLookAt (pos, 0.4f);
	}
	IEnumerator CheckCurrentBlock()
	{

		while(true)
		{
			if(t != null)
			{
				width = currentBlock.width;
				length = currentBlock.length;
				if(t.PathGetPoint(t.ElapsedPercentage()) != Vector3.zero)
				{
					if(bControl.unitMovingOccupiedBlocks.Contains(currentBlock))
					{
						bControl.unitMovingOccupiedBlocks.Remove(currentBlock);
					}
					//if bcontrol unitsmovingOccupiedBlocks contains current block, remove it
					currentBlock = new Block{width = (int)t.PathGetPoint(t.ElapsedPercentage()).x, length = (int)t.PathGetPoint(t.ElapsedPercentage()).z};
					currentBlock.mUnit = this.gameObject;
					if(!unitStats.dead)
					{
						bControl.unitMovingOccupiedBlocks.Add(currentBlock);
					}
					//add current block
				}
				else
				{
					if(bControl.unitMovingOccupiedBlocks.Contains(currentBlock))
					{
						bControl.unitMovingOccupiedBlocks.Remove(currentBlock);
					}
					currentBlock = new Block{width = lastPointWidth, length = lastPointLength};
					currentBlock.mUnit = this.gameObject;
					if(!unitStats.dead)
					{
						bControl.unitMovingOccupiedBlocks.Add(currentBlock);
					}

				}
				if(!t.IsPlaying())
				{
					moving = false;
				}
			}
			yield return null;// new WaitForSeconds(0.1f);
		}
	}

	IEnumerator JumpToTarget(Vector3 target, bool down)
	{
		animator.SetBool ("Jump", true);
//		Debug.Log ("BGUE23tse");

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


		while(on)
		{

//			if(transform.position.y <= 0 && down)
//			{
//				animator.SetBool ("Jump", false);
//				GetComponent<Rigidbody> ().useGravity = false;
//				GetComponent<Rigidbody> ().isKinematic = true;
//				GetComponent<Rigidbody> ().isKinematic = false;
//				transform.position = new Vector3(transform.position.x, 0, transform.position.z);
//				on = false;
////				inPlayerTrench = true;
//				bControl.enemyOccupiedBlocksTrench.Add(currentBlock);
////				DoMove(fPath.Patherize(new Block{width = (int)transform.position.x, length = (int)transform.position.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, 0));
//				yield break;
//			}
//			if(this.transform.position == target)
//			{
//				Debug.Log ("BGUE");
//			}
			//			if(Mathf.Approximately(transform.position.y, 1) && !down && !endNextY)
			//			{
			//				endNextY = true;
			//			}


			//				inEnemyTrench = false;

			//				DoMove (FindPath (new int[1]{1},1, startBlockGoTo.length));


			if(transform.position.x >= target.x && !down) //transform.position.y == (int)1 transform.position.y >= (int)1 && 
			{
//				UnityEngine.Profiler.maxNumberOfSamplesPerFrame = -1;
				on = false;
//				Debug.Log ("BNUE");
				animator.SetBool ("Jump", false);
				GetComponent<Rigidbody> ().useGravity = false;
				GetComponent<Rigidbody> ().isKinematic = true;
				GetComponent<Rigidbody> ().isKinematic = false;
				transform.position = new Vector3(transform.position.x, 1, transform.position.z);

//				
//				currentBlock = new Block{width = currentBlock.width+1, length = currentBlock.length};
				Block b = new Block{width = currentBlock.width+1, length = currentBlock.length};
				Vector3[] p = new Vector3[12]{new Vector3(3,1,2), new Vector3(4,1,2), new Vector3(5,1,2), new Vector3(6,1,2), new Vector3(7,1,2), new Vector3(8,1,2), new Vector3(9,1,2), new Vector3(10,1,2), new Vector3(11,1,2), new Vector3(12,1,2), new Vector3(13,1,2), new Vector3(14,1,2)};
//				Debug.Log (outTrenchPath.FindPath(new int[1]{1},1, currentBlock.length,currentBlock, true));
				MoveOnPath(this.transform,fPath.Patherize(b, new Block{length = 2, width = 13}, new int[1]{1}, 1), false); //fPath.Patherize(b, new Block{length = 2, width = 10}, new int[1]{1}, 1)

				yield break;
			}
			
			yield return null;
		}
		
	}

	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget) 
	{
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



}
