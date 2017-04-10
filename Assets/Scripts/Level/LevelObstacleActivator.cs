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
        if (activated || canActivate == Activators.None || (character.IsPlayer && canActivate == Activators.Enemies) || (!character.IsPlayer && canActivate == Activators.Players))
            return;

        if (activateObj != null)
            activateObj.Activate(character);
    }
}
