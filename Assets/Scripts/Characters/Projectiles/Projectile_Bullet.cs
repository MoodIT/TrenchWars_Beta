using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bullet : Projectile_Base
{
    void Update()
    {
        Vector3 step = Direction * speed * Time.fixedDeltaTime;
        transform.position += step;
    }
}
