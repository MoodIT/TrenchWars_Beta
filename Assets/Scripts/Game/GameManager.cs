﻿using System;
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

    [SerializeField]
    private LevelBuilder levelBuilder = null;
    public LevelBuilder Builder { get { return levelBuilder; } }

    [SerializeField]
    private HUDManager hudManager = null;
    public HUDManager HUD { get { return hudManager; } }

    [Header("Initial Data")]
    [SerializeField]
    private int supplies = 0;
    public int Supplies { get { return supplies; } }

    [SerializeField]
    private int maxTrensies = 1;
    public int MaxTrensies { get { return maxTrensies; } }

    private GameObject waitingToSpawn = null;
    public void SpawnWaitingTrensies(LevelBlock block)
    {
        if (waitingToSpawn == null)
            return;

        Character_Base player = Instantiate(waitingToSpawn, block.transform.position - Vector3.up * .5f, Quaternion.identity, Builder.PlayerParent).GetComponent<Character_Base>();
        player.CurBlock = block;
        waitingToSpawn = null;
    }

    HashSet<Character_Base> Trensies = new HashSet<Character_Base>();

    [SerializeField]
    private int coins = 0;
    public int Coins { get { return coins; } }

    [SerializeField]
    private int gameTimeSec = 100;
    private float timeLeft = float.MaxValue;

    public GameManager()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = gameTimeSec;

        //Init HUD
        HUD.UpdateTimer(timeLeft, gameTimeSec);
        HUD.UpdateCoins();
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
            gameEnded = true;
            return;
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
                    Debug.LogError("FOUND GOAL");
                    nextBlock = node;
                    Stack<LevelBlock> path = new Stack<LevelBlock>();
                    while(nextBlock.prev != null)
                    {
                        path.Push(nextBlock.block);
                        nextBlock = nextBlock.prev;
                        Debug.LogError("BLOCK " + nextBlock.block.BlockID);
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
        if (block.IsDigged)
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
            {
                Debug.LogError("ADDING " + block.BlockID);
                newNodes.Add(node);
            }
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
        if (coins - unit.Cost < 0)
        {
            Debug.LogError("not enough money");
            return;
        }

        if(Trensies.Count >= MaxTrensies)
        {
            Debug.LogError("too many trensies");
            return;
        }

        coins -= unit.Cost;

        HUD.UpdateCoins();
        waitingToSpawn = unit.CharacterPrefab;
    }
}
