using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GrenadeBehaviour : MonoBehaviour 
{
	[HideInInspector]
	public Vector3 direction = new Vector3(-1,0,0); 
	public float speed = 2;
	public Vector3 target;
	public float targetDist;
	[HideInInspector]
	public string shotBy;
	[HideInInspector]
	public List<GameObject> insideCol;
	public bool canDetonate;
	public float damage = 1;
	void Start()
	{
		ShotMovement ();
	}

	void Update()
	{
		if(GetComponent<Rigidbody>().velocity.y < -0.1f)
		{
			canDetonate = true;
		}
//		Debug.Log (GetComponent<Rigidbody> ().velocity.y);
	}
	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget) {
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



	void ShotMovement()
	{
		if(target != Vector3.zero)
		{
			GetComponent<Rigidbody>().AddForce(calculateBestThrowSpeed(transform.position, target, 1f), ForceMode.VelocityChange);
			GetComponent<Rigidbody>().AddTorque(-transform.forward *Random.Range(200,250));
		}

	}
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Unit") && shotBy == "Enemy")
		{
			insideCol.Add(col.gameObject);
//			GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.grenadeHitSound, true, 0.3f, false);
//			col.GetComponent<UnitStats>().health --;
//			this.gameObject.SetActive(false);
		}
////		else
		if(col.CompareTag("Enemy")	&& shotBy == "Unit")
		{
			insideCol.Add(col.gameObject);
//			GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.grenadeHitSound, true, 0.3f, false);

//			col.GetComponent<EnemyStats>().health --; 
//			this.gameObject.SetActive(false);
		}

		if(col.CompareTag("Ground"))
		{
			if(canDetonate)
			{
				GameController.instance.audioController.StartPlay(GameController.instance.gameSettings.grenadeHitSound, true, 0.3f, false);
                
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 2);
				int i = 0;
				while (i < hitColliders.Length) {
					if(hitColliders[i].CompareTag("Unit") && shotBy == "Enemy")
					{
						hitColliders[i].GetComponent<UnitStats>().health -= damage;
//						hitColliders[i].SendMessage("AddDamage");
					}
					if(hitColliders[i].CompareTag("Enemy") && shotBy == "Unit")
					{
						hitColliders[i].GetComponent<EnemyStats>().health -= damage;
					}

					i++;
				}

				if(GameController.instance.gameSettings.grenadeEffect != null)
				{
					Instantiate(GameController.instance.gameSettings.grenadeEffect, col.transform.position, Quaternion.identity);

				}
			}

			Destroy(this.gameObject);
		}

	}

//	void OnTriggerExit(Collider col)
//	{
//		if(insideCol.Contains(col.gameObject))
//		{
//			insideCol.Remove(col.gameObject);
//		}
//	}
}


