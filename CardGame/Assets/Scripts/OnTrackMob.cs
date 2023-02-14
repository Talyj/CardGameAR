using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class OnTrackMob : MonoBehaviour
{

    [SerializeField] private Transform arenaPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Tracked()
    {
        Debug.Log("tracks");
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveAttack()
    {
        if (arenaPos.position != null)
        {
            //float step = 0.1f;
            ////GameObject c = GameObject.FindGameObjectWithTag("dps");
            //GameObject c = GameObject.Find("Lusth").transf
            //while(c.transform.position != arenaPos.position)
            //{

            //c.transform.position = Vector3.Lerp(c.transform.position, arenaPos.position, step);
            //}
        }
        else
        {
            Application.Quit();
        }
        
    }
    public void ArenaPosed()
    {
        arenaPos = GameObject.FindGameObjectWithTag("Arena").transform;
        
    }
}
