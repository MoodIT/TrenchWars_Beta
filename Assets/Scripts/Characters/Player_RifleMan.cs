using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RifleMan : Player_Base
{
    [Header("Projectiles")]
    [SerializeField]
    protected Projectile_Base projectilePrefab = null;

    [SerializeField]
    protected Vector3 projectileOffset = Vector3.zero;

    [SerializeField]
    protected GameObject muzzleFlashEffectPrefab = null;

    [Header("AnimEvents")]
    [SerializeField]
    protected float bulletFireEventTime = 0.5f;

    protected override IEnumerator CombatRangeState()
    {
        anim.SetTrigger(shootParamName);

        yield return new WaitForSeconds(bulletFireEventTime);

        ParticleManager.instance.CreateEffect(muzzleFlashEffectPrefab, projectileOffset, Quaternion.identity);

        if (projectilePrefab != null)
        {
            Projectile_Base bullet = Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.identity, GameManager.Instance.Builder.ProjectileParent);
            bullet.transform.localScale = new Vector3(-1, 1, 1);
            bullet.Owner = this;
            bullet.Direction = transform.right;
        }

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - bulletFireEventTime);

        anim.ResetTrigger(shootParamName);
        EvaluateState();
    }
}
