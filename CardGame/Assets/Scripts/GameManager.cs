using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            endphase = 3
        };

        private float timeBetweenBoards = 2;

        private int turn;
        private gameState state;
        private bool isEndTurn;
        private PlayerStat player1;
        private PlayerStat player2;

        private float health1;
        private float health2;
        private bool first;

        private int currentCardNumber;
        private int oldCardNumber;

        private float damageTurn;
        private float healTurn;

        private bool isPlaying;
        private bool next;
        //[SerializeField] private GameObject[] boards;
        [SerializeField] private GameObject drawBoard;
        [SerializeField] private GameObject mainBoard;
        [SerializeField] private GameObject battleBoard;
        [SerializeField] private GameObject endBoard;
        [SerializeField] private GameObject turnP1;
        [SerializeField] private GameObject turnP2;
        [SerializeField] private GameObject victoryP1;
        [SerializeField] private GameObject victoryP2;

        //[SerializeField] private GameObject[] texts;
        [SerializeField] private GameObject drawText;
        [SerializeField] private GameObject playText;
        [SerializeField] private GameObject battleText;
        [SerializeField] private GameObject endText;

        private void Start()
        {
            #region game rules
            turn = 1;
            isPlaying = true;
            state = gameState.drawPhase;
            isEndTurn = true;
            first = true;
            currentCardNumber = 0;
            oldCardNumber = 0;
            next = false;
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
            //If otherPlayer block the commands 
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
                        if (next)
                        {
                            next = false;
                            if (turn == 1 || turn == 2)
                            {
                                DrawCard(playerTurn, true);
                            }
                            else
                            {
                                DrawCard(playerTurn);
                            }
                            turnP1.SetActive(false);
                            turnP2.SetActive(false);
                        }
                        break;
                    }
                case gameState.mainPhase:
                    {
                        playText.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            CardNumber(true);

                        }
                        if (currentCardNumber != oldCardNumber)
                        {
                            //Have to add the card to the player playing hands
                            oldCardNumber = currentCardNumber;
                            //player1.GetComponent<PlayerManager>().DamagesCalculation(10);
                            //otherPlayer.DamagesCalculation(50);
                            playText.SetActive(false);
                        }
                        break;
                    }
                case gameState.battlePhase:
                    {
                        //Need image target to determine what is being played
                        //Loop trought the list cardsOnField to get damage/effects ...
                        //Let player choose what is the target and caculate damages
                        damageTurn = 0;
                        healTurn = 0;
                        battleText.SetActive(true);
                        //if (Input.GetKeyDown(KeyCode.Z))
                        //{
                        if (playerTurn.GetCardsOnField() != null)
                        {
                            foreach (var c in playerTurn.GetCardsOnField())
                            {
                                if (c.CompareTag("DPS") || c.CompareTag("tank"))
                                {
                                    damageTurn += c.damage;
                                    c.Attack();
                                }
                                else if (c.CompareTag("healer"))
                                {
                                    healTurn += c.damage;
                                    playerTurn.SetHealth(playerTurn.GetHealth() + healTurn); 
                                }
                            }
                            if (otherPlayer.GetCardsOnField() != null)
                            {
                                foreach (var c in otherPlayer.GetCardsOnField())
                                {
                                    if (c.life + c.shield < damageTurn)
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

                            //}

                            battleText.SetActive(false);
                        }
                        break;
                    }
                case gameState.endphase:
                    {
                        //if (Input.GetKeyDown(KeyCode.E))
                        //{

                        //}
                        isEndTurn = false;
                        break;
                    }
            }
        }

        private IEnumerator DoAfter(float time)
        {
            yield return new WaitForSeconds(time);
            ReturnMenu();
        }

        public void Next()
        {
            turnP1.SetActive(false);
            turnP2.SetActive(false);
        }

        private bool DrawCard(PlayerStat player, bool isFirstDraw = false)
        {
            try
            {
                if (isFirstDraw)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        //MODIFY with the monsterManager with the correct ID
                        var cards = UnityEngine.Random.Range(0, 9);
                        player.SetCardsInHand(new MonsterManager());
                    }
                    return true;
                }
                var card = UnityEngine.Random.Range(0, 9);
                player.SetCardsInHand(new MonsterManager());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        #region actions
        private void Draw()
        {
            //TODO
        }

        private void MainPhase()
        {
            //TODO
        }

        private void BattlePhase()
        {
            //TODO
        }

        private void EndPhase()
        {
            //TODO
        }
        #endregion

        private void ResetUI()
        {
            drawBoard.SetActive(false);
            mainBoard.SetActive(false);
            battleBoard.SetActive(false);
            endBoard.SetActive(false);
            turnP1.SetActive(false);
            turnP2.SetActive(false);

            drawText.SetActive(false);
            playText.SetActive(false);
            battleText.SetActive(false);
            endText.SetActive(false);
        }

        public void CheckVictory(PlayerStat p1, PlayerStat p2)
        {
            if (p1.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
                victoryP1.SetActive(true);
                StartCoroutine(DoAfter(3));
            }
            if (p2.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
                victoryP2.SetActive(true);
                StartCoroutine(DoAfter(3));
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

        private IEnumerator ChangeBoard(GameObject boardToDisplay, GameObject boardToHide1, GameObject boardToHide2, GameObject boardToHide3)
        {
            boardToHide1.SetActive(false);
            boardToHide2.SetActive(false);
            boardToHide3.SetActive(false);
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
                        StartCoroutine(ChangeBoard(mainBoard, endBoard, drawBoard, battleBoard));
                        break;
                    }
                case gameState.mainPhase:
                    {
                        state = gameState.battlePhase;
                        StartCoroutine(ChangeBoard(battleBoard, endBoard, drawBoard, mainBoard));
                        break;
                    }
                case gameState.battlePhase:
                    {
                        state = gameState.endphase;
                        StartCoroutine(ChangeBoard(endBoard, mainBoard, drawBoard, battleBoard));
                        break;
                    }
                case gameState.endphase:
                    {
                        state = gameState.drawPhase;
                        StartCoroutine(ChangeBoard(drawBoard, endBoard, mainBoard, battleBoard));
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

