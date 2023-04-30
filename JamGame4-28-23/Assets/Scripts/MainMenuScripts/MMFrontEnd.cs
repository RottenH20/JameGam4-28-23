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
    private string tmp;
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
                if (RecordManager.instance.bestTimes[i] == -1) // Level not beat, no medal
                {
                    // Do nothing
                    continue; // Go to next iteration
                } else {
                    var tempColor = childMedals[i - 1].color;
                    tempColor.a = 255;
                    childMedals[i - 1].color = tempColor; // Set the alpha back to max, we set it to 0 to hide ugly white box
                }


                if (RecordManager.instance.bestTimes[i] < RecordManager.instance.goldMedals[i]) // Gold level achieved
                {
                    childMedals[i - 1].sprite = goldMedal;
                } else if (RecordManager.instance.bestTimes[i] < RecordManager.instance.silverMedals[i]) // Silver level achieved
                  {
                    childMedals[i - 1].sprite = silverMedal;
                } else if (RecordManager.instance.bestTimes[i] < RecordManager.instance.bronzeMedals[i]) // Bronze level achieved
                  {
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
        PlayClickSound();
        StartCoroutine(AnimationLoad("Main Menu"));
    }

    public void LevelChoicePressed()
    {
        crashSound.Play();
        tmp = EventSystem.current.currentSelectedGameObject.name; // Get the "text" from the level

        StartCoroutine(AnimationLoad(tmp));
    }

    public void NextLevel()
    {
        MatchCollection matches = Regex.Matches(SceneManager.GetActiveScene().name, @"\d+");

        int num = 0;
        foreach (Match match in matches) // Pretty useless at only 1 int SHOULD ever return. But it ok :)
        {
            num = int.Parse(match.Value);
        }

        if (num == 9)
        {
            return;
        }
        num++;
        string newSceneName = "Level" + num;
        PlayClickSound();
        StartCoroutine(AnimationLoad(newSceneName));
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
        else if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            // Keep Looping
            if (int.Parse(sceneName.Substring(5)) == 5 && int.Parse(SceneManager.GetActiveScene().name.Substring(5)) == 4)
            {
                FindObjectOfType<AudioControl>().PlayMusic("LevelMusic2");
            }
        }
        else if (sceneName != "Main Menu")
        {
            // Keep Looping
            if (int.Parse(sceneName.Substring(5)) < 5)
            {
                FindObjectOfType<AudioControl>().PlayMusic("LevelMusic1");
            }
            else
            {
                FindObjectOfType<AudioControl>().PlayMusic("LevelMusic2");
            }
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
