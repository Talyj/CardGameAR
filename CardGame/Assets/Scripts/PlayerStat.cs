using Com.MyCompany.MyGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviourPun, IPunObservable
{
    public GameManager gameManager;
    public string playerId;

    private float _health;
    //TODO might have to put that in GameManager
    public List<Mobs> _cardsOnField;
    private List<Mobs> _cardsInHand;

    [HideInInspector]public int currentTurn;

    //player1
    //public Image P1headImg;
    //public Image P1numPlayer;
    ////public Image[] life;
    //public Text P1textLife;

    ////player1
    //public Image P2headImg;
    //public Image P2numPlayer;
    ////public Image[] life;
    //public Text P2textLife;

    public void Start()
    {
        
    }

    public void Update()
    {
        Debug.Log("tour : " + currentTurn);
        if (photonView.IsMine)
        {

        }
    }

    public void UIUpdate()
    {
        //turnNumber.text = turn.ToString();

        //if (player1 != null && player2 != null)
        //{
        //    vieJoueur1Vue1.text = player1.GetHealth().ToString() + "/100";
        //    vieJoueur2Vue1.text = player2.GetHealth().ToString() + "/100";
        //    vieJoueur1Vue2.text = player1.GetHealth().ToString() + "/100";
        //    vieJoueur2Vue2.text = player2.GetHealth().ToString() + "/100";

        //    if (player1.GetHealth() <= 0)
        //    {
        //        victoryPanelP2.SetActive(true);
        //    }

        //    if (player2.GetHealth() <= 0)
        //    {
        //        victoryPanelP1.SetActive(true);
        //    }

        //    for (int i = 100; i > 0; i = i - 10)
        //    {
        //        if (player1.GetHealth() < i && player1.GetHealth() >= (i - 10))
        //        {
        //            for (int j = (i / 10) - 1; j < 10; j++)
        //            {
        //                lifePlayer1Vue1[j].SetActive(false);
        //                lifePlayer1Vue2[j].SetActive(false);
        //            }
        //            lifePlayer1Vue1[(i / 10) - 1].SetActive(true);
        //            lifePlayer1Vue2[(i / 10) - 1].SetActive(true);
        //            for (int j = (i / 10) - 1; j > 0; j--)
        //            {
        //                lifePlayer1Vue1[j - 1].SetActive(true);
        //                lifePlayer1Vue2[j - 1].SetActive(true);
        //            }
        //        }
        //    }

        //    for (int i = 100; i > 0; i = i - 10)
        //    {
        //        if (player2.GetHealth() < i && player2.GetHealth() >= (i - 10))
        //        {
        //            for (int j = (i / 10) - 1; j < 10; j++)
        //            {
        //                lifePlayer2Vue1[j].SetActive(false);
        //                lifePlayer2Vue2[j].SetActive(false);
        //            }
        //            lifePlayer2Vue1[(i / 10) - 1].SetActive(true);
        //            lifePlayer2Vue2[(i / 10) - 1].SetActive(true);
        //            for (int j = (i / 10) - 1; j > 0; j--)
        //            {
        //                lifePlayer2Vue1[j - 1].SetActive(true);
        //                lifePlayer2Vue2[j - 1].SetActive(true);
        //            }
        //        }
        //    }

        //    if (player1.GetHealth() >= 100)
        //    {
        //        lifePlayer1Vue1[9].SetActive(true);
        //        lifePlayer1Vue2[9].SetActive(true);
        //    }

        //    if (player2.GetHealth() >= 100)
        //    {
        //        lifePlayer2Vue1[9].SetActive(true);
        //        lifePlayer2Vue2[9].SetActive(true);
        //    }
        //}
    }

    public PlayerStat()
    {
        _cardsOnField = null;
        _health = 100;
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

    public List<Mobs> GetCardsInHand()
    {
        return _cardsInHand;
    }

    public void SetCardsInHand(Mobs monster)
    {
        _cardsInHand.Add(monster);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GetHealth());
        }
        else
        {
            SetHealth((float)stream.ReceiveNext());
        }
    }
}
