using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[System.Serializable]
public class Enemy: IComparable<Enemy>
{
	public string name;
	public enum EnemyType{GUNNER, FLAMETHROWER, GRENADER};
	public EnemyType enemyType;
	public int power;
	[SerializeField]
	public Block spawnPos;
	public bool spawned;
	public float health = 10;
	public float spawnTime;
	public float speed;
	public Enemy(string newName, EnemyType newEnemyType, Block newSpawnPos, bool newSpawned, float newHealth)
	{
		name = newName;
//		power = newPower;
		enemyType = newEnemyType;
		spawnPos = newSpawnPos;
		spawned = newSpawned;
		health = newHealth;
	}
	
	//This method is required by the IComparable
	//interface. 
	public int CompareTo(Enemy other)
	{
		if(other == null)
		{
			return 1;
		}
		
		//Return the difference in power.
		return power - other.power;
	}

	public void OnGUI()
	{
//		power = 
	}
}
