using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    [SerializeField]
    private GameObject characterPrefab = null;
    public GameObject CharacterPrefab { get { return characterPrefab; } }

    [SerializeField]
    private int cost = 20;
    public int Cost { get { return cost; } }

    [Header("Ingame HUD")]
    [SerializeField]
    private Text uiCost = null;

    [SerializeField]
    private GameObject activeCard = null;

    [SerializeField]
    private GameObject inactiveCard = null;

    private void Start()
    {
        uiCost.text = cost.ToString();

        EventTrigger eventTgr = GetComponent<EventTrigger>();
        Helpers.AddEventTrigger(eventTgr, () => { GameManager.Instance.OnBuyTrensie(gameObject); }, EventTriggerType.PointerDown);
    }

    private void Update()
    {
        activeCard.SetActive(GameManager.Instance.Supplies >= cost);
        inactiveCard.SetActive(GameManager.Instance.Supplies < cost);
    }
}
