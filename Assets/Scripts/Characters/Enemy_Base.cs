using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    public enum EnemyType : int
    {
        Rifleman = 0,
    }
    [SerializeField]
    protected EnemyType type = EnemyType.Rifleman;

    [SerializeField]
    protected float moveSpeed = 1;

    [SerializeField]
    protected int health = 5;

    public LevelBuilder BuilderRef { get; set; }
    public LevelBlock CurBlock { get; set; }
    public LevelBlock NextBlock { get; set; }

    public enum CharacterState : int
    {
        Spawning = 0,
        Moving = 10,
        Jumping = 20,
        Combat_Ranged = 30,
        Combat_Close = 40,
        Idle = 50,
        Dying = 1000,
    }

    public CharacterState state { get; protected set; }

    virtual public void EvaluateState()
    {

    }

    virtual public void ChangeState(CharacterState newState, bool force)
    {
        state = newState;
    }
}
