using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject mesh = null;
    private MeshRenderer meshRender = null;

    [SerializeField]
    private GameObject selection = null;

    [SerializeField]
    private GameObject topLayerPrefab = null;
    private GameObject topLayer = null;

    [Header("BlockData")]
    [SerializeField]
    private int blockID = -1;
    public int BlockID { get { return blockID; } set { blockID = value; } }

    [SerializeField]
    private int cost = 10;
    public int Cost { get { return cost; } }

    public bool IsDigged { get; private set; }

    public int GetBlockSortingOrder()
    { return (int)Helpers.convBlockID_XY(BlockID, GameManager.Instance.Builder.LevelSize).y * -100; }

    public bool isSelected = false;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            selection.SetActive(value);
        }
    }

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

    private List<GameObject> blockDecorations = new List<GameObject>();

    void Awake ()
    {
        if (mesh != null)
        {
            if (!mesh.activeSelf)
                IsDigged = true;

            meshRender = mesh.GetComponent<MeshRenderer>();
        }

        selection.SetActive(false);

        if(!GameManager.Instance.Builder.IsCreatingBlocks)
            GameManager.Instance.Builder.RegisterBlock(this);
    }

    void Start ()
    {
        if (GameManager.Instance.Builder.IsCreatingBlocks)
            GameManager.Instance.Builder.RegisterBlock(this);

        RegisterObstacles();
    }

    public void Initialize()
    {
        if (IsDigged)
        {
            AddNeighborGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs);
            AddNeighborGraphics(GameManager.Instance.Builder.SideGraphicPrefabs);
        }
        else if (topLayerPrefab != null)
        {
            topLayer = Instantiate(topLayerPrefab, transform.position, Quaternion.identity, transform);
            topLayer.GetComponentInChildren<SpriteRenderer>().sortingOrder = GetBlockSortingOrder() - 50;
        }
    }

    public void RegisterObstacle(LevelObstacle obstacle)
    {
        obstacles.Add(obstacle);
        obstacle.Block = this;
        obstacle.SetSortingOrder(GetBlockSortingOrder());
    }

    public void UnregisterObstacle(LevelObstacle obstacle)
    {
        obstacles.Remove(obstacle);
    }

    public void RegisterObstacles()
    {
        if (GameManager.Instance.Builder.IsCreatingBlocks)
            return;

        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.2f, 1 << GameManager.Instance.Builder.ObstacleLayer);
        for (int i = 0; i < collisions.Length; i++)
        {
            Debug.Log("Found " + collisions[i].gameObject.name + " on blockID: " + BlockID);
            LevelObstacle obstacle = collisions[i].gameObject.GetComponent<LevelObstacle>();
            if (obstacle)
            {
                obstacles.Add(obstacle);
                obstacle.SetSortingOrder(GetBlockSortingOrder());

                if (obstacle.Type == LevelObstacle.obstacleType.Base)
                    GameManager.Instance.BasePlacement = this;
            }
        }
    }
    
    public void Activate(Character_Base character)
    {
        foreach (LevelObstacle obstacle in obstacles)
            obstacle.Activate(character);
    }

    public void CreateGraphics(List<LevelBuilder.BlockSideGraphics> graphics, LevelBuilder.Side side)
    {
        if (GameManager.Instance.Builder.IsCreatingBlocks)
            return;

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

            SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in sprites)
            {
                if (side == LevelBuilder.Side.Up)
                    sprite.sortingOrder = GetBlockSortingOrder() + 10;
                else if (side == LevelBuilder.Side.Down)
                    sprite.sortingOrder = GetBlockSortingOrder() - 10;
                else
                    sprite.sortingOrder = GetBlockSortingOrder();
            }

            blockDecorations.Add(obj);
//            Debug.Log("CREATE " + obj.name);
        }
    }
    
    private void AddNeighborGraphics(List<LevelBuilder.BlockSideGraphics> gfxList)
    {
        LevelBlock left = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Left, BlockID);
        if (left != null && !left.IsDigged)
            left.CreateGraphics(gfxList, LevelBuilder.Side.Left);

        LevelBlock right = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Right, BlockID);
        if (right != null && !right.IsDigged)
            right.CreateGraphics(gfxList, LevelBuilder.Side.Right);

        LevelBlock up = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Up, BlockID);
        if (up != null && !up.IsDigged)
            up.CreateGraphics(gfxList, LevelBuilder.Side.Up);

        LevelBlock down = GameManager.Instance.Builder.GetNeighbor(LevelBuilder.Side.Down, BlockID);
        if (down != null && !down.IsDigged)
            down.CreateGraphics(gfxList, LevelBuilder.Side.Down);
    }

    public void Dig()
    {
        //remove graphics
        mesh.gameObject.SetActive(false);
        foreach (GameObject obj in blockDecorations)
            obj.SetActive(false);

        if (topLayer != null)
            topLayer.SetActive(false);

        //Add edge graphics to neighbors
        AddNeighborGraphics(GameManager.Instance.Builder.EdgeGraphicPrefabs);

        //Add side graphics to neighbors
        AddNeighborGraphics(GameManager.Instance.Builder.SideGraphicPrefabs);

        IsDigged = true;
    }
}
