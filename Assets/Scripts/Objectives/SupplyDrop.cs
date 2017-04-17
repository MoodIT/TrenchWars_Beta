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

    [SerializeField]
    private Vector2 dropBlockRange;

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

    [Header("Sounds")]
    [SerializeField]
    protected List<AudioClip> spawnedSounds = null;

    [SerializeField]
    protected List<AudioClip> pickupSounds = null;

    private bool landed = false;
    public bool HasLanded { get { return landed; } }

    void Awake ()
    {
        timeLeftOnGround = timeOnGround;

        LevelBlock block = GameManager.Instance.Builder.GetRandomBlockRange(dropBlockRange);
        transform.position = block.transform.position;
        transform.localPosition += Vector3.up * 12;
    }

    void Start ()
    {
        SoundManager.instance.PlayRandomSound(spawnedSounds);
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
        ParticleManager.instance.CreateEffect(landEffect, transform.position, Quaternion.identity);
        landed = true;
    }

    public void Pickup()
    {
        SoundManager.instance.PlayRandomSound(pickupSounds);

        GameManager.Instance.AddSupplies(supplyAmount);
        ParticleManager.instance.CreateEffect(pickupEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
