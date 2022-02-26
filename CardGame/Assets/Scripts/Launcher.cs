using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviour
    {
        bool isConnecting;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField] private GameObject controlPanel;
        //[Tooltip("The UI Label to inform the user that the connection is in progress")]
        //[SerializeField] private GameObject progressLabel;

        //[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        //[SerializeField] private byte maxPlayersPerRoom = 4;

        //private string gameVersion = "1";


        void Awake()
        {
            //// CRITICAL
            //// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            //PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            //progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public void Play()
        {
            SceneManager.LoadScene("Game");
        }
    }
}