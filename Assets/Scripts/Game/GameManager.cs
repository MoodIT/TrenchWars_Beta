using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private LevelBlock curSelected = null;

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
            Debug.LogError("ClickStart!");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << levelBuilder.BlockLayer);
            if (hit)
            {
                Debug.LogError("ClickHit!");
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if(block)
                {
                    if(block.IsDigged)
                    {
                        if(waitingToSpawn != null)
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
                            if (curSelected != null)
                                curSelected.IsSelected = false;
                            block.IsSelected = true;
                            Debug.Log(block.name + " Selected");
                        }
                        else
                        {
                            block.IsSelected = false;
                            block.Dig();
                            curSelected = null;

                            Debug.Log(block.name + " Digged");
                        }
                    }
                }
                Debug.Log("Hit " + hitInfo.transform.gameObject.name + "    " + hitInfo.distance);
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
