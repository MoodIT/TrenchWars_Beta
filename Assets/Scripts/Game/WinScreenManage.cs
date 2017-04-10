using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenManage : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private EventTrigger resetButton = null;

    [SerializeField]
    private EventTrigger homeButton = null;

    [Header("HUD")]
    [SerializeField]
    private Text uiScore = null;

    [SerializeField]
    private List<Image> uiStars = null;
    [SerializeField]
    private List<Animator> animStars = null;

    public int Coins { get; set; }
    public int Stars { get; set; }

    private int maxScoreCountTime = 5;

    private void Start()
    {
        Helpers.AddEventTrigger(resetButton, () => { StartCoroutine(RestartGame()); }, EventTriggerType.PointerClick);

        Helpers.AddEventTrigger(homeButton, () => 
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }, EventTriggerType.PointerClick);

        StartCoroutine(FillInScore());
    }

    protected IEnumerator FillInScore()
    {
        if (Coins > 0)
        {
            for (int i = 0; i <= Coins; i+=10)
            {
                uiScore.text = i.ToString();
                yield return null;
            }
        }
        uiScore.text = Coins.ToString();

        for (int i = 0; i < Stars; i++)
        {
            uiStars[i].enabled = true;
            StartCoroutine(PlayUIAnim(animStars[i], "newValue", "UI_StarAchieved"));

            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + 0.5)
                yield return null;
        }
    }

    private IEnumerator PlayUIAnim(Animator anim, string triggerName, string newStateName)
    {
        anim.SetBool(triggerName, true);

        while (true)
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

    protected IEnumerator RestartGame()
    {
        ParticleManager.instance.RemoveAll();

        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + 1)
            yield return null;

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
