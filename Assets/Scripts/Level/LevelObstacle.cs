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
    }
    [SerializeField]
    private obstacleType type = obstacleType.Blocker;

    [SerializeField]
    private float walkSpeedModifier = 1;
    public float WalkSpeedModifier { get { return walkSpeedModifier; } }

    [SerializeField]
    private float damage = 0;
    public float Damage { get { return damage; } }

    [SerializeField]
    private bool blockWalking = false;
    public bool isWalkable { get { return !blockWalking; } }

    [SerializeField]
    private bool blockDigging = false;
    public bool IsDiggable { get { return !blockDigging; } }
}
