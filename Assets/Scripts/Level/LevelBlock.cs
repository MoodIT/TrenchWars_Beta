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
    private LevelBuilder builderRef = null;
    public LevelBuilder BuilderRef { get { return builderRef; } set { builderRef = value; } }

    private Transform mesh = null;

    public bool IsDigged { get; private set; }
    public bool IsSelected { get; set; }

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

    [Serializable]
    public class BlockSideGraphics
    {
        [SerializeField]
        private LevelBuilder.Side side = 0;
        public LevelBuilder.Side Side { get { return side; } }

        [SerializeField]
        private GameObject graphics = null;
        public GameObject Graphics { get { return graphics; } }
    }

    [Header("Decorations")]
    [SerializeField]
    private List<BlockSideGraphics> sideGraphicPrefabs = null;

    [SerializeField]
    private List<BlockSideGraphics> edgeGraphicPrefabs = null;

    private List<GameObject> blockDecorations = new List<GameObject>();

    void Awake ()
    {
        mesh = transform.FindChild("Mesh");
        if (mesh == null)
            Debug.LogError("ERROR - Cant find mesh");
    }

	void Start ()
    {
        if (BuilderRef != null)
            BuilderRef.RegisterBlock(this);

        RegisterObstacle();
    }
	
    void OnTriggerEnter(Collider obj)
    {
        Debug.LogError("Trigger");
    }

    public void RegisterObstacle()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.2f, 1 << builderRef.ObstacleLayer);
        for (int i = 0; i < collisions.Length; i++)
        {
            Debug.Log("Found " + collisions[i].gameObject.name + " on blockID: " + BlockID);
            LevelObstacle obstacle = collisions[i].gameObject.GetComponent<LevelObstacle>();
            if (obstacle)
            {
                obstacles.Add(obstacle);
            }
        }
    }

    public void CreateEdgeGraphics(LevelBuilder.Side side)
    {
        List<BlockSideGraphics> edgeVariations = new List<BlockSideGraphics>();
        foreach(BlockSideGraphics bside in edgeGraphicPrefabs)
        {
            if (bside.Side == side && bside.Graphics != null)
                edgeVariations.Add(bside);
        }

        GameObject prefab = edgeVariations[UnityEngine.Random.Range(0, edgeVariations.Count)].Graphics;
        if(prefab != null)
        {
            GameObject obj = Instantiate(prefab, transform.position, transform.rotation, transform) as GameObject;
            blockDecorations.Add(obj);
        }
    }

    public void CreateSideGraphics(LevelBuilder.Side side)
    {
        List<BlockSideGraphics> sideVariations = new List<BlockSideGraphics>();
        foreach (BlockSideGraphics bside in sideGraphicPrefabs)
        {
            if (bside.Side == side && bside.Graphics != null)
                sideVariations.Add(bside);
        }

        GameObject prefab = sideVariations[UnityEngine.Random.Range(0, sideVariations.Count)].Graphics;
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, transform.position, transform.rotation, transform) as GameObject;
            blockDecorations.Add(obj);
        }
    }
    
    private void AddNeighborEdgeGraphics()
    {
        LevelBlock left = builderRef.GetNeighbor(LevelBuilder.Side.Left, BlockID);
        if (left != null && !left.IsDigged)
            left.CreateEdgeGraphics(LevelBuilder.Side.Left);

        LevelBlock right = builderRef.GetNeighbor(LevelBuilder.Side.Right, BlockID);
        if (right != null && !right.IsDigged)
            right.CreateEdgeGraphics(LevelBuilder.Side.Right);

        LevelBlock up = builderRef.GetNeighbor(LevelBuilder.Side.Up, BlockID);
        if (up != null && !up.IsDigged)
            up.CreateEdgeGraphics(LevelBuilder.Side.Up);

        LevelBlock down = builderRef.GetNeighbor(LevelBuilder.Side.Down, BlockID);
        if (down != null && !down.IsDigged)
            down.CreateEdgeGraphics(LevelBuilder.Side.Down);

    }

    private void AddNeighborSideGraphics()
    {
        LevelBlock up = builderRef.GetNeighbor(LevelBuilder.Side.Up, BlockID);
        if (up != null && !up.IsDigged)
            up.CreateSideGraphics(LevelBuilder.Side.Up);
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
