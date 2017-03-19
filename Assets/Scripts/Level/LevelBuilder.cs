using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private List<LevelBlock> curSelected = new List<LevelBlock>();
    private int selectedCost = 0;
    private Coroutine blockSel = null;
    private GameObject digShovel = null;

    [Header("Effects")]
    [SerializeField]
    private GameObject shovelGraphicPrefab = null;
    [SerializeField]
    private GameObject dibEffectPredab = null;

    [Header("Parent Folders")]
    [SerializeField]
    private Transform levelBlockParent = null;
    public Transform LevelBlockParent { get { return levelBlockParent; } }

    [SerializeField]
    private Transform enemyParent = null;
    public Transform EnemyParent { get { return enemyParent; } }

    [SerializeField]
    private Transform playerParent = null;
    public Transform PlayerParent { get { return playerParent; } }

    [SerializeField]
    private Transform projectileParent = null;
    public Transform ProjectileParent { get { return projectileParent; } }

    [Header("Level Building")]
    [SerializeField]
    private bool createBlocks = false;
    [SerializeField]
    private Vector2 levelSize = Vector2.zero;
    [SerializeField]
    private GameObject levelBlockPrefab = null;

    private Dictionary<int, LevelBlock> levelBlockDic = new Dictionary<int, LevelBlock>();

    public int ObstacleLayer { get; private set; }
    public int BlockLayer { get; private set; }
    public int EnemyLayer { get; private set; }
    public int PlayerLayer { get; private set; }

    public enum Side
    {
        Left = 0,
        Right = 1, 
        Up = 2,
        Down = 3,
    }

    [Serializable]
    public class BlockSideGraphics
    {
        [SerializeField]
        private LevelBuilder.Side side = 0;
        public LevelBuilder.Side Side { get { return side; } }

        [SerializeField]
        private GameObject graphics = null;
        public GameObject Graphics { get { return graphics; } }

        [SerializeField]
        private int chance = 10;
        public int Chance { get { return chance; } }
    }

    [Header("Block decorations")]
    [SerializeField]
    private List<BlockSideGraphics> sideGraphicPrefabs = null;
    public List<BlockSideGraphics> SideGraphicPrefabs { get { return sideGraphicPrefabs; } }

    [SerializeField]
    private List<BlockSideGraphics> edgeGraphicPrefabs = null;
    public List<BlockSideGraphics> EdgeGraphicPrefabs { get { return edgeGraphicPrefabs; } }

    void Awake()
    {
        ObstacleLayer = LayerMask.NameToLayer("Obstacle");
        BlockLayer = LayerMask.NameToLayer("Block");
        EnemyLayer = LayerMask.NameToLayer("Enemy");
        PlayerLayer = LayerMask.NameToLayer("Player");
    }

    // Use this for initialization
    void Start ()
    {
        if (createBlocks)
            BuildGrid();

        //initialize all blocks
        foreach (LevelBlock block in levelBlockDic.Values)
        {
            if (block != null)
                block.Initialize();
        }
	}
	
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if (block)
                {
                    if (block.IsDigged)
                    {
                        GameManager.Instance.SpawnWaitingTrensies(block);
                        GameManager.Instance.MoveSelPlayer(block);
                    }
                    else if (block.IsDiggable)
                    {
                        if (!block.IsSelected)
                        {
                            if (curSelected.Count != 0)
                                DeselectAllBlocks();//clear selection
                            else if (block.Cost <= GameManager.Instance.Coins && IsNeighborDiggable(block.BlockID))
                            {//start new selection
                                block.IsSelected = true;
                                curSelected.Add(block);
                                selectedCost += block.Cost;

                                blockSel = StartCoroutine(BlockSelector());
                            }
                        }
                        else if (curSelected.Count != 0)
                        {//check if we have hit the dig block
                            if (block == curSelected[curSelected.Count - 1])
                                StartCoroutine(DigSelectedBlocks());
                        }
                    }
                }
                Debug.Log("Hit " + hitInfo.transform.gameObject.name + "    " + hitInfo.distance);
            }
        }
        else if (Input.GetMouseButtonUp(0) && blockSel != null)
        {
            StopCoroutine(blockSel);
            blockSel = null;

            LevelBlock lastBlock = curSelected[curSelected.Count-1];
            digShovel = Instantiate(shovelGraphicPrefab, lastBlock.transform.position, Quaternion.identity, lastBlock.transform);
        }
    }

    private IEnumerator BlockSelector(float delay = 0.01f)
    {
        while(Input.GetMouseButton(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if (block && 
                    block.IsDiggable && 
                    !block.IsSelected && 
                    (block.Cost + selectedCost) <= GameManager.Instance.Coins && 
                    IsNeighborDiggable(block.BlockID))
                {
                    block.IsSelected = true;
                    curSelected.Add(block);
                    selectedCost += block.Cost;

                    blockSel = StartCoroutine(BlockSelector());
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator DigSelectedBlocks(float delay = 0.2f)
    {
        Destroy(digShovel);
        foreach (LevelBlock block in curSelected)
        {
            GameManager.Instance.AddCoins(-block.Cost);

            block.Dig();
            block.IsSelected = false;

            ParticleManager.instance.CreateEffect(dibEffectPredab, block.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(delay);
        }

        curSelected.Clear();
        selectedCost = 0;
    }

    private bool IsNeighborDiggable(int blockID)
    {
        LevelBlock left = GetNeighbor(LevelBuilder.Side.Left, blockID);
        if (left.IsDigged || left.IsSelected)
            return true;
        LevelBlock up = GetNeighbor(LevelBuilder.Side.Up, blockID);
        if (up.IsDigged || up.IsSelected)
            return true;
        LevelBlock down = GetNeighbor(LevelBuilder.Side.Down, blockID);
        if (down.IsDigged || down.IsSelected)
            return true;
        LevelBlock right = GetNeighbor(LevelBuilder.Side.Right, blockID);
        if (right.IsDigged || right.IsSelected)
            return true;
        return false;
    }

    private void DeselectAllBlocks()
    {
        Destroy(digShovel);
        foreach (LevelBlock block in curSelected)
            block.IsSelected = false;
        curSelected.Clear();
        selectedCost = 0;
    }

    private void BuildGrid()
    {
        Vector3 basePos = transform.position;
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int z = 0; z < levelSize.y; z++)
            {
                LevelBlock block = Instantiate(levelBlockPrefab, new Vector3(basePos.x + x, 0, basePos.z + z), Quaternion.identity, LevelBlockParent).GetComponent<LevelBlock>();
                block.BlockID = z + (x * (int)levelSize.y);
                block.name = "LevelBlock_" + block.BlockID;

                if (x == 0 || x == levelSize.x - 1 || z == 0 || z == levelSize.y - 1)
                    block.IsDiggable = false;
            }
        }
    }

    public void RegisterBlock(LevelBlock block)
    {
        if (levelBlockDic.ContainsKey(block.BlockID))
        {
            Debug.LogError("ERROR: " + block.BlockID + " already registered!", block);
            return;
        }

        levelBlockDic.Add(block.BlockID, block);
    }
    
    public LevelBlock GetNeighbor(Side side, int blockID)
    {
        Vector2 coords = Helpers.convBlockID_XY(blockID, levelSize);

        switch(side)
        {
            case Side.Left:
                coords.x -= 1;
                if (coords.x < 0)
                    return null;
                break;
            case Side.Right:
                coords.x += 1;
                if (coords.x >= levelSize.x)
                    return null;
                break;
            case Side.Up:
                coords.y += 1;
                if (coords.y >= levelSize.y)
                    return null;
                break;
            case Side.Down:
                coords.y -= 1;
                if (coords.y < 0)
                    return null;
                break;
        }

        if (!levelBlockDic.ContainsKey(Helpers.convXY_BlockID(coords, levelSize)))
            return null;
        return levelBlockDic[Helpers.convXY_BlockID(coords, levelSize)];
    } 
}
