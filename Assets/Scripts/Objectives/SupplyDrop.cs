using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDrop : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    [SerializeField]
    private int supplyAmount = 20;

    [SerializeField]
    private float timeOnGround = 2;
    private float timeLeftOnGround = 2;

    [Header("Effects")]
    [SerializeField]
    private GameObject landEffect = null;

    [SerializeField]
    private GameObject pickupEffect = null;

    [Header("Graphics")]
    [SerializeField]
    private GameObject parachute = null;

    [SerializeField]
    private GameObject create = null;

    private bool landed = false;
    public bool HasLanded { get { return landed; } }

    void Awake ()
    {
        timeLeftOnGround = timeOnGround;
    }

	void Update ()
    {
        if (transform.localPosition.y > 0)
        {
            float step = speed * Time.deltaTime;
            if (transform.localPosition.y < step)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                Land();
            }
            else
                transform.localPosition -= new Vector3(0, step, 0);
        }
        else if (!landed)
            Land();

        if (landed)
        {
            timeLeftOnGround -= Time.deltaTime;
            if (timeLeftOnGround <= 0)
                Destroy(gameObject);
        }
	}

    private void Land()
    {
        parachute.SetActive(false);
        landed = true;
    }

    public void Pickup()
    {
        GameManager.Instance.AddSupplies(supplyAmount);

        Destroy(gameObject);
    }
}
