using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	public int damage = 11;

	[HideInInspector]
	public Block placedOnBlock;

	private BlockControl bControl;
	void Start()
	{
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
		bControl.OnDigUpdate += UpdateDigCheck ;
	}

	void OnDisable()
	{
		bControl.OnDigUpdate -= UpdateDigCheck ;
	}

	void UpdateDigCheck()
	{
		if(bControl.interactableBlockStates[placedOnBlock.mWidth, placedOnBlock.mLength] == 0)
		{
//			Debug.Log ("DESTROY MINE");
			Destroy(this.gameObject);
		}
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Enemy"))
		{
			col.GetComponent<EnemyStats>().health -= damage;
			Instantiate(GameController.instance.gameSettings.grenadeEffect, this.transform.position, Quaternion.identity);

			Destroy(this.gameObject);
		}
	}
}
