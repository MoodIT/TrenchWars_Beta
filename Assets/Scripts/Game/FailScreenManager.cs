using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FailScreenManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private EventTrigger resetButton = null;

    [Header("Sounds")]
    [SerializeField]
    protected AudioClip failSound = null;

    void Start ()
    {
        Helpers.AddEventTrigger(resetButton, () => { StartCoroutine(RestartGame()); }, EventTriggerType.PointerClick);
    }

    void OnEnable ()
    {
        SoundManager.instance.PlaySound(failSound, gameObject);
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
