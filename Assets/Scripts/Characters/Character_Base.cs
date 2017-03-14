using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Base : MonoBehaviour
{
    public enum CharacterType : int
    {
        Rifleman = 0,
    }
    [SerializeField]
    protected CharacterType type = CharacterType.Rifleman;

    [SerializeField]
    protected float moveSpeed = 1;

    [SerializeField]
    protected int health = 5;

    public void AddDamage(int amount, Character_Base from)
    {
        health -= amount;
        Debug.Log(name + " health: " + health, gameObject);
        if (health <= 0)
            Die();
    }

    virtual public void Die()
    {
        ChangeState(CharacterState.Dying);
    }

    public LevelBuilder BuilderRef { get; set; }
    public LevelBlock CurBlock { get; set; }
    public LevelBlock NextBlock { get; set; }

    virtual public bool IsPlayer { get { return true; } }

    protected Animator anim = null;

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

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    virtual public void EvaluateState()
    {

    }

    virtual public void ChangeState(CharacterState newState, bool force = false)
    {
        state = newState;
    }
}
