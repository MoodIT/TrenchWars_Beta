using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Base : Character_Base
{
    protected Coroutine stateThread = null;
    protected LevelBlock[] movePath = null;

    [SerializeField]
    protected CharacterState initialState = CharacterState.Moving;

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
    [SerializeField]
    protected string idleParamName = "Idle";

    void Start()
    {
        //register
        GameManager.Instance.AddPlayer(this);

        Initialize();
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

        if (movePath != null)
        {
            ChangeState(CharacterState.Moving);
            return;
        }

        if (GameManager.Instance.EnemyInRange(CurBlock.BlockID, range))
        {
            ChangeState(CharacterState.Combat_Ranged);
            return;
        }
        else
        {
            ChangeState(CharacterState.Idle);
            return;
        }
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
            case CharacterState.Idle:
                stateThread = StartCoroutine(IdleState());
                break;
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

    protected IEnumerator IdleState()
    {
        anim.SetTrigger(idleParamName);
        yield return new WaitForSeconds(1f);

        anim.ResetTrigger(idleParamName);
        EvaluateState();
    }

    protected IEnumerator CombatCloseState()
    {
        yield return new WaitForSeconds(0.5f);
        EvaluateState();
        yield return null;
    }

    protected virtual IEnumerator CombatRangeState()
    {
        yield return new WaitForSeconds(0.5f);
        EvaluateState();
        yield return null;
    }

    public override void MoveTo(LevelBlock[] path)
    {
        movePath = path;
        ChangeState(CharacterState.Moving);
    }

    protected IEnumerator MoveState(LevelBuilder.Side dir)
    {
        anim.SetTrigger(walkParamName);
        yield return null;

        for (int i = 0; i < movePath.Length; i++)
        {
            NextBlock = movePath[i];

            //calc vectors
            Vector3 fromPos = CurBlock.transform.position;
            Vector3 toPos = NextBlock.transform.position;
            fromPos.y = toPos.y = transform.position.y;

            SetSortingOrder(NextBlock.GetBlockSortingOrder() + 10);

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
        }

        movePath = null;
        anim.ResetTrigger(walkParamName);
        EvaluateState();
    }

    protected IEnumerator DieState(int waitSec)
    {
        anim.SetTrigger(dieParamName);

        yield return new WaitForSeconds(waitSec);

        GameManager.Instance.RemovePlayer(this);
        Destroy(gameObject);
    }
}
