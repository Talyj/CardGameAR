using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject menuRegles;
    public GameObject menuPause;
    
    public GameObject menuCards;
    public Image card;
    public Sprite[] cardId;
    private int i = 0;

    public GameObject audioOn;
    public GameObject audioOff;

    public AudioSource menuAudio;
    public AudioSource buttonAudio;

    void Start() {

        if(audioOn) {
            audioOn.SetActive(true);
        }

        if(audioOff) {
            audioOff.SetActive(false);
        }

        if(menuRegles) {
            menuRegles.SetActive(false);
        }

        if(menuPause) {
            menuPause.SetActive(false);
        }

        if(menuCards) {
            menuCards.SetActive(false);
        }
    }

    public void SetAudioOn() {
        PlaySoundButton();
        menuAudio.volume = 0.2f;
        audioOn.SetActive(true);
        audioOff.SetActive(false);
    }

    public void SetAudioOff() {
        PlaySoundButton();
        menuAudio.volume = 0.0f;
        audioOn.SetActive(false);
        audioOff.SetActive(true);
    }

    public void DisplayRules() {
        PlaySoundButton();
        menuRegles.SetActive(true);
    }

    public void ExitRules() {
        PlaySoundButton();
        menuRegles.SetActive(false);
    }

    public void DisplayCards() {
        PlaySoundButton();
        menuCards.SetActive(true);
    }

    public void ExitCards() {
        PlaySoundButton();
        menuCards.SetActive(false);
    }

    public void DisplayPause() {
        PlaySoundButton();
        menuPause.SetActive(true);
    }

    public void ExitPause() {
        PlaySoundButton();
        menuPause.SetActive(false);
    }

    public void DisplayCardList(bool dir) {
        if(dir) i += 1; else i -= 1;
        if(i > cardId.Length-1) i = 0;
        if(i < 0) i = cardId.Length-1;
        Debug.Log(i);
        card.sprite = cardId[i];
    }

    public void LoadMenu() {
        PlaySoundButton();
        Application.LoadLevel("Launcher");
    }

    public void PlaySoundButton() {
        buttonAudio.Play(0);
    }
}
