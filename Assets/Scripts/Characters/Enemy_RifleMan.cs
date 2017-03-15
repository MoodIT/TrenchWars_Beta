using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RifleMan : Enemy_Base
{
    private Coroutine stateThread = null;

    [SerializeField]
    private CharacterState initialState = CharacterState.Moving;

    [SerializeField]
    private int range = 3;

    [SerializeField]
    private int coins = 10;

    [Header("Projectiles")]
    [SerializeField]
    private Projectile_Base projectilePrefab = null;

    [SerializeField]
    private Vector3 projectileOffset = Vector3.zero;

    [Header("AnimStates")]
    [SerializeField]
    private string ShootState = "Stand_Shoot";

    [Header("AnimParams")]
    [SerializeField]
    private string walkParamName = "Walk";
    [SerializeField]
    private string shootParamName = "Shoot";
    [SerializeField]
    private string dieParamName = "Die";

    [Header("AnimEvents")]
    [SerializeField]
    private float bulletFireEventTime = 0.5f;

    void Start()
    {
        EvaluateState();
    }

    public override void EvaluateState()
    {
        if (state == CharacterState.Dying)
            return;

        if (health <= 0)
        {
            ChangeState(CharacterState.Dying);
            return;
        }

        LevelBlock next = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Left, CurBlock.BlockID);
        if (next == null)
        {
            ChangeState(CharacterState.Idle, true);
            return;
        }
        else if (next.HasPlayer)
        {
            NextBlock = next;
            ChangeState(CharacterState.Combat_Close);
            return;
        }
        else if (state == CharacterState.Moving)
        {
            ChangeState(CharacterState.Combat_Ranged);
            return;
        }
        else
        {
            if (!next.IsWalkable)
            {
                next = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Down, CurBlock.BlockID);
                if (next.IsWalkable)
                {
                    NextBlock = next;
                    ChangeState(CharacterState.Moving);
                    return;
                }
                else
                {
                    next = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Up, CurBlock.BlockID);
                    if (next.IsWalkable)
                    {
                        NextBlock = next;
                        ChangeState(CharacterState.Moving);
                        return;
                    }
                    else
                    {
                        Debug.LogError("ERROR - Enemy cannot move!", gameObject);
                        {
                            ChangeState(CharacterState.Idle, true);
                            return;
                        }
                    }
                }
            }
            else
            {
                NextBlock = next;
                ChangeState(CharacterState.Moving);
                return;
            }
        }
    }

    public override void ChangeState(CharacterState newState, bool force = false)
    {
        if (!force && newState == state)
            return;

        if (stateThread != null)
            StopCoroutine(stateThread);

        Debug.Log("ChangeState " + state + "->" + newState);
        state = newState;
        switch (state)
        {
            case CharacterState.Moving:
                stateThread = StartCoroutine(MoveState(LevelBuilder.Side.Left));
                break;
            case CharacterState.Combat_Ranged:
                stateThread = StartCoroutine(CombatRangeState());
                break;
            case CharacterState.Combat_Close:
                stateThread = StartCoroutine(CombatCloseState());
                break;
            case CharacterState.Dying:
                stateThread = StartCoroutine(DieState(5));
                break;
            default:
                Debug.LogError("ERROR - Unhandled state " + newState, gameObject);
                break;
        }
    }

    private IEnumerator CombatCloseState()
    {
        yield return new WaitForSeconds(0.5f);
        EvaluateState();
        yield return null;
    }

    private IEnumerator CombatRangeState()
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

    private IEnumerator MoveState(LevelBuilder.Side dir)
    {
        anim.SetTrigger(walkParamName);

        //calc vectors
        Vector3 fromPos = CurBlock.transform.position;
        Vector3 toPos = NextBlock.transform.position;

        if (NextBlock.IsDigged && !CurBlock.IsDigged)
        {//go down
            fromPos.y = transform.position.y - 1;
            toPos.y = transform.position.y - 1;
        }
        else if (!NextBlock.IsDigged && CurBlock.IsDigged)
        {//go up
            fromPos.y = transform.position.y + 1;
            toPos.y = transform.position.y + 1;
        }
        else
            fromPos.y = toPos.y = transform.position.y;

        //make sure we are centered
        transform.position = fromPos;

        //move to next
        Vector3 toNext = toPos - fromPos;
        float stepCount = toNext.magnitude / (moveSpeed * Time.fixedDeltaTime * NextBlock.WalkSpeedModifier);
        Vector3 step = toNext / stepCount;
        while (stepCount > 0)
        {
            stepCount--;
            transform.position += step;
            yield return null;
        }

        CurBlock = NextBlock;

        anim.ResetTrigger(walkParamName);
        EvaluateState();
    }

    private IEnumerator DieState(int waitSec)
    {
        anim.SetTrigger(dieParamName);

        GameManager.Instance.AddCoins(coins);

        yield return new WaitForSeconds(waitSec);
        Destroy(gameObject);
    }
}
