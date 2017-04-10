using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    public enum AbilityType
    {
        Mine = 1,
    }

    [SerializeField]
    private GameObject abilityPrefab = null;
    public GameObject AbilityPrefab { get { return abilityPrefab; } }

    [SerializeField]
    private AbilityType type = AbilityType.Mine;

    [SerializeField]
    private GameObject placementGraphics = null;
    public GameObject PlacementGraphics { get; }

    [SerializeField]
    private int count = 3;
    public int Count { get { return count; } }

    [Header("Ingame HUD")]
    [SerializeField]
    private Text uiCount = null;

    public void UpdateCount(int count)
    {
        this.count = count;
        uiCount.text = "x" + count.ToString();
    }

    // Use this for initialization
    void Start ()
    {
        UpdateCount(count);

        EventTrigger eventTgr = GetComponent<EventTrigger>();
        Helpers.AddEventTrigger(eventTgr, () => { GameManager.Instance.OnUseAbility(gameObject); }, EventTriggerType.PointerDown);
    }
}
