using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        uiCost.text = cost.ToString();

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate { GameManager.Instance.OnBuyTrensie(gameObject); } );
    }
}
