using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField]
    protected Transform enemyParent = null;

    [SerializeField]
    private float initialDelay = 0;

    [System.Serializable]
    public class spawnEnemyDef
    {
        public Enemy_Base enemyPrefab = null;
        public int spawnCount = 1;
        public int spawnDelay = 1;
    }

    [SerializeField]
    private List<spawnEnemyDef> enemyPrefabs = new List<spawnEnemyDef>();

    [SerializeField]
    private int repeatCount = 0;

    private int spawnIdx = 0;
    private int spawnCount = 0;
    private float timeToNextSpawn = 0;

    private LevelBlock startBlock = null;

    [Header("Debug")]
    [SerializeField]
    private GameObject mesh = null;

    void Start()
    {
        timeToNextSpawn = initialDelay;
        if (mesh != null)
            mesh.SetActive(false);

        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.2f, 1 << GameManager.Instance.Builder.BlockLayer);
        if (collisions.Length == 0 || collisions.Length > 1)
            Debug.LogError("ERROR - Spawner cant fint startblock", gameObject);
        else
            startBlock = collisions[0].GetComponent<LevelBlock>();
    }

    // Update is called once per frame
    void Update ()
    {
        timeToNextSpawn -= Time.deltaTime;
		if(timeToNextSpawn <= 0)
        {
            if(spawnIdx >= enemyPrefabs.Count)
            {
                if (repeatCount > 0)
                {
                    spawnIdx = 0;
                    repeatCount--;
                }
                else
                {
                    gameObject.SetActive(false);
                    return;
                }
            }

            spawnEnemyDef def = enemyPrefabs[spawnIdx];
            Enemy_Base block = Instantiate(def.enemyPrefab, Vector3.zero, Quaternion.identity, enemyParent).GetComponent<Enemy_Base>();
            block.CurBlock = startBlock;

            spawnCount++;
            timeToNextSpawn = def.spawnDelay;

            if (spawnCount >= def.spawnCount)
            {
                spawnIdx++;
                spawnCount = 0;
            }
        }
    }
}
