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
    private bool blockDigging = false;
    public bool IsDiggable { get { return !blockDigging; } }
}
