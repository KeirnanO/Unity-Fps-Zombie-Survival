using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public RectTransform rectTransform;

    public float maxHeight;
    float minHeight;
    public float floatTime;

    float floatDeltaTime;

    public void SetDamageText(string text)
    {
        damageText.text = text;        
    }

    private void Start()
    {
        floatDeltaTime = Time.time;
        minHeight = rectTransform.position.y;
    }

    private void Update()
    {
        float newHeight = Mathf.Lerp(minHeight, maxHeight + minHeight, (Time.time - floatDeltaTime) / floatTime);

        rectTransform.position = new Vector3(rectTransform.position.x, newHeight, rectTransform.position.z);

        if (newHeight >= maxHeight)
            Destroy(gameObject);
    }
}
