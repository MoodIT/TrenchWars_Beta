using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleFlag : LevelObstacle
{
    [Header("Obstacle Flag")]
    [SerializeField]
    private GameObject initialFlagEffect = null;

    private GameObject initialFlag = null;

	void Start ()
    {
        GameManager.Instance.RegisterFlag(this);

        initialFlag = ParticleManager.instance.CreateEffect(initialFlagEffect, transform.localPosition, Quaternion.identity);
    }

    public override void Activate(Character_Base character)
    {
        base.Activate(character);
        Destroy(initialFlag);
        GameManager.Instance.FlagActivated(this);
    }
}
