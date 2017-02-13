using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShootingCollider : MonoBehaviour {
	public List<GameObject> enemiesInRange;
	public UnitShooting uShooting;
//	// Use this for initialization
	void Start () 
	{
		uShooting = GetComponentInParent<UnitShooting> ();
	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.CompareTag("Enemy"))
		{
			if(!uShooting.enemiesInSight.Contains(col.gameObject))
				uShooting.enemiesInSight.Add(col.gameObject);
		}
	}
	void OnTriggerExit(Collider col)
	{
		if(col.gameObject.CompareTag("Enemy"))
		{
			if(uShooting.enemiesInSight.Contains(col.gameObject))
				uShooting.enemiesInSight.Remove(col.gameObject);
		}
	}
}
