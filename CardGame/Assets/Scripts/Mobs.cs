using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Mobs : MonoBehaviour
{
    public float life;
    public float damage;
    public float shield;
    public string type;
    public bool hasShield;
    public Animator anim;
    public bool isUsed;
    public GameObject particles;
    public bool isPlayed;

    public void Start()
    {
        isUsed = false;
        isPlayed = false;
    }

    void Update() {
        //Pour les test d'animations, à retirer
        if(Input.GetKeyDown("s")) {
            OnSpawn();
        }
        if(Input.GetKeyDown("m")) {
            Die();
        }
        if(Input.GetKeyDown("p")) {
            GetDamage();
        }
        if(Input.GetKeyDown("a")) {
            Attack();
        }

        //sauf lui

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            DestroyEntity();
        }

        if(life <= 0)
        {
            life = 0;
            Die();
            DestroyEntity();
        }
    }

    void Awake() {
        this.gameObject.SetActive(false);
        OnSpawn();
    }

    public void OnSpawn() {
        particles.SetActive(true);
        Invoke("SpawnMe", 2.0f);
    }

    private void SpawnMe() {
        this.gameObject.SetActive(true);
        anim.SetBool("Spawn", true);
        Invoke("Reset", 0.2f);
    }

    private void Reset() {
        anim.SetBool("GetDamage", false);
        anim.SetBool("Attack", false);
        anim.SetBool("Spawn", false);
    }

    public void Heal() { // a changer en player, recuperer sa vie
        anim.SetBool("Attack", true);
        Invoke("Reset", 0.2f);
        //player.life += m;
    }

    public void Attack() {
        anim.SetBool("Attack", true);
        Invoke("Reset", 0.2f);
    }
 
    public void GetDamage(/*float m*/) {
        //if(hasShield == true) {
        //    //a check si ca bug ici
        //    while(m > 0 || shield > 0) {
        //        shield--;
        //        m--;
        //    }
        //    life -= m;
        //} else {
        //    life -= m; 
        //}
        anim.SetBool("GetDamage", true);
        Invoke("Reset", 0.2f);
    }

    public void Die() {
        anim.SetBool("Die", true);
    }

    private void DestroyEntity() {
        Destroy(this.gameObject);
    }

}
