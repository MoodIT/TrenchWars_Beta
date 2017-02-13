using UnityEngine;
using System.Collections;

public class Raycast : MonoBehaviour {

	public RaycastHit Ray(Vector3 pos)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100))
		{
			return hit;
		}
		return hit;
	}
}
