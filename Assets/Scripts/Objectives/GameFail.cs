using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFail : LevelObstacle
{
    [Header("Game Fail")]
    [SerializeField]
    private GameObject mesh = null;

    public void Awake()
    {
        if (mesh != null)
            mesh.SetActive(false);
    }

    public override void Activate(Character_Base character)
    {
        GameManager.Instance.GameFailed();
    }
}
