using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacle : MonoBehaviour
{
    public enum obstacleType : int
    {
        Blocker = 0,
        Barbwire = 1,
        Decoration = 2,
        Mine = 3,
        GameState = 4,
        Flag = 5,
        Activator = 6,
        Base = 7,
    }
    [SerializeField]
    protected obstacleType type = obstacleType.Blocker;
    public obstacleType Type { get { return type; } }

    [SerializeField]
    protected float walkSpeedModifier = 1;
    public float WalkSpeedModifier { get { return walkSpeedModifier; } }

    [SerializeField]
    protected int damage = 0;
    public int Damage { get { return damage; } }

    [SerializeField]
    protected bool blockWalking = false;
    public bool isWalkable { get { return !blockWalking; } }

    [SerializeField]
    protected bool blockDigging = false;
    public bool IsDiggable { get { return !blockDigging; } }

    [Header("Activation")]
    [SerializeField]
    protected GameObject activateEffect = null;

    [SerializeField]
    protected bool removeAfterActivation = false;

    [Header("Sounds")]
    [SerializeField]
    protected AudioClip activateSound = null;

    public LevelBlock Block { get; set; }

    public enum Activators
    {
        Players = 0,
        Enemies = 1,
        Both = 2,
        None = 3,
    }
    [SerializeField]
    protected Activators canActivate = Activators.None;

    protected bool activated = false;
    public bool IsActivated { get { return activated; } }

    protected Character_Base.DamageType GetDamageType()
    {
        if (Type == obstacleType.Mine)
            return Character_Base.DamageType.Explosion;
        return Character_Base.DamageType.Normal;
    }

    public void SetSortingOrder(int order)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = order;
        }
    }

    public virtual void Activate(Character_Base character)
    {
        if (activated)
            return;

        if (canActivate == Activators.None || (character.IsPlayer && canActivate == Activators.Enemies) || (!character.IsPlayer && canActivate == Activators.Players))
            return;

        SoundManager.instance.PlaySound(activateSound);

        ParticleManager.instance.CreateEffect(activateEffect, transform.localPosition, Quaternion.identity);

        character.AddDamage(Math.Abs(Damage), GetDamageType());

        if (removeAfterActivation)
            StartCoroutine(RemoveNextFrame());

        activated = true;
        Debug.Log(name + " Activated");
    }

    private IEnumerator RemoveNextFrame()
    {
        yield return null;

        if (Block != null)
            Block.UnregisterObstacle(this);
        Destroy(gameObject);
    }
}
