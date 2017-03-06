using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject levelBlockPrefab = null;
    [SerializeField]
    private Transform levelBlocks = null;

    [SerializeField]
    private bool createBlocks = false;
    [SerializeField]
    private Vector2 levelSize = Vector2.zero;

    private Dictionary<int, LevelBlock> levelBlockDic = new Dictionary<int, LevelBlock>();

    public int ObstacleLayer { get; private set; }
    public int BlockLayer { get; private set; }

    public enum Side
    {
        Left = 0,
        Right = 1, 
        Up = 2,
        Down = 3,
    }

    void Awake()
    {
        ObstacleLayer = LayerMask.NameToLayer("Obstacle");
        BlockLayer = LayerMask.NameToLayer("Block");
    }

    // Use this for initialization
    void Start ()
    {
        if (createBlocks)
            BuildGrid();
	}
	
    private void BuildGrid()
    {
        Vector3 basePos = transform.position;
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int z = 0; z < levelSize.y; z++)
            {
                LevelBlock block = Instantiate(levelBlockPrefab, new Vector3(basePos.x + x, 0, basePos.z + z), Quaternion.identity, levelBlocks).GetComponent<LevelBlock>();
                block.BlockID = z + (x * (int)levelSize.y);
                block.BuilderRef = this;
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
