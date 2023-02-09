using Com.MyCompany.MyGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBis : MonoBehaviour
{
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
