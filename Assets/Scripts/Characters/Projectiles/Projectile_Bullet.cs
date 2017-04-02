using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bullet : Projectile_Base
{
    [SerializeField]
    protected int speed = 1;

    private bool hit = false;

    void Update()
    {
        if (hit)
            return;

        Vector3 step = Direction * speed * Time.fixedDeltaTime;
        transform.position += step;

        //cleanup
        if (transform.position.x > 50 && transform.position.x < -50)
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
        yield return null;// new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
