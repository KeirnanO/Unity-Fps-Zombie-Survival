using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "WandItem")]
public class WandItemObject : ItemObject
{
    public CardScriptableObject attackCard;
    public CardScriptableObject abilityCard;
}
