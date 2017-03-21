using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Ingame HUD")]
    [SerializeField]
    private Text uiSupplies = null;
    [SerializeField]
    private Animator animSupplies = null;

    [SerializeField]
    private Text uiTrensies = null;
    [SerializeField]
    private Animator animTrensies = null;

    [SerializeField]
    private Text uiTime = null;

    [SerializeField]
    private RectTransform uiTimeNeedle = null;

    [SerializeField]
    private List<Image> uiStarsEmpty = null;

    [SerializeField]
    private Image uiProgress = null;

    public void UpdateTimer(float timeLeft, float levelTime)
    {
        if (timeLeft <= 0)
        {
            uiStarsEmpty[0].enabled = true;
            uiTime.text = "0:00";
            return;
        }

        uiTime.text = (int)Math.Floor(timeLeft / 60f) + ":" + ((int)(timeLeft % 60)).ToString("00");

        float pctDone = timeLeft / levelTime;
        uiTimeNeedle.rotation = Quaternion.Euler(0, 0, 360f * (1 - pctDone));

        uiProgress.fillAmount = pctDone;

        if (pctDone < 0.5f && !uiStarsEmpty[2].enabled)
            uiStarsEmpty[2].enabled = true;
        else if (pctDone < 0.25f && !uiStarsEmpty[1].enabled)
            uiStarsEmpty[1].enabled = true;
    }

    public void UpdateSupplies()
    {
        StartCoroutine(PlayUIAnim(animSupplies, "newValue", "UI_HUD_ValueIncrease"));

        uiSupplies.text = GameManager.Instance.Supplies.ToString();
    }

    public void UpdateTrensies(int count, int total)
    {
        StartCoroutine(PlayUIAnim(animTrensies, "newValue", "UI_HUD_ValueIncrease"));

        uiTrensies.text = count + "/" + total;
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

    //Button events
    public void OnGamePause()
    {
        Debug.LogError("PAUSE!!");
    }
}
