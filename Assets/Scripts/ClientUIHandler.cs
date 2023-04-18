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
