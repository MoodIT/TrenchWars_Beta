using UnityEngine;
using System.Collections;
using DG.Tweening;
public class UnitMelee : MonoBehaviour {

	private Animator anim;
	private UnitMovement unitMove;
	private UnitShooting unitShoot;
	private UnitStats unitStats;

	public float timeBetweenHits = 0.5f;
	public float hitDamage = 4f;
	[HideInInspector]
	public bool fighting;

	private EnemyStats enmStats;
	void Start () 
	{
		anim = GetComponentInChildren<Animator>();
		unitMove = GetComponent<UnitMovement>();
		unitShoot = GetComponent<UnitShooting>();
		unitStats= GetComponent<UnitStats>();
	}

	public void EnterFight(GameObject enemy)
	{
		anim.SetBool("Shoot", false);
		anim.SetBool("InPosition", false);
		anim.SetBool("Melee", true);
		unitMove.t.Kill();
		fighting = true;
		enmStats = enemy.GetComponent<EnemyStats>();
		StartCoroutine(DamageInterval());

//		unitMove.t.Complete();
		unitMove.t.Kill();
		unitMove.lastPointLength = unitMove.currentBlock.mLength;
		unitMove.lastPointWidth = unitMove.currentBlock.mWidth;

		transform.DOLookAt(enmStats.transform.position, 0.1f);
		unitShoot.mayShoot = false;
//		Debug.Log ("UIUIUIUIUIUIUIUIUIUIUIUI");
		//look at enemy
	}
	void ExitFight()
	{
		fighting = false;
		anim.SetBool("Melee", false);
		if(!unitStats.dead)
		{
			unitMove.InPosition();
		}

	}
	IEnumerator DamageInterval()
	{	
		float mTime = 0;
		while(fighting)
		{
			if(enmStats.health > 0 && !unitStats.dead)
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
			}
			yield return null;
		}
	}

	void DealDamage()
	{
		enmStats.health -= hitDamage;
	}


	

}
