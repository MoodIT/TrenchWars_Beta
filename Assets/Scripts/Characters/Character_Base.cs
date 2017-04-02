using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Base : MonoBehaviour
{
    public enum CharacterType : int
    {
        Rifleman = 0,
        Flamer = 1,
        Grenadier = 2,
    }
    [SerializeField]
    protected CharacterType type = CharacterType.Rifleman;
    public CharacterType Type { get { return type; } }

    [SerializeField]
    protected float moveSpeed = 1;

    [SerializeField]
    protected int range = 5;

    [SerializeField]
    private int health = 5;
    protected int curHealth = 0;

    [SerializeField]
    protected int delayRemoveCorpse = 2;

    [SerializeField]
    protected SpriteRenderer healthBar = null;

    protected void Initialize()
    {
        curHealth = health;
        EvaluateState();
    }

    public void AddDamage(int amount, Character_Base from = null)
    {
        if (amount == 0)
            return;

        curHealth -= amount;

        healthBar.transform.localScale = new Vector3(Mathf.Clamp01((float)curHealth / (float)health), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
//        Debug.Log(name + " health: " + curHealth, gameObject);
        if (curHealth <= 0)
            Die();
    }

    virtual public void Die()
    {
        ChangeState(CharacterState.Dying);
    }

    public bool IsSelected { get; set; }

    public LevelBlock CurBlock { get; set; }
    public LevelBlock NextBlock { get; set; }

    public void SetSortingOrder(int order)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
            sprite.sortingOrder = order;

        healthBar.sortingOrder += 1;
    }

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
    { }

    virtual public void MoveTo(LevelBlock[] path)
    { }

    virtual public void ChangeState(CharacterState newState, bool force = false)
    {
        state = newState;
    }
}
