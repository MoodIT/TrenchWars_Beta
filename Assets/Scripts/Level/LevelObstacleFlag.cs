using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleFlag : LevelObstacle
{
	void Start ()
    {
        GameManager.Instance.RegisterFlag(this);	
	}

    public override void Activate(Character_Base character)
    {
        base.Activate(character);

        GameManager.Instance.FlagActivated(this);
    }
}
