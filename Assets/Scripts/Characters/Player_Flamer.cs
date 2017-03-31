using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Flamer : Player_Base
{
    [SerializeField]
    private Projectile_Base flameEffectPrefab = null;

    [SerializeField]
    protected Vector3 flameOffset = Vector3.zero;

    [Header("AnimEvents")]
    [SerializeField]
    protected float flameFireEventTime = 0.5f;

    protected override IEnumerator CombatRangeState()
    {
        anim.SetTrigger(shootParamName);

        yield return new WaitForSeconds(flameFireEventTime);

        if (flameEffectPrefab != null)
        {
            Projectile_Base flame = Instantiate(flameEffectPrefab, transform.position + flameOffset, Quaternion.identity, GameManager.Instance.Builder.ProjectileParent);
            flame.Owner = this;
            flame.Direction = transform.right;
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
