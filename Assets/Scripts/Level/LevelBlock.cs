using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    [Header("BlockData")]
    [SerializeField]
    private int blockID = -1;
    public int BlockID { get { return blockID; } set { blockID = value; } }

    [SerializeField]
    private GameObject mesh = null;

    public bool IsDigged { get; private set; }
    public bool IsSelected { get; set; }

    [SerializeField]
    private bool hasPlayer = false; //TEST!!
    public bool HasPlayer { get { return hasPlayer; } }
    //public bool HasPlayer { get; }

    [SerializeField]
    private bool isWalkable = true;
    public bool IsWalkable
    {
        get
        {
            if (!isWalkable)
                return false;

            foreach (LevelObstacle obstacle in obstacles)
            {
                if (!obstacle.isWalkable)
                    return false;
            }
            return true;
        }
    }

    public float WalkSpeedModifier
    {
        get
        {
            float modi = 1.0f;

            foreach (LevelObstacle obstacle in obstacles)
            {
                if (modi > obstacle.WalkSpeedModifier)
                    modi = obstacle.WalkSpeedModifier;
            }

            return modi;
        }
    }

    [SerializeField]
    private bool isDiggable = true;
    public bool IsDiggable
    {
        get
        {
            if (!isDiggable || IsDigged)
                return false;

            foreach (LevelObstacle obstacle in obstacles)
            {
                if (!obstacle.IsDiggable)
                    return false;
            }
            return true;
        }
        set
        {
            isDiggable = value;
        }
    }

    public List<LevelObstacle> obstacles = new List<LevelObstacle>();

/*    //remove
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

    [Header("Decorations")]
    [SerializeField]
    private List<BlockSideGraphics> sideGraphicPrefabs = null;

    [SerializeField]
    private List<BlockSideGraphics> edgeGraphicPrefabs = null;
    //---*/

    private List<GameObject> blockDecorations = new List<GameObject>();

    void Awake ()
    {
        if (mesh != null && !mesh.activeSelf)
            IsDigged = true;

        GameManager.Instance.Builder.RegisterBlock(this);
    }

    void Start ()
    {
        RegisterObstacles();
    }

    public void Initialize()
    {
        if (IsDigged)
        {
            AddNeighborEdgeGraphics();
            AddNeighborSideGraphics();
        }
    }

    public void RegisterObstacles()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.2f, 1 << GameManager.Instance.Builder.ObstacleLayer);
        for (int i = 0; i < collisions.Length; i++)
        {
            Debug.Log("Found " + collisions[i].gameObject.name + " on blockID: " + BlockID);
            LevelObstacle obstacle = collisions[i].gameObject.GetComponent<LevelObstacle>();
            if (obstacle)
                obstacles.Add(obstacle);
        }
    }

    public void CreateGraphics(List<LevelBuilder.BlockSideGraphics> graphics, LevelBuilder.Side side)
    {
        int totalChance = 0;
        List<LevelBuilder.BlockSideGraphics> variations = new List<LevelBuilder.BlockSideGraphics>();
        foreach(LevelBuilder.BlockSideGraphics bside in graphics)
        {
            if (bside.Side == side && bside.Graphics != null)
            {
                variations.Add(bside);
                totalChance += bside.Chance;
            }
        }

        //chose random prefab
        GameObject prefab = null;
        int rand = UnityEngine.Random.Range(0, totalChance);
        int curChance = 0;
        foreach(LevelBuilder.BlockSideGraphics graphic in variations)
        {
            curChance += graphic.Chance;
            if (curChance > rand)
            {
                prefab = graphic.Graphics;
                break;
            }
        }

        if(prefab != null)
        {
            GameObject obj = Instantiate(prefab, transform.position, transform.rotation, transform) as GameObject;
            blockDecorations.Add(obj);
            Debug.Log("CREATE " + obj.name);
        }
    }
    
    private void AddNeighborEdgeGraphics()
    {
        LevelBlock left = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Left, BlockID);
        if (left != null && !left.IsDigged)
            left.CreateGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs, LevelBuilder.Side.Left);

        LevelBlock right = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Right, BlockID);
        if (right != null && !right.IsDigged)
            right.CreateGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs, LevelBuilder.Side.Right);

        LevelBlock up = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Up, BlockID);
        if (up != null && !up.IsDigged)
            up.CreateGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs, LevelBuilder.Side.Up);

        LevelBlock down = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Down, BlockID);
        if (down != null && !down.IsDigged)
            down.CreateGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs, LevelBuilder.Side.Down);
    }

    private void AddNeighborSideGraphics()
    {
        LevelBlock left = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Left, BlockID);
        if (left != null && !left.IsDigged)
            left.CreateGraphics(GameManager.Instance.Builder.SideGraphicPrefabs, LevelBuilder.Side.Left);

        LevelBlock right = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Right, BlockID);
        if (right != null && !right.IsDigged)
            right.CreateGraphics(GameManager.Instance.Builder.SideGraphicPrefabs, LevelBuilder.Side.Right);

        LevelBlock up = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Up, BlockID);
        if (up != null && !up.IsDigged)
            up.CreateGraphics(GameManager.Instance.Builder.SideGraphicPrefabs, LevelBuilder.Side.Up);

        LevelBlock down = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Down, BlockID);
        if (down != null && !down.IsDigged)
            down.CreateGraphics(GameManager.Instance.Builder.SideGraphicPrefabs, LevelBuilder.Side.Down);
    }

    public void Dig()
    {
        //disable collison and remove graphics
        GetComponent<BoxCollider>().enabled = false;
        mesh.gameObject.SetActive(false);
        foreach (GameObject obj in blockDecorations)
            obj.SetActive(false);

        //Add edge graphics to neighbors
        AddNeighborEdgeGraphics();

        //Add side graphics to neighbors
        AddNeighborSideGraphics();

        IsDigged = true;
    }
}
