using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacle : MonoBehaviour
{
    enum obstacleType : int
    {
        Blocker = 0,
        Barbwire = 1,
        Decoration = 2,
        Mine = 3,
        GameState = 4,
        Flag = 5,
    }
    [SerializeField]
    private obstacleType type = obstacleType.Blocker;

    [SerializeField]
    private float walkSpeedModifier = 1;
    public float WalkSpeedModifier { get { return walkSpeedModifier; } }

    [SerializeField]
    private int damage = 0;
    public int Damage { get { return damage; } }

    [SerializeField]
    private bool blockWalking = false;
    public bool isWalkable { get { return !blockWalking; } }

    [SerializeField]
    private bool blockDigging = false;
    public bool IsDiggable { get { return !blockDigging; } }

    [Header("Activation")]
    [SerializeField]
    private GameObject activateEffect = null;

    [SerializeField]
    private bool removeAfterActivation = false;

    public enum Activators
    {
        Players = 0,
        Enemies = 1,
        Both = 2,
        None = 3,
    }
    [SerializeField]
    private Activators canActivate = Activators.None;

    private bool activated = false;

    public virtual void Activate(Character_Base character)
    {
        if (activated)
            return;

        if (canActivate == Activators.None || (character.IsPlayer && canActivate == Activators.Enemies) || (!character.IsPlayer && canActivate == Activators.Players))
            return;

        ParticleManager.instance.CreateEffect(activateEffect, transform.localPosition, Quaternion.identity);

        character.AddDamage(Math.Abs(Damage));

        if (removeAfterActivation)
            Destroy(gameObject);

        activated = true;
        Debug.Log(name + " Activated");
    }

}
