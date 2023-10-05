using System.Collections;
using UnityEngine;

public enum EffectType
{
    None = 0,
    Charm = 1,
    Ward = 2,
    Hang = 3
}

[System.Serializable]
public class FightEffect : MonoBehaviour
{
    public EffectType effectType;
    public int strength;
}
