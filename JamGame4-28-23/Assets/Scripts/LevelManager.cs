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

    private void Start() {
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        hud = FindObjectOfType<HudReferenceManager>();
        OnStartLevel();
        player = FindObjectOfType<HammerController>();
    }

    public void OnStartLevel() {
        startTime = Time.time;
        penalties = 0;
        finalTime = -1;
        Time.timeScale = 1;
    }

    public void CompleteLevel() {
        finalTime = Time.time - startTime + penalties;
        if(finalTime < RecordManager.instance.bestTimes[levelNumber] || RecordManager.instance.bestTimes[levelNumber] < 0) {
            RecordManager.instance.bestTimes[levelNumber] = finalTime;
            hud.timerText.text = "New best time: " + finalTime.ToString("0.00") + "   -   R to restart";
        } else {
            hud.timerText.text = "You: " + finalTime.ToString("0.00") + "   -   Best: " + RecordManager.instance.bestTimes[levelNumber].ToString("0.00") + "   -   R to restart";
        }
        Time.timeScale = 0;
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
