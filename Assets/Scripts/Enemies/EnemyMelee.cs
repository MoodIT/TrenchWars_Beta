using UnityEngine;
using System.Collections;
using DG.Tweening;
public class EnemyMelee : MonoBehaviour {
	private EnemyMovement enmMovement;
	private BlockControl bControl;
	private UnitController uCon;
	private EnemyStats enmStats;
	private Animator anim;
	private FindPath fPath;
	private EnemyShooting enmShoot;

	public Block nextBlock;
	public Block prevBlock;
	private float startDX;
	private float startDZ;

	[HideInInspector]
	public bool fighting;
	public float timeBetweenHits = 0.5f;
	public float hitDamage = 1;

	private UnitStats hittingUnitStats;
	// Use this for initialization
	void Start () 
	{
		uCon = GameController.instance.unitControls;
//		animator = GetComponentInChildren < Animator> ();
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		enmMovement = GetComponent<EnemyMovement> ();
		enmStats = GetComponent<EnemyStats>();
		startDX = 1/(Mathf.Abs(enmMovement.t.PathGetPoint(1).x - enmMovement.currentBlock.mWidth));
		startDZ = 1/(Mathf.Abs(enmMovement.t.PathGetPoint(1).z - enmMovement.currentBlock.mLength));
		StartCoroutine(LookForUnitInTrench());
		anim = GetComponentInChildren<Animator>();
		fPath = GameController.instance.unitControllerObj.GetComponent<FindPath>();
		enmShoot = GetComponent<EnemyShooting>();
	}
//	
	void Update () 
	{
		if(enmMovement != null && enmMovement.t != null)
		{
			nextBlock.mWidth = (int)enmMovement.t.PathGetPoint(enmMovement.t.ElapsedPercentage() + startDX).x;
			nextBlock.mLength = (int)enmMovement.t.PathGetPoint(enmMovement.t.ElapsedPercentage() + startDZ).z;
		}
	}

	IEnumerator LookForUnitInTrench()
	{
		while(true) //!enmStats.dead && !fighting
		{
			if(enmMovement.inPlayerTrench && !enmStats.dead && !fighting)
			{
//				RaycastHit hit;
				Ray ray = new Ray(transform.position, transform.forward);
				Ray ray2 = new Ray(transform.position, -transform.forward);
				Ray ray3 = new Ray(transform.position, transform.right);
				Ray ray4 = new Ray(transform.position, -transform.right);
//				Vector3 fwd = transform.TransformDirection(Vector3.forward);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 0.4f))
				{
					if(hit.collider.CompareTag("Unit"))
					{
						hit.collider.gameObject.GetComponent<UnitMelee>().EnterFight(this.gameObject);
						hittingUnitStats = hit.collider.gameObject.GetComponent<UnitStats>();
						EnterFight();
//						yield break;
					}
				}
				if (Physics.Raycast(ray2, out hit, 0.4f) || Physics.Raycast(ray3, out hit, 0.4f) || Physics.Raycast(ray4, out hit, 0.4f))
				{

					if(hit.collider.CompareTag("Unit"))
					{
						//if unit is moving
						//maybe if unit is moving towards
						hit.collider.gameObject.GetComponent<UnitMelee>().EnterFight(this.gameObject);
						hittingUnitStats = hit.collider.gameObject.GetComponent<UnitStats>();
						EnterFight();
//						yield break;
					}
				}
//				HitUnit(new Ray(transform.position, transform.forward), true);
//				HitUnit(new Ray(transform.position, -transform.forward), false);
//				HitUnit(new Ray(transform.position, transform.right), false);
//				HitUnit(new Ray(transform.position, -transform.right), false);

				
				//if find unit back left or right And if the unit is moving And if the unit is moving towards the enemy then enter fight
					
			}
//			Debug.DrawLine(ray.origin, hit.point);
			yield return null;

		}

	
//			print("There is something in front of the object!");
		
	}

//	void HitUnit(Ray r, bool front)
//	{
//		RaycastHit hit;
//		if (Physics.Raycast(r, out hit, 0.4f))
//		{
//			if(hit.collider.CompareTag("Unit"))
//			{
//				hit.collider.gameObject.GetComponent<UnitMelee>().EnterFight(this.gameObject);
//				hittingUnitStats = hit.collider.gameObject.GetComponent<UnitStats>();
//				EnterFight();
////				yield break;
//			}
////			return true;
//		}
////		return false;
//	}

//		while(!enmStats.dead && !fighting)
//		{
//			if(enmMovement.inPlayerTrench)
//			{
//				for(int i = 0; i < bControl.interactableBlockStates.GetLength(0); i++)
//				{
//					for(int i2 = 0; i2 < bControl.interactableBlockStates.GetLength(1); i2++)
//					{
////						if(bControl.interactableBlockStates[i,i2] != 0) continue;
////						if(bControl.unitMovingOccupiedBlocks.
//						for(int t = 0; t < bControl.unitMovingOccupiedBlocks.Count; t++)
//						{
////							Debug.Log("UIGE");
//							if(bControl.unitMovingOccupiedBlocks[t].mWidth == nextBlock.mWidth && bControl.unitMovingOccupiedBlocks[t].mLength == nextBlock.mLength)
//							{
//								bControl.unitMovingOccupiedBlocks[t].mUnit.GetComponent<UnitMelee>().EnterFight(this.gameObject);
//								hittingUnitStats = bControl.unitMovingOccupiedBlocks[t].mUnit.GetComponent<UnitStats>();
//								EnterFight();
//								yield break;
//							}
//						}
//
//					}
//				}
//			}
//			yield return null;
//		}
//	}

	void EnterFight()
	{
		anim.SetBool("Melee", true);
		anim.SetBool("Shoot", false);
		fighting = true;
		enmMovement.t.Kill();
		this.transform.DOLookAt(hittingUnitStats.transform.position, 0.1f);
		enmShoot.mayShoot = false;
//		StopCoroutine(LookForUnitInTrench());
		StartCoroutine(AttackInterval());
	}
	void ExitFight()
	{
		fighting = false;
		enmMovement.t.Play();
//		Debug.Log ("BUGEUIG");
//		StartCoroutine(LookForUnitInTrench());
//		enmMovement.MoveOnPath
		enmShoot.mayShoot = false;
		enmMovement.DoMove(fPath.Patherize(new Block{width = (int)transform.position.x, length = (int)transform.position.z}, new Block{width = bControl.barracksBlockPos.width, length = bControl.barracksBlockPos.length}, new int[2]{0,2}, (1-GameController.instance.gameSettings.groundHeight)));
//		enmMovement.inPlayerTrench = true;
		anim.SetBool("Melee", false);
	}
	IEnumerator AttackInterval()
	{
		float mTime = 0;
		while(fighting && !enmStats.dead)
		{
			if(hittingUnitStats.health > 0)
			{
				mTime += Time.deltaTime;
				if(mTime > timeBetweenHits)
				{
					DealDamage();
					mTime = 0;
					
				}
			}
			else
			{
				ExitFight();

				yield break;
			}
			yield return null;
		}
	}
	
	void DealDamage()
	{
		hittingUnitStats.health -= hitDamage;
	}
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(new Vector3((int)nextBlock.mWidth, 0.5f, (int)nextBlock.mLength), new Vector3(0.5f,0.5f,0.5f));
//		Gizmos.DrawWireCube(new Vector3((int)prevBlock.mWidth, 0.5f, (int)prevBlock.mLength), new Vector3(0.5f,0.5f,0.5f));
	}


}
