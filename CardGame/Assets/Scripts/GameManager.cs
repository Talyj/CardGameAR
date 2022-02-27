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
            //If otherPlayer block the commands 
            switch (state)
            {
                //TODO Pour la partie ou tu tire une carte du deck, au lieu de le faire dans le jeu 
                //je pensais le faire dans la vraie vie en mode on a notre deck à côté de nous et on pioche dedans directement
                //Si ça te convient alors efface la region "DRAW"
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
                        #region DRAW
                        if (turn == 1 || turn == 2)
                        {
                            DrawCard(playerTurn, true);
                        }
                        else
                        {
                            DrawCard(playerTurn);
                        }
                        #endregion
                        
                        break;
                    }
                case gameState.mainPhase:
                    {
                        playText.SetActive(true);
                        //TODO la partie commenté juste en dessous permet de tester sans carte PARCE QUE J'AVAIS PAS DE PTN DE CARTE POUR TESTER
                        //Donc oue c'est un truc que tu peux supprimer quand t'auras des cartes
                        //if (Input.GetKeyDown(KeyCode.A))
                        //{
                        //    CardNumber(true);

                        //}
                        //Jsais pas pourquoi ça marche mais ça marche du coup
                        //il faut remplacer ça par un fonction qui détecte une nouvelle imagetarget mais je sais pas laquelle
                        if (isTargetFound)
                        {
                            isTargetFound = false;
                            //TODO Ajouter l'image target detection, ajoute la carte détectée dans la liste
                            //Remplacer new Mobs avec le monstre détecté lors du spawn
                            playerTurn.SetCardsOnField(new Mobs());

                            //Dans le cas ou Tajma spawn il soigne
                            //TODO Ici on echange le "c" avec l'objet qu'on récupère lorsqu'on détecte le monstre
                            //if (c.CompareTag("tajma") && !c.isUsed)
                            //{
                            //    playerTurn.SetHealth(playerTurn.GetHealth + c.damage);
                            //    c.isUsed = true
                            //}
                            playText.SetActive(false);
                        }
                        break;
                    }
                case gameState.battlePhase:
                    {
                        //Need image target to determine what is being played
                        //Loop trought the list cardsOnField to get damage/effects ...
                        damageTurn = 0;
                        healTurn = 0;
                        battleText.SetActive(true);

                        //TODO c'est juste un test de victory condition qui pourra être effacé plus tard
                        #region test
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            otherPlayer.SetHealth(0);
                        }
                        #endregion
                        if (playerTurn.GetCardsOnField() != null)
                        {
                            foreach (var c in playerTurn.GetCardsOnField())
                            {
                                if (c.CompareTag("DPS"))
                                {
                                    damageTurn += c.damage;
                                    c.Attack();
                                }
                                else if (c.CompareTag("ymir"))
                                {
                                    healTurn += c.damage;
                                    c.Heal();
                                    playerTurn.SetHealth(playerTurn.GetHealth() + healTurn); 
                                }
                                else if (c.CompareTag("ekey"))
                                {
                                    foreach(var ca in playerTurn.GetCardsOnField())
                                    {
                                        ca.life += c.damage;
                                    }
                                }
                            }
                            if (otherPlayer.GetCardsOnField() != null)
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
                        }
                        break;
                    }
                case gameState.endphase:
                    {
                        //Jsais plus à quoi sert cette phase mais nsm on la garde
                        break;
                    }
            }
        }

        public void TargetFound()
        {
            isTargetFound = true;
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

