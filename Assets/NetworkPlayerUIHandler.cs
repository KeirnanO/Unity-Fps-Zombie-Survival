using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkPlayerUIHandler : NetworkBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI tooltipText;

    public Graphic playerDamageSprite;

    public GameObject pointsPrefab;
    public GameObject negativePointsPrefab;
    public Transform pointsLocation;
    public GameObject canvas;


    public override void OnStartAuthority()
    {
        enabled = true;
        canvas.SetActive(true);

        GetComponent<Health>().OnHealthChanged += SetHealthSprite;
    }


    public void CreatePointPrefab(int points)
    {
        if (points >= 0)
        {
            TextMeshProUGUI newText = Instantiate(pointsPrefab, pointsLocation.position, Quaternion.identity, transform).GetComponent<TextMeshProUGUI>();

            newText.SetText(points.ToString());
        }
        else
        {
            TextMeshProUGUI newText = Instantiate(negativePointsPrefab, pointsLocation.position, Quaternion.identity, transform).GetComponent<TextMeshProUGUI>();

            newText.SetText(points.ToString());
        }
    }

    [Client]
    public void DisplayToolTip(string text)
    {
        tooltipText.SetText(text);
    }


    [Client]
    public void SetHealthSprite(float health) 
    {
        print("new health = " + health);

        float max = 10;

        float difference = max - health;
        float transparency = 1 * (difference / max);

        float white = health / max;
        Color newColor = new Color(1, white, white, transparency);
        playerDamageSprite.color = newColor;
    }
}
