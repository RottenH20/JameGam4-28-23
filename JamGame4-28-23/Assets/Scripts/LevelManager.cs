using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class LevelManager : MonoBehaviour {
    float startTime;
    float penalties = 0;
    float finalTime = -1;

    int levelNumber;

    HudReferenceManager hud;
    HammerController player;
    GameObject OldTimeTracker;

    // These need to be made public and set in inspector, they are started as inactive and can't be found
    public GameObject WinScreen;
    public TextMeshProUGUI times;

    private void Start() {
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        hud = FindObjectOfType<HudReferenceManager>();
        OnStartLevel();
        player = FindObjectOfType<HammerController>();
        OldTimeTracker = GameObject.Find("CurrentUITime");
    }

    public void OnStartLevel() {
        startTime = Time.time;
        penalties = 0;
        finalTime = -1;
        Time.timeScale = 1;
    }

    public void CompleteLevel() {
        finalTime = Time.time - startTime + penalties; // Computes currentTime

        OldTimeTracker.SetActive(false); // We set old time to false (got in the way of end screen)
        WinScreen.SetActive(true);
        player.gameObject.transform.Find("SlimeVisuals").gameObject.SetActive(false); // Hide old player
        player.gameObject.transform.Find("Square (1)").gameObject.SetActive(false); // Hide hammer

        if (finalTime < RecordManager.instance.bestTimes[levelNumber] || RecordManager.instance.bestTimes[levelNumber] < 0) {
            RecordManager.instance.bestTimes[levelNumber] = finalTime; // Set new best Time

            times.text = "Best Time: " + finalTime.ToString("0.00") + "\n"
                + "Current Time: " + finalTime.ToString("0.00");
            //hud.timerText.text = "New best time: " + finalTime.ToString("0.00") + "   -   R to restart";
        } else {
            times.text = "Best Time: " + RecordManager.instance.bestTimes[levelNumber].ToString("0.00") + "\n"
                + "Current Time: " + finalTime.ToString("0.00");
                //= "You: " + finalTime.ToString("0.00") + "   -   Best: " + RecordManager.instance.bestTimes[levelNumber].ToString("0.00") + "   -   R to restart";
        }
        //Time.timeScale = 0;
    }

    public void RestartLevel() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update() {
        if(finalTime == -1)
            hud.timerText.text = (Time.time - startTime + penalties).ToString("0.00");
        if (Input.GetButtonDown("Restart")) {
            RestartLevel();
        }
        if (Input.GetButtonDown("Quit")) {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        }
        if (Input.GetKeyDown(KeyCode.Equals)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void AddPenalty(float timePenalty = 1) {
        penalties += timePenalty;
        player.Damage();
    }

    //public string RoundTime(float t) {
    //    return Mathf.CeilToInt(
    //}
}
