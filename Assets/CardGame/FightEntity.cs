using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FightEntity
{
    public FightPlayer player;
    public CardBattleEnemyController monster;
    public Health health;
    public Mana mana;

    [SerializeField] private List<FightEffect> fightEffects;

    public FightEntity()
    {
    }

    public FightEntity(FightPlayer fightPlayer)
    {
        player = fightPlayer;
        fightEffects = new List<FightEffect>();

        mana = fightPlayer.GetComponent<Mana>();
        health = fightPlayer.GetComponent<Health>();
    }

    public FightEntity (CardBattleEnemyController fightEnemy)
    {
        monster = fightEnemy;
        fightEffects = new List<FightEffect>();

        mana = fightEnemy.GetComponent<Mana>();
        health = fightEnemy.GetComponent<Health>();
    }

    public void AddEffect(FightEffect newEffect)
    {
        fightEffects.Add(newEffect);
    }

    public void RemoveEffect(FightEffect newEffect)
    {
        fightEffects.Remove(newEffect);
    }

    public bool IsDead()
    {
        return health.IsDead;
    }


    public List<FightEffect> GetEffectsByType(EffectType type)
    {
        List<FightEffect> filteredFEList = new List<FightEffect>();

        foreach(var effect in fightEffects)
        {
            if (effect.effectType == type)
                filteredFEList.Add(effect);
        }

        return filteredFEList;
    }
}