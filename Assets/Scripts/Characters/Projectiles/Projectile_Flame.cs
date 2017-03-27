using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Flame : Projectile_Base
{
    [SerializeField]
    private ParticleSystem flameEffect = null;

    private bool hit = false;

    void Start()
    {
        if (flameEffect != null)
            flameEffect.Play();
    }

    void Update()
    {
        if (hit)
            return;

        if (!flameEffect.isPlaying)
            Destroy(this);
    }

    void OnTriggerEnter(Collider obj)
    {
        Character_Base hitChar = obj.gameObject.GetComponent<Character_Base>();
        if ((hitChar.IsPlayer && !Owner.IsPlayer) || (!hitChar.IsPlayer && Owner.IsPlayer))
        {
            hit = true;
            hitChar.AddDamage(damage, Owner);
            StartCoroutine(HitTarget());
        }
    }

    private IEnumerator HitTarget()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
