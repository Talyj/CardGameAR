using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        private PlayerStat player1 = new PlayerStat(1);
        private PlayerStat player2 = new PlayerStat(1);

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

        [SerializeField] private TextMeshProUGUI turnNumber;
        [SerializeField] private GameObject victoryPanelP1;
        [SerializeField] private GameObject victoryPanelP2;

        [SerializeField] private GameObject[] lifePlayer1Vue1;
        [SerializeField] private GameObject[] lifePlayer2Vue1;
        [SerializeField] private GameObject[] lifePlayer1Vue2;
        [SerializeField] private GameObject[] lifePlayer2Vue2;
        [SerializeField] private TextMeshProUGUI vieJoueur1Vue1;
        [SerializeField] private TextMeshProUGUI vieJoueur2Vue1;
        [SerializeField] private TextMeshProUGUI vieJoueur1Vue2;
        [SerializeField] private TextMeshProUGUI vieJoueur2Vue2;
        [SerializeField] private GameObject panelJoueur1;
        [SerializeField] private GameObject panelJoueur2;

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

        void Update() {
            turnNumber.text = turn.ToString();
            vieJoueur1Vue1.text = player1.GetHealth().ToString() + "/100";
            vieJoueur2Vue1.text = player2.GetHealth().ToString() + "/100";
            vieJoueur1Vue2.text = player1.GetHealth().ToString() + "/100";
            vieJoueur2Vue2.text = player2.GetHealth().ToString() + "/100";
            if(player1.GetHealth() <= 0) {
                victoryPanelP2.SetActive(true);
            }

            if(player2.GetHealth() <= 0) {
                victoryPanelP1.SetActive(true);
            }

            for(int i = 100; i > 0; i=i-10) {
                if(player1.GetHealth() < i && player1.GetHealth() >= (i-10)) {
                    for(int j = (i/10)-1; j < 10; j++) {
                        lifePlayer1Vue1[j].SetActive(false);
                        lifePlayer1Vue2[j].SetActive(false);
                    }
                    lifePlayer1Vue1[(i/10)-1].SetActive(true);
                    lifePlayer1Vue2[(i/10)-1].SetActive(true);
                    for(int j = (i/10)-1; j > 0; j--) {
                        lifePlayer1Vue1[j-1].SetActive(true);
                        lifePlayer1Vue2[j-1].SetActive(true);
                    }
                }
            }

            for(int i = 100; i > 0; i=i-10) {
                if(player2.GetHealth() < i && player2.GetHealth() >= (i-10)) {
                    for(int j = (i/10)-1; j < 10; j++) {
                        lifePlayer2Vue1[j].SetActive(false);
                        lifePlayer2Vue2[j].SetActive(false);
                    }
                    lifePlayer2Vue1[(i/10)-1].SetActive(true);
                    lifePlayer2Vue2[(i/10)-1].SetActive(true);
                    for(int j = (i/10)-1; j > 0; j--) {
                        lifePlayer2Vue1[j-1].SetActive(true);
                        lifePlayer2Vue2[j-1].SetActive(true);
                    }
                }
            }

            if(player1.GetHealth() >= 100) {
                lifePlayer1Vue1[9].SetActive(true);
                lifePlayer1Vue2[9].SetActive(true);
            }

            if(player2.GetHealth() >= 100) {
                lifePlayer2Vue1[9].SetActive(true);
                lifePlayer2Vue2[9].SetActive(true);
            }


            if (isPlaying)
            {
                MainGame();
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
                player1 = new PlayerStat(1);
                player2 = new PlayerStat(1);
                first = false;
            }
            if (SceneManager.GetActiveScene().name == "Game")
            {
                if (turn % 2 == 0)
                {
                    //Tour 2 P2
                    panelJoueur1.SetActive(false);
                    panelJoueur2.SetActive(true);
                    GameLoop(player2, player1);
                }
                else
                {
                    //Tour 1 P1
                    panelJoueur1.SetActive(true);
                    panelJoueur2.SetActive(false);
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

                        //}
                        //Jsais pas pourquoi �a marche mais �a marche du coup
                        //il faut remplacer �a par un fonction qui d�tecte une nouvelle imagetarget mais je sais pas laquelle
                        if (currentCardNumber != oldCardNumber)
                        {
                            oldCardNumber = currentCardNumber;
                            //TODO Ajouter l'image target detection, ajoute la carte d�tect�e dans la liste
                            //Remplacer new Mobs avec le monstre d�tect� lors du spawn
                            playerTurn.SetCardsOnField(new Mobs());

                            //Dans le cas ou Tajma spawn il soigne
                            //TODO Ici on echange le "c" avec l'objet qu'on r�cup�re lorsqu'on d�tecte le monstre
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

                        //TODO c'est juste un test de victory condition qui pourra �tre effac� plus tard
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
                                    foreach (var ca in playerTurn.GetCardsOnField())
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
                        //Jsais plus � quoi sert cette phase mais nsm on la garde
                        break;
                    }
            }
        }

        private IEnumerator DoAfter(float time)
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
                StartCoroutine(DoAfter(3));
            }
            if (p2.GetHealth() <= 0)
            {
                ResetUI();
                isPlaying = false;
                victoryP1.SetActive(true);
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

