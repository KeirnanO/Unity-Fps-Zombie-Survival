using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIHandler : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI tooltipText;

    public Loadout loadout;
    public NetworkPlayerController player;
    public Spawner spawner;

    public Graphic playerDamageSprite;

    public GameObject pointsPrefab;
    public GameObject negativePointsPrefab;
    public Transform pointsLocation;

    static public ClientUIHandler instance;


    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (player == null)
            return;

        if(player.GetLoadout().GetCurrentGun() != null)
            ammoText.SetText(player.GetLoadout().GetCurrentGun().ammoInClip + " / " + player.GetLoadout().GetCurrentGun().ammo);

        //roundText.SetText(spawner.round.ToString());
        pointsText.SetText(player.GetPoints().ToString());


        float difference = player.maxHealth - player.GetHealth();
        float transparency = 1 * (difference / player.maxHealth);

        float white = player.GetHealth() / player.maxHealth;
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

    public void SetPlayer(NetworkPlayerController _player)
    {
        player = _player;
    }
}
