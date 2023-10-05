using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CribbageOpponentHandUI : MonoBehaviour
{
    public static CribbageOpponentHandUI instance;

    private CribbageGamePlayer localPlayer;
    public CribbageGamePlayer LocalPlayer { get { return localPlayer; } }


    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private List<CribbageOpponentCardDisplay> cardDisplays;

    public void SetCardDisplay(List<int> cardIDs)
    {
        for (int i = 0; i < Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count); i++)
        {
            cardDisplays[i].SetCard(CribbageCardDatabase.instance.CardBackDatabase[0]);
            cardDisplays[i].gameObject.SetActive(true);
        }

        for (int i = cardIDs.Count; i < cardDisplays.Count; i++)
        {
            cardDisplays[i].SetCard(null);
            cardDisplays[i].gameObject.SetActive(false);
        }

        Vector3 cardOffset = Vector3.right * 200;
        Vector3 evenOffset = Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count) % 2 == 0 ? cardOffset / 2 : Vector3.zero;
        int mid = Mathf.FloorToInt(Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count) / 2);

        for (int i = 0; i < Mathf.Clamp(cardIDs.Count, 0, cardDisplays.Count); i++)
        {
            int xOffset = i - mid;

            cardDisplays[i].SetPosition(Vector3.zero + (cardOffset * xOffset) + evenOffset);
        }
    }

    public void SetLocalPlayer(CribbageGamePlayer player)
    {
        localPlayer = player;
    }
}
