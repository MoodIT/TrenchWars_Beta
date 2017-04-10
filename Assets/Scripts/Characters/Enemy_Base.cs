using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character_Base
{
    protected Coroutine stateThread = null;

    [SerializeField]
    protected CharacterState initialState = CharacterState.Moving;

    [SerializeField]
    protected int supplies = 10;

    [SerializeField]
    protected int coins = 100;

    [Header("Projectiles")]
    [SerializeField]
    protected Projectile_Base projectilePrefab = null;

    [SerializeField]
    protected Vector3 projectileOffset = Vector3.zero;

    [Header("AnimStates")]
    [SerializeField]
    protected string ShootState = "Stand_Shoot";
    [SerializeField]
    protected string WalkState = "Walk";
    [SerializeField]
    protected string JumpState = "Jump";

    [Header("AnimParams")]
    [SerializeField]
    protected string walkParamName = "Walk";
    [SerializeField]
    protected string shootParamName = "Shoot";
    [SerializeField]
    protected string jumpParamName = "Jump";
    [SerializeField]
    protected string idle = "Idle";
    [SerializeField]
    protected string raisedGunToLow = "RaisedToLow";
    [SerializeField]
    protected string raisedGunToHigh = "RaisedToHigh";
    [SerializeField]
    protected string dieParamName = "Die";
    [SerializeField]
    protected string dieMineParamName = "Die_Mine";
    [SerializeField]
    protected string dieFlamerParamName = "Die_Flamer";

    [Header("AnimEvents")]
    [SerializeField]
    protected float bulletFireEventTime = 0.5f;

    [Header("Jumping")]
    [SerializeField]
    protected AnimationCurve jumpDown = null;

    [SerializeField]
    protected AnimationCurve jumpUp = null;

    public override bool IsPlayer { get { return false; } }

    void Start()
    {
        //register
        GameManager.Instance.AddEnemy(this);

        Initialize();
    }

    public override void ChangeState(CharacterState newState, bool force = false)
    {
        if (state == CharacterState.Dying)
            return;

        if (stateThread != null)
            StopCoroutine(stateThread);

//        Debug.Log("ChangeState " + state + "->" + newState);
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
                stateThread = StartCoroutine(DieState(delayRemoveCorpse));
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

        if (curHealth <= 0)
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
        else if (state == CharacterState.Moving && GameManager.Instance.PlayerInRange(CurBlock.BlockID, range))
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

        AnimationCurve walkCurve = null;

        //calc vectors
        Vector3 fromPos = CurBlock.transform.position;
        Vector3 toPos = NextBlock.transform.position;

        SetSortingOrder(NextBlock.GetBlockSortingOrder() + 10);

        if (NextBlock.IsDigged && !CurBlock.IsDigged)
            walkCurve = jumpDown;
        else if (!NextBlock.IsDigged && CurBlock.IsDigged)
            walkCurve = jumpUp;
        else
            fromPos.y = toPos.y = transform.position.y;

        //make sure we are centered
        transform.position = fromPos;

        //move to next
        Vector3 toNext = toPos - fromPos;
        float stepCountTotal = toNext.magnitude / (moveSpeed * Time.fixedDeltaTime * NextBlock.WalkSpeedModifier);
        Vector3 step = toNext / stepCountTotal;
        float stepsLeft = stepCountTotal;
        float jumpStep = stepCountTotal - Mathf.Ceil(stepCountTotal / 3.0f);
        while (stepsLeft > 0)
        {
            if (GameManager.Instance.IsGamePaused)
            {
                yield return null;
                continue;
            }

            stepsLeft--;
            transform.position += step;

            if (walkCurve != null)
            {
                float yOffset = walkCurve.Evaluate(1f - (stepsLeft / stepCountTotal));
                transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);

                if(stepsLeft == jumpStep)
                    StartCoroutine(JumpAnim());
            }

            yield return null;
        }

        CurBlock = NextBlock;
        CurBlock.Activate(this);

        anim.ResetTrigger(walkParamName);
        EvaluateState();
    }

    virtual protected IEnumerator JumpAnim()
    {
        anim.ResetTrigger(walkParamName);
        anim.SetTrigger(jumpParamName);

        while (true)
        {
            yield return null;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(WalkState))
                break;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(JumpState))
        {
            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    break;
                yield return null;
            }
        }
        anim.ResetTrigger(jumpParamName);
        anim.SetTrigger(walkParamName);
    }

    virtual protected IEnumerator DieState(float waitSec)
    {
        anim.SetTrigger(dieParamName);
        
        switch(lastDamageType)
        {
            case DamageType.Explosion:
                anim.SetTrigger(dieMineParamName);
                //HACK
                waitSec = 0.2f;
                break;
            case DamageType.Fire:
                anim.SetTrigger(dieFlamerParamName);
                break;
        }

        GameManager.Instance.AddSupplies(supplies);
        GameManager.Instance.AddCoins(coins);
        GameManager.Instance.RemoveEnemy(this);

        yield return new WaitForSeconds(waitSec);

        Destroy(gameObject);
    }
}
