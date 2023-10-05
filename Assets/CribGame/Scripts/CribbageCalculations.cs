using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CribbageCalculations
{
    public static int score_hand(List<int> hand, int cut, bool is_crib)
    {
        return score_15s(hand, cut) * 2 +
               score_pairs(hand, cut) * 2 +
               score_runs(hand, cut) +
               score_flush(hand, cut, is_crib) * 4 +
               score_nobs(hand, cut) * 1;
    }

    public static int score_15s(List<int> hand, int cut)
    {
        var a = Mathf.Clamp((int)CribbageCardDatabase.instance.CardDatabase[hand[0]].value + 1, 1, 10);
        var b = Mathf.Clamp((int)CribbageCardDatabase.instance.CardDatabase[hand[1]].value + 1, 1, 10);
        var c = Mathf.Clamp((int)CribbageCardDatabase.instance.CardDatabase[hand[2]].value + 1, 1, 10);
        var d = Mathf.Clamp((int)CribbageCardDatabase.instance.CardDatabase[hand[3]].value + 1, 1, 10);
        var e = Mathf.Clamp((int)CribbageCardDatabase.instance.CardDatabase[cut].value + 1, 1, 10);
        int num_15s = 0;

        // five cards - C(5,5)=1
        if (a + b + c + d + e == 15)
            ++num_15s;

        // four cards - C(5,4)=5
        if (a + b + c + d == 15)
            ++num_15s;
        if (a + b + c + e == 15)
            ++num_15s;
        if (a + b + d + e == 15)
            ++num_15s;
        if (a + c + d + e == 15)
            ++num_15s;
        if (b + c + d + e == 15)
            ++num_15s;

        // three cards - C(5,3)=10
        if (a + b + c == 15)
            ++num_15s;
        if (a + b + d == 15)
            ++num_15s;
        if (a + b + e == 15)
            ++num_15s;
        if (a + c + d == 15)
            ++num_15s;
        if (a + c + e == 15)
            ++num_15s;
        if (a + d + e == 15)
            ++num_15s;
        if (b + c + d == 15)
            ++num_15s;
        if (b + c + e == 15)
            ++num_15s;
        if (b + d + e == 15)
            ++num_15s;
        if (c + d + e == 15)
            ++num_15s;

        // two cards - C(5,2)=10
        if (a + b == 15)
            ++num_15s;
        if (a + c == 15)
            ++num_15s;
        if (a + d == 15)
            ++num_15s;
        if (a + e == 15)
            ++num_15s;
        if (b + c == 15)
            ++num_15s;
        if (b + d == 15)
            ++num_15s;
        if (b + e == 15)
            ++num_15s;
        if (c + d == 15)
            ++num_15s;
        if (c + e == 15)
            ++num_15s;
        if (d + e == 15)
            ++num_15s;

        return num_15s;
    }
    public static int score_pairs(List<int> hand, int cut) {
        var a = (int)CribbageCardDatabase.instance.CardDatabase[hand[0]].value;
        var b = (int)CribbageCardDatabase.instance.CardDatabase[hand[1]].value;
        var c = (int)CribbageCardDatabase.instance.CardDatabase[hand[2]].value;
        var d = (int)CribbageCardDatabase.instance.CardDatabase[hand[3]].value;
        var e = (int)CribbageCardDatabase.instance.CardDatabase[cut].value;
        int num_pairs = 0;

      if (a == b)
        ++num_pairs;
      if (a == c)
        ++num_pairs;
      if (a == d)
        ++num_pairs;
      if (a == e)
        ++num_pairs;

      if (b == c)
        ++num_pairs;
      if (b == d)
        ++num_pairs;
      if (b == e)
        ++num_pairs;

      if (c == d)
        ++num_pairs;
      if (c == e)
        ++num_pairs;

      if (d == e)
        ++num_pairs;

      return num_pairs;
    }

    
    public static int score_runs(List<int> hand, int cut) {

        List<int> handValues = new List<int>();

        var value = (int)CribbageCardDatabase.instance.CardDatabase[cut].value;
        handValues.Add(value);

        foreach (int cardID in hand)
        {
            value = (int)CribbageCardDatabase.instance.CardDatabase[cardID].value;
            handValues.Add(value);
        }        

        handValues.Sort();

        //Depth of the run [lowest value is 1]
        int runIndex = 1;
        //Amount of cards in the run
        int runValue = 1;
        //Amount of total runs in the hand
        int runs = 1;
        //Total score from runs
        int score = 0;

        int prevValue = handValues[0];
        for(int i = 1; i < handValues.Count; i++)
        {
            if (handValues[i] == prevValue)
            {
                runs += runIndex;
            }
            else if (handValues[i] == prevValue + 1)
            {
                runIndex++;
                runValue++;
            }
            else
            {
                if (runValue > 2)
                    score += runs * runValue;

                runs = 1;
                runIndex = 1;
                runValue = 1;
            }

            prevValue = handValues[i];
        }

        if (runValue > 2)
            score += runs * runValue;

        return score;
    }
    

    public static int score_flush(List<int> hand, int cut, bool is_crib) {
        var a = (int)CribbageCardDatabase.instance.CardDatabase[hand[0]].suit;
        var b = (int)CribbageCardDatabase.instance.CardDatabase[hand[1]].suit;
        var c = (int)CribbageCardDatabase.instance.CardDatabase[hand[2]].suit;
        var d = (int)CribbageCardDatabase.instance.CardDatabase[hand[3]].suit;
        var e = (int)CribbageCardDatabase.instance.CardDatabase[cut].suit;

        if (a != b)
            return 0;
        if (a != c)
            return 0;
        if (a != d)
            return 0;

        // All 4 cards in `hand` are the same suit

        if (a == e)
            return 1;

        // In the crib, a flush counts only if all five cards are the same suit.
        if (is_crib)
            return 0;

        return 1;
    }

    public static int score_nobs(List<int> hand, int cut)
    {
        Suit suit = CribbageCardDatabase.instance.CardDatabase[cut].suit;

        var a = CribbageCardDatabase.instance.CardDatabase[hand[0]];
        if (a.value == Value.Jack && a.suit == suit)
            return 1;

        var b = CribbageCardDatabase.instance.CardDatabase[hand[1]];
        if (b.value == Value.Jack && b.suit == suit)
            return 1;

        var c = CribbageCardDatabase.instance.CardDatabase[hand[2]];
        if (c.value == Value.Jack && c.suit == suit)
            return 1;

        var d = CribbageCardDatabase.instance.CardDatabase[hand[3]];
        if (d.value == Value.Jack && d.suit == suit)
            return 1;

        return 0;
    }  
}