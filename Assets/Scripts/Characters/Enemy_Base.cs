using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character_Base
{
    protected Coroutine stateThread = null;

    [SerializeField]
    protected CharacterState initialState = CharacterState.Moving;

    [SerializeField]
    protected int coins = 10;

    [Header("Projectiles")]
    [SerializeField]
    protected Projectile_Base projectilePrefab = null;

    [SerializeField]
    protected Vector3 projectileOffset = Vector3.zero;

    [Header("AnimStates")]
    [SerializeField]
    protected string ShootState = "Stand_Shoot";

    [Header("AnimParams")]
    [SerializeField]
    protected string walkParamName = "Walk";
    [SerializeField]
    protected string shootParamName = "Shoot";
    [SerializeField]
    protected string dieParamName = "Die";

    [Header("AnimEvents")]
    [SerializeField]
    protected float bulletFireEventTime = 0.5f;

    public override bool IsPlayer { get { return false; } }

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

    virtual protected IEnumerator CombatCloseState()
    {
        yield return new WaitForSeconds(0.5f);
        EvaluateState();
        yield return null;
    }

    virtual protected IEnumerator CombatRangeState()
    {
        anim.SetTrigger(shootParamName);

        yield return new WaitForSeconds(2);

        anim.ResetTrigger(shootParamName);
        EvaluateState();
    }

    virtual protected IEnumerator MoveState(LevelBuilder.Side dir)
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
        CurBlock.Activate(this);

        anim.ResetTrigger(walkParamName);
        EvaluateState();
    }

    virtual protected IEnumerator DieState(int waitSec)
    {
        anim.SetTrigger(dieParamName);

        GameManager.Instance.AddSupplies(coins);

        yield return new WaitForSeconds(waitSec);
        Destroy(gameObject);
    }
}
