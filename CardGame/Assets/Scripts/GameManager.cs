using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;
using Image = UnityEngine.UI.Image;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameManager Instance;

        public GameObject[] playerPrefabs;

        private enum gameState
        {
            //Draw a card
            drawPhase = 0,
            //Here is when we put cards
            mainPhase = 1,
            //Here is when we order attacks
            battlePhase = 2,
        };

        private float timeBetweenBoards = 2;

        public int turn;
        private gameState state;
        private PlayerStat player1;
        private PlayerStat player2;

        private bool first;

        private int currentCardNumber;
        private int oldCardNumber;

        private float damageTurn;
        private float healTurn;

        private bool isPlaying;
        private bool isTargetFound;
        //[SerializeField] private GameObject[] boards;
        [SerializeField] private GameObject drawBoard;
        [SerializeField] private GameObject mainBoard;
        [SerializeField] private GameObject battleBoard;
        [SerializeField] private GameObject turnP1;
        [SerializeField] private GameObject turnP2;

        //[SerializeField] private GameObject[] texts;
        [SerializeField] private GameObject drawText;
        [SerializeField] private GameObject playText;
        [SerializeField] private GameObject battleText;

        [SerializeField] private TextMeshProUGUI turnNumber;
        [SerializeField] private GameObject victoryPanelP1;
        [SerializeField] private GameObject victoryPanelP2;

        [SerializeField] private GameObject[] lifePlayer1Vue1;
        [SerializeField] private GameObject[] lifePlayer2Vue1;
        [SerializeField] private GameObject[] lifePlayer1Vue2;
        [SerializeField] private GameObject[] lifePlayer2Vue2;
        [SerializeField] private Text vieJoueur1Vue1;
        [SerializeField] private Text vieJoueur2Vue1;
        [SerializeField] private Text vieJoueur1Vue2;
        [SerializeField] private Text vieJoueur2Vue2;
        [SerializeField] private GameObject panelJoueur1;
        [SerializeField] private GameObject panelJoueur2;

        //[0] = player 1, [1] = player2
        [SerializeField] private Image[] numplayers;

        //TODO might have to put that in GameManager
        public List<int> _P1CardsOnField;
        public List<int> _P2CardsOnField;

        public GameObject playerPrefab;

        //Same content as the one in PlayerItem
        public Sprite[] avatars;


        public void Awake()
        {
            var pler = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerStat>();
            pler.gameManager = this;
            foreach(var player in PhotonNetwork.PlayerList)
            {
                if (photonView.IsMine)
                {
                    if (pler.GetComponentInChildren<Image>().CompareTag("tete"))
                    {
                        pler.GetComponentInChildren<Image>().sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
                    }
                }
                else
                {
                    if (pler.GetComponentInChildren<Image>().CompareTag("teteE"))
                    {
                        pler.GetComponentInChildren<Image>().sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
                    }
                }
            }

        }

        private void Start()
        {
            #region game rules
            turn = 1;
            isPlaying = false;
            state = gameState.drawPhase;
            first = true;
            currentCardNumber = 0;
            oldCardNumber = 0;
            isTargetFound = false;
            #endregion
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(InstancePlayers());
            }
        }

        void Update()
        {
            UIUpdate();
            if (isPlaying && PhotonNetwork.IsMasterClient)
            {
                MainGame();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public IEnumerator InstancePlayers()
        {
            while(!isPlaying)
            {
                if(PhotonNetwork.PlayerList.Length >= 2)
                {
                    foreach (var play in FindObjectsOfType<PlayerStat>())
                    {
                        if (play.GetComponent<PhotonView>().IsMine)
                        {
                            player1 = play;
                        }
                        else
                        {
                            player2 = play;
                        }
                    }
                    isPlaying = true;
                }
                yield return new WaitForSeconds(5);
            }
        }

        private void UIUpdate()
        {
            turnNumber.text = turn.ToString();

            if (player1 != null && player2 != null)
            {
                vieJoueur1Vue1.text = player1.GetHealth().ToString() + "/100";
                vieJoueur2Vue1.text = player2.GetHealth().ToString() + "/100";
                vieJoueur1Vue2.text = player1.GetHealth().ToString() + "/100";
                vieJoueur2Vue2.text = player2.GetHealth().ToString() + "/100";

                if (player1.GetHealth() <= 0)
                {
                    victoryPanelP2.SetActive(true);
                }

                if (player2.GetHealth() <= 0)
                {
                    victoryPanelP1.SetActive(true);
                }

                for (int i = 100; i > 0; i = i - 10)
                {
                    if (player1.GetHealth() < i && player1.GetHealth() >= (i - 10))
                    {
                        for (int j = (i / 10) - 1; j < 10; j++)
                        {
                            lifePlayer1Vue1[j].SetActive(false);
                            lifePlayer1Vue2[j].SetActive(false);
                        }
                        lifePlayer1Vue1[(i / 10) - 1].SetActive(true);
                        lifePlayer1Vue2[(i / 10) - 1].SetActive(true);
                        for (int j = (i / 10) - 1; j > 0; j--)
                        {
                            lifePlayer1Vue1[j - 1].SetActive(true);
                            lifePlayer1Vue2[j - 1].SetActive(true);
                        }
                    }
                }

                for (int i = 100; i > 0; i = i - 10)
                {
                    if (player2.GetHealth() < i && player2.GetHealth() >= (i - 10))
                    {
                        for (int j = (i / 10) - 1; j < 10; j++)
                        {
                            lifePlayer2Vue1[j].SetActive(false);
                            lifePlayer2Vue2[j].SetActive(false);
                        }
                        lifePlayer2Vue1[(i / 10) - 1].SetActive(true);
                        lifePlayer2Vue2[(i / 10) - 1].SetActive(true);
                        for (int j = (i / 10) - 1; j > 0; j--)
                        {
                            lifePlayer2Vue1[j - 1].SetActive(true);
                            lifePlayer2Vue2[j - 1].SetActive(true);
                        }
                    }
                }

                if (player1.GetHealth() >= 100)
                {
                    lifePlayer1Vue1[9].SetActive(true);
                    lifePlayer1Vue2[9].SetActive(true);
                }

                if (player2.GetHealth() >= 100)
                {
                    lifePlayer2Vue1[9].SetActive(true);
                    lifePlayer2Vue2[9].SetActive(true);
                }
            }
        }

        #region game rules
        private void MainGame()
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                //if (turn % 2 == 0)
                //{
                //    //Tour 2 P2
                //    //panelJoueur1.SetActive(false);
                //    //panelJoueur2.SetActive(true);
                //    GameLoop(player2, player1);
                //}
                //else
                //{
                //    //Tour 1 P1
                //    //panelJoueur1.SetActive(true);
                //    //panelJoueur2.SetActive(false);
                //    GameLoop(player1, player2);
                //}
                if (PhotonNetwork.IsMasterClient)
                {
                    GameLoop();
                    CheckVictory(player1, player2);
                }
            }
        }
        //private void GameLoop(PlayerStat playerTurn, PlayerStat otherPlayer)
        private void GameLoop()
        {
            PlayerStat playerTurn = null;
            PlayerStat otherPlayer = null;
            List<int> cardOnFieldPTurn = new List<int>();
            List<int> cardOnFieldOPTurn = new List<int>();
            if (turn % 2 == 0)
            {
                //Tour 2 P2
                playerTurn = player2;
                //cardOnFieldPTurn = _P2CardsOnField;
                otherPlayer = player1;
                //cardOnFieldOPTurn = _P1CardsOnField;
            }
            else
            {
                //Tour 1 P1
                playerTurn = player1;
                //cardOnFieldPTurn = _P1CardsOnField;
                otherPlayer = player2;
                //cardOnFieldOPTurn = _P2CardsOnField;
            }
            //If otherPlayer block the commands 
            switch (state)
            {
                //TODO Pour la partie ou tu tire une carte du deck, au lieu de le faire dans le jeu 
                //je pensais le faire dans la vraie vie en mode on a notre deck � c�t� de nous et on pioche dedans directement
                //Si �a te convient alors efface la region "DRAW"
                case gameState.drawPhase:
                    {
                        ////setActive text -> Piochez une carte !
                        if (turn % 2 == 0)
                        {
                            turnP2.SetActive(true);
                        }
                        else
                        {
                            turnP1.SetActive(true);
                        }
                        break;
                    }
                case gameState.mainPhase:
                    {
                        playText.SetActive(true);
                        if (isTargetFound)
                        {
                            cardOnFieldPTurn = new List<int>();
                            isTargetFound = false;
                            var cards = FindObjectsOfType<DefaultObserverEventHandler>();
                            foreach (var c in cards)
                            {
                                if (isTrackingMarker(c.name))
                                {
                                    var card = c.GetComponentInChildren<Mobs>();
                                    //SetCardsOnField(card, playerTurn);

                                    if (card.CompareTag("tajma") && !card.isUsed)
                                    {
                                        playerTurn.SetHealth(playerTurn.GetHealth() + card.damage);
                                        card.isUsed = true;
                                    }
                                }
                            }

                            ChangePhase();
                            playText.SetActive(false);
                        }
                        break;
                    }
                case gameState.battlePhase:
                    {
                        damageTurn = 0;
                        healTurn = 0;
                        battleText.SetActive(true);

                        if (turn == 1)
                        {
                            ChangePhase();
                        }
                        if (playerTurn.GetCardsOnField().Count > 0)
                        {
                            foreach (var c in playerTurn.GetCardsOnField())
                            {
                                if (c.CompareTag("dps"))
                                {
                                    damageTurn += c.damage;
                                    c.Attack();
                                }
                                else if (c.CompareTag("healer"))
                                {
                                    healTurn += c.damage;
                                    c.Heal();
                                    playerTurn.SetHealth(playerTurn.GetHealth() + healTurn);
                                }
                                else if (c.CompareTag("ekey"))
                                {
                                    foreach (var ca in playerTurn.GetCardsOnField())
                                    {
                                        ca.life += c.damage;
                                    }
                                }
                            }
                            if (otherPlayer.GetCardsOnField().Count > 0)
                            {
                                foreach (var c in otherPlayer.GetCardsOnField())
                                {
                                    if (c.life < damageTurn)
                                    {
                                        damageTurn -= c.life;
                                        CardNumber(false);
                                        oldCardNumber = currentCardNumber;
                                        c.Die();
                                    }
                                    else
                                    {
                                        c.life -= damageTurn;
                                        c.GetDamage();
                                    }
                                }
                            }
                            else
                            {
                                otherPlayer.SetHealth(otherPlayer.GetHealth() - damageTurn);
                            }

                            battleText.SetActive(false);
                            ChangePhase();
                        }
                        break;
                    }
            }
            if (_P2CardsOnField.Count > 0 || _P1CardsOnField.Count > 0)
            {
                if (turn % 2 == 0)
                {
                    //Tour 2 P2
                    _P2CardsOnField = cardOnFieldPTurn;
                    _P1CardsOnField = cardOnFieldOPTurn;
                }
                else
                {
                    //Tour 1 P1
                    _P1CardsOnField = cardOnFieldPTurn;
                    _P2CardsOnField = cardOnFieldOPTurn;
                }
            }
        }

        public void SetCardsOnField(Mobs monster, List<int> cardOnFildCP)
        {
            if (cardOnFildCP.Count > 3)
            {
                //No more space
            }
            cardOnFildCP.Add(monster.idMonster);
        }

        private bool isTrackingMarker(string imageTargetName)
        {
            try
            {
                //var imageTarget = GameObject.Find(imageTargetName);
                //var trackable = imageTarget.GetComponent<TrackableBehaviour>();
                //var status = trackable.CurrentStatus.ToString();
                //return status == "TRACKED";
            }
            catch (Exception e)
            {
                var toto = 0;
            }
            return true;

        }

        private bool DrawCard(PlayerStat player, bool isFirstDraw = false)
        {
            try
            {
                if (isFirstDraw)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        //MODIFY with the Mobs with the correct ID
                        var cards = UnityEngine.Random.Range(0, 9);
                        player.SetCardsInHand(new Mobs());
                    }
                    return true;
                }
                var card = UnityEngine.Random.Range(0, 9);
                player.SetCardsInHand(new Mobs());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private void ResetUI()
        {
            drawBoard.SetActive(false);
            mainBoard.SetActive(false);
            battleBoard.SetActive(false);
            //endBoard.SetActive(false);
            turnP1.SetActive(false);
            turnP2.SetActive(false);

            drawText.SetActive(false);
            playText.SetActive(false);
            battleText.SetActive(false);
            //endText.SetActive(false);
        }

        public void CheckVictory(PlayerStat p1, PlayerStat p2)
        {
            if (p1.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
            }
            if (p2.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
            }
        }

        public void CardNumber(bool isUp)
        {
            if (isUp)
            {
                currentCardNumber = oldCardNumber + 1;
            }
            else
            {
                currentCardNumber = oldCardNumber - 1;
            }
        }

        private IEnumerator ChangeBoard(GameObject boardToDisplay, GameObject boardToHide1, GameObject boardToHide2)
        {
            boardToHide1.SetActive(false);
            boardToHide2.SetActive(false);
            boardToDisplay.SetActive(true);
            yield return new WaitForSeconds(timeBetweenBoards);
            boardToDisplay.SetActive(false);
            turnP1.SetActive(false);
            turnP2.SetActive(false);
        }

        public void ChangePhase()
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            ResetUI();
            switch (state)
            {
                case gameState.drawPhase:
                    {
                        state = gameState.mainPhase;
                        StartCoroutine(ChangeBoard(mainBoard, drawBoard, battleBoard));
                        break;
                    }
                case gameState.mainPhase:
                    {
                        state = gameState.battlePhase;
                        StartCoroutine(ChangeBoard(battleBoard, drawBoard, mainBoard));
                        break;
                    }
                case gameState.battlePhase:
                    {
                        state = gameState.drawPhase;
                        StartCoroutine(ChangeBoard(drawBoard, mainBoard, battleBoard));
                        turn += 1;
                        if (turn % 2 == 0)
                        {
                            turnP2.SetActive(true);
                        }
                        else
                        {
                            turnP1.SetActive(true);
                        }
                        break;
                    }
            }
            photonView.RPC("SyncValues", RpcTarget.All, new object[] { turn });
            //}
        }

        [PunRPC]
        public void SyncValues(int turn)
        {
            this.turn = turn;
        }

        public void ReturnMenu()
        {
            SceneManager.LoadScene("Launcher");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(turn);
                stream.SendNext((int)state);
            }
            else
            {
                turn = (int)stream.ReceiveNext();
                state = (gameState)stream.ReceiveNext();
            }
        }
        #endregion
    }
}

