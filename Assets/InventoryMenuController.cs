using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InventoryMenuController : NetworkBehaviour
{
    [SerializeField] private PlayerDeck deck;
    public GameObject cardInventoryGameObject;
    public NetworkCameraController cameraController;

    public List<CardSlot> cardSlots;
    public List<InventoryCardDisplay> cardDisplays;
    public List<CardScriptableObject> knownCards;

    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }


    public override void OnStartAuthority()
    {
        enabled = true;
        Controls.Player.Inventory.performed += ctx => OpenInventory();
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    void OpenInventory()
    {
        if (cardInventoryGameObject.activeSelf)
        {
            cardInventoryGameObject.SetActive(false);
            cameraController.enabled = true;

            return;
        }

        cameraController.enabled = false;
        ShowCardInventoryUI();
    }

    public void ShowCardInventoryUI()
    {
        cardInventoryGameObject.SetActive(true);

        foreach(CardSlot slot in cardSlots)
        {
            slot.image.enabled = false;
            slot.gameObject.SetActive(false);
        }

        for(int i = 0; i < deck.deckSize; i++)
        {
            cardSlots[i].gameObject.SetActive(true);
        }

        SetCardSlotUI();
        SetInventoryCardSlotUI();
    }

    void SetCardSlotUI()
    {
        deck.Sort();

        for (int i = 0; i < deck.deckSize; i++)
        {
            cardSlots[i].image.enabled = false;
        }

        //deck.deck.Sort();

        int counter = 0;
        foreach (CardScriptableObject card in deck.deck)
        {
            cardSlots[counter].image.enabled = true;
            cardSlots[counter].image.sprite = card.cardSprite;
            counter++;
        }
    }

    void SetInventoryCardSlotUI()
    {
        foreach(InventoryCardDisplay cardDisplay in cardDisplays)
        {
            cardDisplay.gameObject.SetActive(false);
        }

        for (int i = 0; i < knownCards.Count; i++)
        {
            cardDisplays[i].gameObject.SetActive(true);
            cardDisplays[i].SetCard(knownCards[i]);
        }
    }

    public void RemoveCard(int index)
    {
        deck.deck.Remove(deck.deck[index]);

        SetCardSlotUI();
    }

    public void AddCard(CardScriptableObject card)
    {
        deck.deck.Add(card);

        SetCardSlotUI();
    }
}
