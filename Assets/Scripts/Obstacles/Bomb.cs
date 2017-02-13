using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Bomb : MonoBehaviour {

	void Start () 
	{
		this.transform.DOMove(new Vector3(this.transform.position.x, 0.5f, this.transform.position.z), 1f).OnComplete(OnComplete).SetEase(Ease.InSine);
	}
//	
//	void Update () {
//	
//	}
	void OnComplete()
	{
		Instantiate(GameController.instance.gameSettings.grenadeEffect, this.transform.position, Quaternion.identity);
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 2);
		int i = 0;
		while (i < hitColliders.Length) {
//			if(hitColliders[i].CompareTag("Unit") && shotBy == "Enemy")
//			{
//				hitColliders[i].GetComponent<UnitStats>().health -= damage;
//				//						hitColliders[i].SendMessage("AddDamage");
//			}
			if(hitColliders[i].CompareTag("Enemy"))
			{
				hitColliders[i].GetComponent<EnemyStats>().health -= 5;
			}
			
			i++;
		}
		Destroy(this.gameObject);
	}
//	void OnTriggerEnter(Collider col)
//	{
//		Instantiate(GameController.instance.gameSettings.grenadeEffect, this.transform.position, Quaternion.identity);
//	}
}
