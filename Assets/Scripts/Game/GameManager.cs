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
        if (waitingToSpawn != null)
        {
            Character_Base player = Instantiate(waitingToSpawn, block.transform.position - Vector3.up * .5f, Quaternion.identity, Builder.PlayerParent).GetComponent<Character_Base>();
            player.CurBlock = block;
            waitingToSpawn = null;
        }
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
    }
/*        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << levelBuilder.BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if (block)
                {
                    if (block.IsDigged)
                    {
                        if (waitingToSpawn != null)
                        {
                            Character_Base player = Instantiate(waitingToSpawn, block.transform.position - Vector3.up * .5f, Quaternion.identity, Builder.PlayerParent).GetComponent<Character_Base>();
                            player.CurBlock = block;
                            waitingToSpawn = null;
                            Debug.LogError("Created!");
                        }
                    }
                    else if (block.IsDiggable)
                    {
                        if (!block.IsSelected)
                        {
                            if (curSelected.Count != 0)
                                DeselectAllBlocks();//clear selection
                            else if (block.Cost <= coins && IsNeighborDiggable(block.BlockID))
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
        }
    }

    private IEnumerator BlockSelector(float delay = 0.1f)
    {
        while(Input.GetMouseButton(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << levelBuilder.BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if (block && block.IsDiggable && !block.IsSelected && (block.Cost + selectedCost) <= coins && IsNeighborDiggable(block.BlockID))
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
        foreach (LevelBlock block in curSelected)
        {
            coins -= block.Cost;
            HUD.UpdateCoins();

            block.Dig();
            block.IsSelected = false;

            yield return new WaitForSeconds(delay);
        }

        curSelected.Clear();
        selectedCost = 0;
    }

    private bool IsNeighborDiggable(int blockID)
    {
        LevelBlock left = Builder.GetNeighbor(LevelBuilder.Side.Left, blockID);
        if (left.IsDigged || left.IsSelected)
            return true;
        LevelBlock up = Builder.GetNeighbor(LevelBuilder.Side.Up, blockID);
        if (up.IsDigged || up.IsSelected)
            return true;
        LevelBlock down = Builder.GetNeighbor(LevelBuilder.Side.Down, blockID);
        if (down.IsDigged || down.IsSelected)
            return true;
        LevelBlock right = Builder.GetNeighbor(LevelBuilder.Side.Right, blockID);
        if (right.IsDigged || right.IsSelected)
            return true;
        return false;
    }

    private void DeselectAllBlocks()
    {
        foreach (LevelBlock block in curSelected)
            block.IsSelected = false;
        curSelected.Clear();
        selectedCost = 0;
    }
    */
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
