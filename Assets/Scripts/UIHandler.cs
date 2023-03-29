using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI tooltipText;

    public Loadout loadout;
    public AimController player;
    public Spawner spawner;

    public Graphic playerDamageSprite;

    public GameObject pointsPrefab;
    public GameObject negativePointsPrefab;
    public Transform pointsLocation;

    static public UIHandler instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        ammoText.SetText(loadout.GetCurrentGun().ammoInClip + " / " + loadout.GetCurrentGun().ammo);
        roundText.SetText(spawner.round.ToString());
        pointsText.SetText(player.GetPoints().ToString());


        float difference = player.maxHealth - player.health;
        float transparency = 1 * (difference / player.maxHealth);

        float white = player.health / player.maxHealth;
        Color newColor = new Color(1, white, white, transparency);
        playerDamageSprite.color = newColor;
    }

    public void CreatePointPrefab(int points)
    {
        if(points >= 0)
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

    public void DisplayToolTip(string text)
    {
        tooltipText.SetText(text);
    }
}
