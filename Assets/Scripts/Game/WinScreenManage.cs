using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinScreenManage : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private EventTrigger resetButton = null;

    [SerializeField]
    private EventTrigger homeButton = null;

    private void Start()
    {
        Helpers.AddEventTrigger(resetButton, () => { StartCoroutine(RestartGame()); }, EventTriggerType.PointerClick);

        Helpers.AddEventTrigger(homeButton, () => { SceneManager.LoadScene(0, LoadSceneMode.Single); }, EventTriggerType.PointerClick);
    }

    protected IEnumerator RestartGame()
    {
        ParticleManager.instance.RemoveAll();
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
