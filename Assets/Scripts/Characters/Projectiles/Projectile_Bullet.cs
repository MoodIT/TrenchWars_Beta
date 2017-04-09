using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bullet : Projectile_Base
{
    [SerializeField]
    protected int speed = 1;

    [Header("Effects")]
    [SerializeField]
    protected GameObject hitEffectPrefab = null;
    [SerializeField]
    protected GameObject muzzleEffectPrefab = null;

    private bool hit = false;

    void Start()
    {
        ParticleManager.instance.CreateEffect(muzzleEffectPrefab, transform.localPosition, Quaternion.identity);
    }

    void Update()
    {
        if (hit)
            return;

        Vector3 step = Direction * speed * Time.deltaTime; ;
        transform.localPosition += step;

        //cleanup
        if (transform.localPosition.x > 50 && transform.localPosition.x < -50)
            Destroy(this);
    }

    void OnTriggerEnter(Collider obj)
    {
        Character_Base hitChar = obj.gameObject.GetComponent<Character_Base>();
        if ((hitChar.IsPlayer && !Owner.IsPlayer) || (!hitChar.IsPlayer && Owner.IsPlayer))
        {
            hit = true;
            hitChar.AddDamage(damage, Owner);
            ParticleManager.instance.CreateEffect(hitEffectPrefab, transform.localPosition, Quaternion.identity);

            StartCoroutine(HitTarget());
        }
    }

    private IEnumerator HitTarget()
    {
        yield return null;
        Destroy(gameObject);
    }
}
