using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private bool gameEnded = false;
    public bool IsGameRunning { get { return !gameEnded; } }

    private Character_Base selPlayer = null;
    public bool HasSelPlayer { get { return selPlayer != null; } }

    [SerializeField]
    private LevelBuilder levelBuilder = null;
    public LevelBuilder Builder { get { return levelBuilder; } }

    public LevelBlock BasePlacement { private get; set; }

    private HashSet<LevelObstacleFlag> flagDic = new HashSet<LevelObstacleFlag>();
    public void RegisterFlag(LevelObstacleFlag flag)
    {
        if (!flagDic.Contains(flag))
            flagDic.Add(flag);
    }

    public void FlagActivated(LevelObstacleFlag flag)
    {
        //check if game is done
        int activeCount = 0;
        foreach(LevelObstacle flagObj in flagDic)
        {
            if (flagObj.IsActivated)
                activeCount++;
        }
        if (activeCount >= flagDic.Count)
            GameWon();
    }

    [Header("Supplies")]
    [SerializeField]
    private GameObject supplyDropPrefab = null;

    [SerializeField]
    private GameObject supplyDropParent = null;

    [SerializeField]
    private Vector2 supplyDropSpawnDelay = new Vector2(10, 20);
    private float supplyDropSpawnTimeLeft = 0;

    [Header("HUD")]
    [SerializeField]
    private HUDManager hudManager = null;
    public HUDManager HUD { get { return hudManager; } }

    [SerializeField]
    private GameObject winScreen = null;

    [SerializeField]
    private GameObject failScreen = null;

    [Header("Initial Data")]
    [SerializeField]
    private int supplies = 0;
    public int Supplies { get { return supplies; } }

    [SerializeField]
    private int maxTrensies = 1;
    public int MaxTrensies { get { return maxTrensies; } }

    private GameObject waitingToSpawn = null;
    public GameObject SpawningPlayer { get { return waitingToSpawn; } }
    public void SpawnWaitingTrensies(LevelBlock block)
    {
        if (waitingToSpawn == null)
            return;

        Character_Base player = Instantiate(waitingToSpawn, BasePlacement.transform.position - (Vector3.up * .5f), Quaternion.identity, Builder.PlayerParent).GetComponent<Character_Base>();
        player.CurBlock = BasePlacement;

        selPlayer = player;
        MoveSelPlayer(block);

        waitingToSpawn = null;
    }

    HashSet<Character_Base> Trensies = new HashSet<Character_Base>();

    [SerializeField]
    private int gameTimeSec = 100;
    private float timeLeft = float.MaxValue;

    public GameManager()
    {
        Instance = this;
    }

    void Awake()
    {
    }

    void Start()
    {
        timeLeft = gameTimeSec;

        //Init HUD
        HUD.UpdateTimer(timeLeft, gameTimeSec);
        HUD.UpdateSupplies();
        HUD.UpdateTrensies(Trensies.Count, MaxTrensies);
    }

    void Update()
    {
        if (!IsGameRunning)
            return;

        timeLeft -= Time.deltaTime;
        hudManager.UpdateTimer(timeLeft, gameTimeSec);

        if (timeLeft <= 0)
        {
            GameFailed();
            return;
        }

        supplyDropSpawnTimeLeft -= Time.deltaTime;
        if(supplyDropSpawnTimeLeft <= 0)
        {
            LevelBlock block = Builder.GetRandomBlock();
            GameObject supplyDrop = Instantiate(supplyDropPrefab, block.transform.position, Quaternion.identity, supplyDropParent.transform);
            supplyDrop.transform.localPosition += Vector3.up * 12;

            supplyDropSpawnTimeLeft = UnityEngine.Random.Range(supplyDropSpawnDelay.x, supplyDropSpawnDelay.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << Builder.PlayerLayer);
            if (hit)
            {
                Character_Base player = hitInfo.collider.GetComponent<Character_Base>();
                if (player)
                {
                    selPlayer = player;
                }
            }
            else
                selPlayer = null;

            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << Builder.SupplyDropLayer);
            if(hit)
            {
                SupplyDrop supplies = hitInfo.collider.GetComponent<SupplyDrop>();
                if (supplies)
                    supplies.Pickup();
            }
        }
    }

    class BlockNode
    {
        public float dist = float.MaxValue;
        public LevelBlock block = null;
        public BlockNode prev = null;
    }

    public void MoveSelPlayer(LevelBlock goalblock)
    {
        if (selPlayer == null)
            return;

        List<BlockNode> newNodes = new List<BlockNode>();
        List<BlockNode> usedNodes = new List<BlockNode>();

        BlockNode start = new BlockNode();
        start.block = selPlayer.CurBlock;
        start.dist = (goalblock.transform.position - start.block.transform.position).sqrMagnitude; ;
        newNodes.Add(start);

        while(newNodes.Count > 0)
        {//find next block
            BlockNode nextBlock = null;
            foreach (BlockNode node in newNodes)
            {
                if(node.block == goalblock)
                {
                    nextBlock = node;
                    Stack<LevelBlock> path = new Stack<LevelBlock>();
                    while(nextBlock.prev != null)
                    {
                        path.Push(nextBlock.block);
                        nextBlock = nextBlock.prev;
                    }
                    selPlayer.MoveTo(path.ToArray());
                    return;
                }

                if (nextBlock == null || node.dist < nextBlock.dist)
                    nextBlock = node;
            }
            newNodes.Remove(nextBlock);

            //add new nodes
            LevelBlock up = Builder.GetNeighbor(LevelBuilder.Side.Up, nextBlock.block.BlockID);
            AddNextNode(up, goalblock, nextBlock, newNodes, usedNodes);
            LevelBlock down = Builder.GetNeighbor(LevelBuilder.Side.Down, nextBlock.block.BlockID);
            AddNextNode(down, goalblock, nextBlock, newNodes, usedNodes);
            LevelBlock left = Builder.GetNeighbor(LevelBuilder.Side.Left, nextBlock.block.BlockID);
            AddNextNode(left, goalblock, nextBlock, newNodes, usedNodes);
            LevelBlock right = Builder.GetNeighbor(LevelBuilder.Side.Right, nextBlock.block.BlockID);
            AddNextNode(right, goalblock, nextBlock, newNodes, usedNodes);

            usedNodes.Add(nextBlock);
        }
        Debug.LogError("No Path");
    }

    private void AddNextNode(LevelBlock block, LevelBlock goal, BlockNode cur, List<BlockNode> newNodes, List<BlockNode> usedNodes)
    {
        if (block != null && block.IsDigged)
        {
            BlockNode node = new BlockNode();
            node.dist = (goal.transform.position - block.transform.position).sqrMagnitude;
            node.block = block;
            node.prev = cur;

            bool skip = false;
            foreach (BlockNode openNode in newNodes)
            {
                if (openNode.block.BlockID == block.BlockID && openNode.dist <= node.dist)
                {
                    skip = true;
                    break;
                }
            }

            foreach (BlockNode usedNode in usedNodes)
            {
                if (usedNode.block.BlockID == block.BlockID && usedNode.dist <= node.dist)
                {
                    skip = true;
                    break;
                }
            }

            if (!skip)
                newNodes.Add(node);
        }
    }

    public void AddPlayer(Character_Base player)
    {
        Trensies.Add(player);

        HUD.UpdateTrensies(Trensies.Count, MaxTrensies);
    }

    public void RemovePlayer(Character_Base player)
    {
        if(Trensies.Contains(player))
            Trensies.Remove(player);

        HUD.UpdateTrensies(Trensies.Count, MaxTrensies);
    }

    public void AddSupplies(int count)
    {
        supplies += count;
        HUD.UpdateSupplies();
    }

    //Button events
    public void OnBuyTrensie(GameObject info)
    {
        if (waitingToSpawn != null)
        {
            Debug.LogError("waiting to place");
            return;
        }

        UnitCard unit = info.GetComponent<UnitCard>();
        if (supplies - unit.Cost < 0)
        {
            Debug.LogError("not enough money");
            return;
        }

        if(Trensies.Count >= MaxTrensies)
        {
            Debug.LogError("too many trensies");
            return;
        }

        supplies -= unit.Cost;

        HUD.UpdateSupplies();
        waitingToSpawn = unit.CharacterPrefab;
    }

    public void GameFailed()
    {
        if (failScreen != null)
            failScreen.SetActive(true);

        gameEnded = true;
    }

    public void GameWon()
    {
        if (winScreen != null)
            winScreen.SetActive(true);

        gameEnded = true;
    }
}
