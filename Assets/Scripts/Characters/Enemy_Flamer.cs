﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flamer : Enemy_Base
{
    protected override IEnumerator CombatRangeState()
    {
        anim.SetTrigger(shootParamName);

        yield return new WaitForSeconds(bulletFireEventTime);

        if (projectilePrefab != null)
        {
            Projectile_Base bullet = Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.identity, GameManager.Instance.Builder.ProjectileParent);
            bullet.Owner = this;
            bullet.Direction = -transform.right;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(ShootState))
        {
            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    break;
                yield return null;
            }
        }

        anim.ResetTrigger(shootParamName);
        EvaluateState();
    }
}
