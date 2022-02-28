using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    private float _health;
    public List<Mobs> _cardsOnField;
    private List<Mobs> _cardsInHand;

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

    public List<Mobs> GetCardsOnField()
    {
        return _cardsOnField;
    }

    public int SetCardsOnField(Mobs monster)
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

    public List<Mobs> GetCardsInHand()
    {
        return _cardsInHand;
    }

    public void SetCardsInHand(Mobs monster)
    {
        _cardsInHand.Add(monster);
    }

}
