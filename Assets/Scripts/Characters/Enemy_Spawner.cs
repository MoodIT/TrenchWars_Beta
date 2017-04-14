using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : Generic_Spawner
{
    protected override void Spawn(spawnDef obj)
    {
        Enemy_Base enemy = Instantiate(obj.prefab, Vector3.zero, Quaternion.identity, ParentFolder).GetComponent<Enemy_Base>();
        enemy.CurBlock = startBlock;
    }
}
