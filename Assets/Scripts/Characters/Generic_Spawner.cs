using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generic_Spawner : MonoBehaviour
{
    [SerializeField]
    protected Transform ParentFolder = null;

    [SerializeField]
    protected float initialDelay = 0;

    [System.Serializable]
    public class spawnDef
    {
        public GameObject prefab = null;
        public int spawnCount = 1;
        public int spawnDelay = 1;
    }

    [SerializeField]
    protected List<spawnDef> prefabs = new List<spawnDef>();

    [SerializeField]
    protected int repeatCount = 0;

    protected int spawnIdx = 0;
    protected int spawnCount = 0;
    protected float timeToNextSpawn = 0;

    protected LevelBlock startBlock = null;

    [Header("Debug")]
    [SerializeField]
    protected GameObject mesh = null;

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
            if(spawnIdx >= prefabs.Count)
            {
                if (repeatCount > 0)
                {
                    spawnIdx = 0;
                    repeatCount--;
                }
                else
                {
//                    gameObject.SetActive(false);
                    return;
                }
            }

            spawnDef def = prefabs[spawnIdx];
            Spawn(def);

            spawnCount++;
            timeToNextSpawn = def.spawnDelay;

            if (spawnCount >= def.spawnCount)
            {
                spawnIdx++;
                spawnCount = 0;
            }
        }
    }

    virtual protected void Spawn(spawnDef spawn)
    {
        GameObject obj = Instantiate(spawn.prefab, Vector3.zero, Quaternion.identity, ParentFolder);
        
    }
}
