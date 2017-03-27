using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleState : LevelObstacle
{
    [SerializeField]
    private ParticleSystem activateEffect = null;

    private bool activated = false;

    public void Activate()
    {

    }
}
