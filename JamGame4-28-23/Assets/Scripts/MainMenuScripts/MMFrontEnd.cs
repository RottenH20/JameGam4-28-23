using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class MMFrontEnd : MonoBehaviour
{
    Animator transition;
    private int tmp;
    public bool useMedals = false; // Set to true if its your main menu
    public Sprite goldMedal, silverMedal, bronzeMedal;
    private GameObject MedalsParent;
    public Transform levelSelect;
    public Transform playButton;
    public AudioSource crashSound, ClickSound;
    public AudioSource[] RestartSound = new AudioSource[3];

    // Pretty poorly written. Sorry :(
    private void Start()
    {
        transition = GameObject.Find("CircleFade").GetComponent<Animator>();
    }

    void PrepareMedals() {
        if (useMedals) {
            MedalsParent = GameObject.Find("Medals");

            // Setup the medals here

            Image[] childMedals = new Image[MedalsParent.transform.childCount];

            for (int i = 0; i <= MedalsParent.transform.childCount - 1; i++) {
                childMedals[i] = MedalsParent.transform.GetChild(i).gameObject.GetComponent<Image>();
            }

            for (int i = 1; i <= 9; i++) {
                RecordManager.Medal medal = RecordManager.instance.GetLevelMedal(i - 1);
                if (medal == RecordManager.Medal.None) // Level not beat, no medal
                {
                    // Do nothing
                    continue; // Go to next iteration
                } else {
                    var tempColor = childMedals[i - 1].color;
                    tempColor.a = 255;
                    childMedals[i - 1].color = tempColor; // Set the alpha back to max, we set it to 0 to hide ugly white box
                }


                if (medal == RecordManager.Medal.Gold) { // Gold level achieved
                    childMedals[i - 1].sprite = goldMedal;
                } else if (medal == RecordManager.Medal.Silver) { // Silver level achieved
                    childMedals[i - 1].sprite = silverMedal;
                } else if (medal == RecordManager.Medal.Bronze ){ // Bronze level achieved
                    childMedals[i - 1].sprite = bronzeMedal;
                }
            }
        }
    }
 
    public void reLoadCurrentScene()
    {
        PlayRestartSound();
        StartCoroutine(AnimationLoad(SceneManager.GetActiveScene().name));
    }

    public void returnToMainMenu()
    {
        RecordManager.instance.CurrentLevel = -1;
        PlayClickSound();
        StartCoroutine(AnimationLoad("Main Menu"));
    }

    public void LevelChoicePressed()
    {
        crashSound.Play();
        tmp =  int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(EventSystem.current.currentSelectedGameObject.name.Length-1)); // Get the "text" from the level
        RecordManager.instance.CurrentLevel = tmp - 1;
        Debug.Log(tmp);
        StartCoroutine(AnimationLoad(RecordManager.instance.GetCurrentLevelName()));
    }

    public void NextLevel()
    {
        RecordManager.instance.CurrentLevel ++;
        PlayClickSound();
        StartCoroutine(AnimationLoad(RecordManager.instance.GetCurrentLevelName()));
    }

    IEnumerator AnimationLoad(string sceneName)
    {
        // Play animation
        transition.SetTrigger("Start");
        // Wait
        if (SceneManager.GetActiveScene().name == "Main Menu")
            yield return new WaitForSeconds(2.5f);
        else
            yield return new WaitForSeconds(1f);
        if (SceneManager.GetActiveScene().name == "Intro")
        {
            // Keep looping
        }
        else if (sceneName == "Main Menu")
        {
            // Play Main Menu Music here
            FindObjectOfType<AudioControl>().PlayMusic("MainMenuMusic");
        }
        else
        {
            FindObjectOfType<AudioControl>().PlayMusic(RecordManager.instance.GetCurrentLevelMusic());
        }
        
        //Load Scene
        SceneManager.LoadScene(sceneName);
    }

    public void OpenLevelSelect() {
        playButton.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(true);
        PlayClickSound();
        PrepareMedals();
    }

    public void PlayClickSound()
    {
        ClickSound.Play();
    }

    public void PlayRestartSound()
    {
        System.Random rnd = new System.Random();
        int num = rnd.Next(0, 3);

        RestartSound[num].Play();
    }
}
