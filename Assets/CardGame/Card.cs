using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;

    public Card()
    {

    }

    public Card(int _id, string _cardName, int _cost, int _power, string _cardDescription)
    {
        id = _id;
        cardName = _cardName;
        cost = _cost;
        power = _power;
        cardDescription = _cardDescription;


    }
}
