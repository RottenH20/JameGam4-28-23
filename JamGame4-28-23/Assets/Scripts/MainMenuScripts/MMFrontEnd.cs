using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MMFrontEnd : MonoBehaviour
{
    public Animator transition;
    public GameObject LevelSelect, Options, Statistics;
    
    private bool LevelSelectOn, OptionsOn, StatisticsOn;
    private string tmp;

    // Sorry for the bad code MUG if you read this, there is most likely a "cleaner" way to write this, however I dont know how :(
    public void OptionsPressed()
    {
        Options.SetActive(true);
        LevelSelect.SetActive(false);
        Statistics.SetActive(false);

    }

    public void StatisticsPressed()
    {

        Options.SetActive(false);
        LevelSelect.SetActive(false);
        Statistics.SetActive(true);
    }

    public void LevelSelectPressed()
    {

        Options.SetActive(false);
        LevelSelect.SetActive(true);
        Statistics.SetActive(false);
    }
 
    public void reLoadCurrentScene()
    {
        StartCoroutine(AnimationLoad(SceneManager.GetActiveScene().name));
    }

    public void returnToMainMenu()
    {
        StartCoroutine(AnimationLoad("Main Menu"));
    }

    public void LevelChoicePressed()
    {
        tmp = EventSystem.current.currentSelectedGameObject.name; // Get the "text" from the level

        StartCoroutine(AnimationLoad(tmp));
    }

    IEnumerator AnimationLoad(string sceneName)
    {
        // Play animation
        transition.SetTrigger("Start");
        // Wait
        yield return new WaitForSeconds(1); // Change depending on transistion time
        if (sceneName == "MainMenu")
        {
            // Play Main Menu Music here
            //FindObjectOfType<AudioControl>().PlayMusic("MainMenu");
        }
        else
        {
            // Play Game Music here
            //FindObjectOfType<AudioControl>().PlayMusic("GameMusic");
        }
        //Load Scene
        SceneManager.LoadScene(sceneName);
    }
}
