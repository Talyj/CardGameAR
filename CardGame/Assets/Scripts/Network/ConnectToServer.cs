using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public InputField usernameInput;
    public TextMeshProUGUI buttonText;
    public Text warning;

    void Start() {
        buttonText.text = "LANCER";
    }

    public void OnClickConnect()
    {
        if(usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connexion...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        } else {
            warning.text = "Un pseudo est obligatoire pour débuter!";
            warning.color = Color.red;
            Invoke("ResetText", 3f);
        }
    }

    public override void OnConnectedToMaster() {
        SceneManager.LoadScene("Lobby");
    }

    public void ResetText() {
        warning.text = "Entre ton pseudo...";
        warning.color = new Color32(50, 50, 50, 128);
    }
}
