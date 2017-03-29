using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleActivator : LevelObstacle
{
    [Header("Obstacle Activator")]
    [SerializeField]
    private LevelObstacle activateObj = null;

    public override void Activate(Character_Base character)
    {
        if (activateObj != null)
            activateObj.Activate(character);
    }
}
