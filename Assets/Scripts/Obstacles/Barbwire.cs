using UnityEngine;
using System.Collections;

public class Barbwire : MonoBehaviour {

	public float damage = 0.1f;
	public float timeBetweenDamage = 0.5f;

	private bool damaging;
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Enemy"))
		{
			StartCoroutine(DamageEnemy(col.gameObject));
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.CompareTag("Enemy"))
		{
			StopCoroutine(DamageEnemy(col.gameObject));
			damaging = false;
		}
	}
	IEnumerator DamageEnemy(GameObject enemy)
	{
		damaging = true;
		float mTime = 0;
		while(damaging)
		{
			if(mTime < timeBetweenDamage)
			{
				mTime += Time.deltaTime;

			}
			else
			{
				mTime = 0;
				DealDamage(enemy);
			}
			yield return null;
		}
	}

	void DealDamage(GameObject enemy)
	{
		if(enemy != null)
		{
			enemy.GetComponent<EnemyStats>().health -= damage;
		}
	}
}
