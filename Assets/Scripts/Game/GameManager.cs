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

    [SerializeField]
    private LevelBuilder levelBuilder = null;
    public LevelBuilder Builder { get { return levelBuilder; } }

    [Header("Initial Data")]
    [SerializeField]
    private int supplies = 0;

    [SerializeField]
    private int maxTrensies = 1;
    public int MaxTrensies { get { return maxTrensies; } }

    HashSet<Character_Base> Trensies = new HashSet<Character_Base>();

    [SerializeField]
    private int coins = 0;

    [SerializeField]
    private int gameTimeSec = 100;

    private float timeLeft = float.MaxValue;

    [Header("Ingame HUD")]
    [SerializeField]
    private Text uiSupplies = null;

    [SerializeField]
    private Text uiTrensies = null;
    [SerializeField]
    private Animator animTrensies = null;

    [SerializeField]
    private Text uiCoins = null;
    [SerializeField]
    private Animator animCoins = null;

    [SerializeField]
    private Text uiTime = null;

    [SerializeField]
    private RectTransform uiTimeNeedle = null;

    [SerializeField]
    private List<Image> uiStarsEmpty = null;

    [SerializeField]
    private Image uiProgress = null;
       
    public GameManager()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
        timeLeft = gameTimeSec;
    }

    void Update()
    {
        if (gameEnded)
            return;

        UpdateUI_Timer();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50, 1 << levelBuilder.BlockLayer);
            if (hit)
            {
                LevelBlock block = hitInfo.collider.GetComponent<LevelBlock>();
                if(block)
                {
                    if (!block.IsDiggable)
                    {
                        Debug.Log(block.name + " Blocked");
                    }
                    else
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

    private void UpdateUI_Timer()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            gameEnded = true;
            uiStarsEmpty[0].enabled = true;
            uiTime.text = "0:00";
            return;
        }

        uiTime.text = (int)Math.Floor(timeLeft / 60f) + ":" + ((int)(timeLeft % 60)).ToString("00");

        float pctDone = timeLeft / (float)gameTimeSec;
        uiTimeNeedle.rotation = Quaternion.Euler(0, 0, 360f * (1 - pctDone));

        uiProgress.fillAmount = pctDone;

        if (pctDone < 0.5f && !uiStarsEmpty[2].enabled)
            uiStarsEmpty[2].enabled = true;
        else if (pctDone < 0.25f && !uiStarsEmpty[1].enabled)
            uiStarsEmpty[1].enabled = true;
    }

    private void UpdateUI()
    {
        uiSupplies.text = supplies.ToString();
        uiTrensies.text = Trensies.Count + "/" + maxTrensies;
        uiCoins.text = coins.ToString();
    }

    public void AddPlayer(Character_Base player)
    {
        Trensies.Add(player);

        StartCoroutine(PlayUIAnim(animTrensies, "newValue", "UI_HUD_ValueIncrease"));

        UpdateUI();
    }

    public void RemovePlayer(Character_Base player)
    {
        if(Trensies.Contains(player))
            Trensies.Remove(player);

        UpdateUI();
    }

    public void AddCoins(int count)
    {
        coins += count;

        StartCoroutine(PlayUIAnim(animCoins, "newValue", "UI_HUD_ValueIncrease"));

        UpdateUI();
    }

    private IEnumerator PlayUIAnim(Animator anim, string triggerName, string newStateName)
    {
        anim.SetBool(triggerName, true);

        while(true)
        {
            yield return null;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                break;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(newStateName))
        {
            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    break;
                yield return null;
            }
        }

        anim.SetBool(triggerName, false);
    }

    public void OnPause()
    {
        Debug.Log("GAME PAUSE!");
    }
}
