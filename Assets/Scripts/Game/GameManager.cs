using Assets.Scripts;
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

    private bool gamePaused = false;
    public bool IsGamePaused { get { return gamePaused; } }

    public void ToggleGamePause()
    {
        if (IsGameRunning)
        {
			Debug.Log ("yoyo");
            Time.timeScale = 1.0f - Time.timeScale;
            gamePaused = !gamePaused;
        }
    }

    private Player_Base selPlayer = null;
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

    [Header("HUD")]
    [SerializeField]
    private HUDManager hudManager = null;
    public HUDManager HUD { get { return hudManager; } }

    [SerializeField]
    private WinScreenManage winScreen = null;

    [SerializeField]
    private GameObject failScreen = null;

    [Header("Initial Data")]
    [SerializeField]
    private int supplies = 0;
    public int Supplies { get { return supplies; } }

    [SerializeField]
    private int coins = 0;
    public int Coins { get { return coins; } }

    [SerializeField]
    private int maxTrensies = 1;
    public int MaxTrensies { get { return maxTrensies; } }

    private GameObject waitingForAbility = null;
    public GameObject SpawningAbility { get { return waitingForAbility; } }
    public void SpawWaitingAbility(LevelBlock block)
    {
        if (waitingForAbility == null)
            return;

        LevelObstacle obstacle = Instantiate(waitingForAbility, block.transform.position + (Vector3.up * .5f), Quaternion.identity, Builder.ObstacleParent).GetComponent<LevelObstacle>();
        block.RegisterObstacle(obstacle);

        waitingForAbility = null;
    }

    private GameObject waitingToSpawn = null;
    public GameObject SpawningPlayer { get { return waitingToSpawn; } }
    public void SpawnWaitingTrensies(LevelBlock block)
    {
        if (waitingToSpawn == null)
            return;

        Player_Base player = Instantiate(waitingToSpawn, BasePlacement.transform.position - (Vector3.up * .5f), Quaternion.identity, Builder.PlayerParent).GetComponent<Player_Base>();
        player.CurBlock = BasePlacement;

        SoundManager.instance.PlaySound(spawnTrensieSound, player.gameObject);

        MovePlayer(player, block);

        waitingToSpawn = null;
    }

    HashSet<Player_Base> Trensies = new HashSet<Player_Base>();
    HashSet<Enemy_Base> Enemies = new HashSet<Enemy_Base>();

    [SerializeField]
    private int gameTimeSec = 100;
    private float timeLeft = float.MaxValue;

    [Header("Sounds")]
    [SerializeField]
    protected AudioClip spawnTrensieSound = null;

    [SerializeField]
    protected AudioClip moveTrensieSound = null;

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
        timeLeft -= Time.deltaTime; ;
        HUD.UpdateTimer(timeLeft, gameTimeSec);

        if (timeLeft <= 0)
        {
			Debug.Log ("lala");
            GameFailed();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << Builder.PlayerLayer);
            if (hit)
            {
				Debug.Log ("playerhit");
                Player_Base player = hitInfo.collider.GetComponent<Player_Base>();
                if (player)
                    selPlayer = player;
            }
            else
                selPlayer = null;

            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << Builder.SupplyDropLayer);
            if(hit)
            {
				Debug.Log ("supplyhit");

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
        MovePlayer(selPlayer, goalblock);
    }

    public void MovePlayer(Player_Base player, LevelBlock goalblock)
    {
        if (player == null)
            return;

        Player_Base playerAtBlock = GetPlayerAtBlock(goalblock);
        if (playerAtBlock != null)
        {
            LevelBlock freeBlock = GetNearestFreeBlock(goalblock);

            if (freeBlock != null)
                MovePlayer(playerAtBlock, freeBlock);
        }

        SoundManager.instance.PlaySound(moveTrensieSound, player.gameObject);

        List<BlockNode> newNodes = new List<BlockNode>();
        List<BlockNode> usedNodes = new List<BlockNode>();

        BlockNode start = new BlockNode();
        start.block = player.CurBlock;
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
                    player.MoveTo(path.ToArray());
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

    private Player_Base GetPlayerAtBlock(LevelBlock block)
    {
        foreach (Player_Base player in Trensies)
        {
            if (player.CurBlock == block)
                return player;
        }
        return null;
    }

    private LevelBlock GetNearestFreeBlock(LevelBlock block)
    {
        LevelBuilder.Side[] sides = new LevelBuilder.Side[] { LevelBuilder.Side.Down, LevelBuilder.Side.Left, LevelBuilder.Side.Right, LevelBuilder.Side.Up };

        HashSet<LevelBlock> usedBlocks = new HashSet<LevelBlock>();
        List<LevelBlock> blocks = new List<LevelBlock>();
        blocks.Add(block);

        while (blocks.Count > 0)
        {
            LevelBlock curBlock = blocks[0];
            usedBlocks.Add(curBlock);
            blocks.Remove(curBlock);

            foreach (LevelBuilder.Side side in sides)
            {
                LevelBlock neighbor = Builder.GetNeighbor(side, curBlock.BlockID);
                if (neighbor.IsDigged)
                {
                    if(!GetPlayerAtBlock(neighbor))
                        return neighbor;
                    
                    if(!usedBlocks.Contains(neighbor))
                        blocks.Add(neighbor);
                }
            }
        }
        return null;
    }

    public void AddPlayer(Player_Base player)
    {
        Trensies.Add(player);

        HUD.UpdateTrensies(Trensies.Count, MaxTrensies);
    }

    public void RemovePlayer(Player_Base player)
    {
        if(Trensies.Contains(player))
            Trensies.Remove(player);

        HUD.UpdateTrensies(Trensies.Count, MaxTrensies);
    }

    public void AddEnemy(Enemy_Base enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy_Base enemy)
    {
        if (Enemies.Contains(enemy))
            Enemies.Remove(enemy);
    }

    public void AddSupplies(int count)
    {
        supplies += count;
        HUD.UpdateSupplies();
    }

    public void AddCoins(int count)
    {
        coins += count;
        HUD.UpdateCoins();
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

    public void OnUseAbility(GameObject info)
    {
        if (waitingForAbility != null)
        {
            Debug.LogError("waiting to place");
            return;
        }

        Ability ability = info.GetComponent<Ability>();
        if (ability.Count == 0)
        {
            Debug.LogError("not enough left");
            return;
        }

        ability.UpdateCount(ability.Count - 1);
        waitingForAbility = ability.AbilityPrefab;
    }

    //
    public bool PlayerInRange(int fromID, int range)
    {
        foreach(Player_Base character in Trensies)
        {
            Vector2 dist = Helpers.CalcBlockDistance(fromID, character.CurBlock.BlockID, Builder.LevelSize);
            if (Mathf.Abs(dist.x) < range && dist.y == 0)
                return true;
        }
        return false;
    }

    public bool EnemyInRange(int fromID, int range)
    {
        foreach (Enemy_Base character in Enemies)
        {
            Vector2 dist = Helpers.CalcBlockDistance(fromID, character.CurBlock.BlockID, Builder.LevelSize);
            if (Mathf.Abs(dist.x) < range && dist.y == 0)
                return true;
        }
        return false;
    }

    //win/loose
    public void GameFailed()
    {
		Debug.Log ("gamefail pik");
        if (failScreen != null)
            failScreen.SetActive(true);

        StartCoroutine(DelayGameOver());
    }

    private IEnumerator DelayGameOver()
    {
        yield return null;
        ToggleGamePause();
        gameEnded = true;
    }

    public void GameWon()
    {
//        winScreen.Coins = Coins;
        winScreen.TimeSec = gameTimeSec - timeLeft;
        winScreen.Stars = HUD.CountActiveStars();

        if (winScreen != null)
            winScreen.gameObject.SetActive(true);

        StartCoroutine(DelayGameOver());
    }
}
