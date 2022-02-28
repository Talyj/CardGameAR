using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
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

        private int turn;
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
        [SerializeField] private GameObject victoryP1;
        [SerializeField] private GameObject victoryP2;

        //[SerializeField] private GameObject[] texts;
        [SerializeField] private GameObject drawText;
        [SerializeField] private GameObject playText;
        [SerializeField] private GameObject battleText;

        private void Start()
        {
            #region game rules
            turn = 1;
            isPlaying = true;
            state = gameState.drawPhase;
            first = true;
            currentCardNumber = 0;
            oldCardNumber = 0;
            isTargetFound = false;
            #endregion
        }

        private void Update()
        {
            if (isPlaying)
            {
                MainGame();
                Debug.Log("tour numéro : " + turn);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        #region game rules
        private void MainGame()
        {
            if (first)
            {
                player1 = new PlayerStat(100);
                player2 = new PlayerStat(100);
                first = false;
            }
            if (SceneManager.GetActiveScene().name == "Game")
            {
                if (turn % 2 == 0)
                {
                    //Tour 2 P2
                    GameLoop(player2, player1);
                }
                else
                {
                    //Tour 1 P1
                    GameLoop(player1, player2);
                }
                CheckVictory(player1, player2);
            }
        }

        private void GameLoop(PlayerStat playerTurn, PlayerStat otherPlayer)
        {
            switch (state)
            {
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
                        //#region DRAW
                        //if (turn == 1 || turn == 2)
                        //{
                        //    DrawCard(playerTurn, true);
                        //}
                        //else
                        //{
                        //    DrawCard(playerTurn);
                        //}
                        //#endregion

                        break;
                    }
                case gameState.mainPhase:
                    {
                        playText.SetActive(true);
                        if (isTargetFound)
                        {
                            playerTurn._cardsOnField = new List<Mobs>();
                            isTargetFound = false;
                            var cards = FindObjectsOfType<DefaultObserverEventHandler>();
                            foreach (var c in cards)
                            {
                                if (isTrackingMarker(c.name))
                                {
                                    var card = c.GetComponentInChildren<Mobs>();
                                    playerTurn.SetCardsOnField(card);

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
        }

        public void TargetFound()
        {
            isTargetFound = true;
        }

        private bool isTrackingMarker(string imageTargetName)
        {
            try
            {
                var imageTarget = GameObject.Find(imageTargetName);
                var trackable = imageTarget.GetComponent<TrackableBehaviour>();
                var status = trackable.CurrentStatus.ToString();
                return status == "TRACKED";
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
                victoryP2.SetActive(true);
                //StartCoroutine(DoAfter(3));
            }
            if (p2.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
                victoryP1.SetActive(true);
                //StartCoroutine(DoAfter(3));
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
                        turn++;
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
        }

        public void ReturnMenu()
        {
            SceneManager.LoadScene("Launcher");
        }
        #endregion
    }
}

