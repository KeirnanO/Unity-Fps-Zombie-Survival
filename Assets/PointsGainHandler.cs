using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsGainHandler : MonoBehaviour
{
    Rigidbody2D rb;
    TextMeshProUGUI text;

    float length = .25f;
    float time;

    public float fadeStrength;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        float rand = Random.Range(-100, 100);

        rb.velocity = new Vector2(200, rand);
        time = Time.time + length;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > time)
        {
            Color color = text.color;

            color.a -= Time.deltaTime * fadeStrength;
            text.color = color;

            if (color.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
