using UnityEngine;
using System.Collections;

public class ShotBehaviour : MonoBehaviour 
{
	[HideInInspector]
	public Vector3 direction; 
	public float speed = 2;
	public float lifeTime;
	public bool scaleOverTime;
	private Vector3 startScale;
	public Vector3 endScale = new Vector3(4,4,4);

	public bool colorOverTime;
	public Color startColor = Color.white;
	public Color endColor = Color.white;
	public bool dieOnCollision = true;
	[HideInInspector]
	public string shotBy;
	public float damage = 1;
	void Start()
	{
		StartCoroutine (ShotMovement ());
		StartCoroutine (LifeTime ());

		if (scaleOverTime)
			StartCoroutine (ScaleOverTime ());

		if (colorOverTime)
			StartCoroutine (ColorOverTime ());
	}

	IEnumerator ShotMovement()
	{
		bool on = true;
		while(on)
		{			
			transform.position = transform.position + direction * Time.deltaTime * speed;
			yield return null;
		}
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Enemy") && shotBy == "Unit")
		{
			col.GetComponent<EnemyStats>().health -= damage;
			if(dieOnCollision)
				this.gameObject.SetActive(false);
		}
		if(col.CompareTag("Unit") && shotBy == "Enemy")
		{
			col.GetComponent<UnitStats>().health -= damage;
			if(dieOnCollision)
				this.gameObject.SetActive(false);
		}

	}

	IEnumerator LifeTime()
	{
		yield return new WaitForSeconds (lifeTime);
		Destroy (this.gameObject);
	}

	IEnumerator ScaleOverTime()
	{
		bool on = true;
		float mTime = 0;
		while(on)
		{
			if(this.transform.localScale.x < endScale.x)
			{
				mTime += Time.deltaTime;
				this.transform.localScale = Vector3.Lerp (startScale, endScale, mTime/lifeTime);
			}
			else
			{
				on = false;
				break;
			}
			yield return null;
		}
	}
	IEnumerator ColorOverTime()
	{
		bool on = true;
		float mTime = 0;
		Renderer r = GetComponent<Renderer> ();

		while(on)
		{
//			if(r.material.color.r != endColor.r)
//			{
				mTime += Time.deltaTime;
				r.material.color = Color.Lerp(startColor, endColor, mTime/lifeTime);
//				this.transform.localScale = Vector3.Lerp (startScale, endScale, mTime/lifeTime);
//			}
//			else
//			{
//				on = false;
//				break;
//			}
			yield return null;
		}
	}

}


