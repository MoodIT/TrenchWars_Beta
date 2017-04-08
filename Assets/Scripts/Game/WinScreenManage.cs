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

        Helpers.AddEventTrigger(homeButton, () => { SceneManager.LoadScene(0, LoadSceneMode.Single); }, EventTriggerType.PointerClick);

        StartCoroutine(FillInScore());
    }

    protected IEnumerator FillInScore()
    {
        if (Coins > 0)
        {
            float timeDelay = maxScoreCountTime / (Coins + Mathf.Epsilon);
            for (int i = 0; i <= Coins; i+=10)
            {
                uiScore.text = i.ToString();
                yield return new WaitForSeconds(timeDelay);
            }
        }
        uiScore.text = Coins.ToString();

        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < Stars; i++)
        {
            uiStars[i].enabled = true;
            StartCoroutine(PlayUIAnim(animStars[i], "newValue", "UI_StarAchieved"));
            yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
