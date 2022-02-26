using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    private float _health;
    private List<MonsterManager> _cardsOnField;

    public PlayerStat(float health)
    {
        _cardsOnField = null;
        _health = health;
    }

    public float GetHealth()
    {
        return _health;
    }

    public void SetHealth(float health)
    {
        _health = health;
    }

    public List<MonsterManager> GetCardsOnField()
    {
        return _cardsOnField;
    }

    public int SetCardsOnField(MonsterManager monster)
    {
        if(_cardsOnField.Count > 3)
        {
            //No more space
            return 0;
        }
        _cardsOnField.Add(monster);
        //Success
        return 1;
    }
}
