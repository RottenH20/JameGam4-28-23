using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class MMFrontEnd : MonoBehaviour
{
    Animator transition;
    private string tmp;
    public bool useMedals = false;
    private GameObject MedalsParent;

    private void Start()
    {
        transition = GameObject.Find("CircleFade").GetComponent<Animator>();
        if (useMedals)
            MedalsParent = GameObject.Find("Medals");
    }

    // Sorry for the bad code MUG if you read this, there is most likely a "cleaner" way to write this, however I dont know how :(
 
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

    public void NextLevel()
    {
        MatchCollection matches = Regex.Matches(SceneManager.GetActiveScene().name, @"\d+");

        int num = 0;
        foreach (Match match in matches)
        {
            num = int.Parse(match.Value);
        }

        if (num == 9)
        {
            return; // No more levels
        }
        num++;
        string newSceneName = "Level" + num;
        StartCoroutine(AnimationLoad(newSceneName));
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
