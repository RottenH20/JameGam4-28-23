using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour {
    float startTime;
    float penalties = 0;
    float finalTime = -1;

    int levelNumber;

    MMFrontEnd levelLoader;
    HudReferenceManager hud;
    HammerController player;
    GameObject OldTimeTracker;

    public Sprite goldMedal, silverMedal, bronzeMedal; // Set what each sprite is. Used becuase we are missing Gold/Silver/Bronze medal sprites

    // These need to be made public and set in inspector, they are started as inactive and can't be found
    public GameObject WinScreen;
    public TextMeshProUGUI times;
    public Image Medal;
    public TextMeshProUGUI timeNeeded;
    bool levelComplete = false;

    private void Start() {
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        hud = FindObjectOfType<HudReferenceManager>();
        OnStartLevel();
        player = FindObjectOfType<HammerController>();
        OldTimeTracker = GameObject.Find("CurrentUITime");
        levelLoader = FindObjectOfType<MMFrontEnd>();
    }

    public void OnStartLevel() {
        startTime = Time.time;
        penalties = 0;
        finalTime = -1;
        Time.timeScale = 1;
    }

    public void CompleteLevel() {
        levelComplete = true;
        finalTime = Time.time - startTime + penalties; // Computes currentTime

        OldTimeTracker.SetActive(false); // We set old time to false (got in the way of end screen)
        WinScreen.SetActive(true);
        player.gameObject.SetActive(false);
        float bestTime = RecordManager.instance.GetLevelTime(RecordManager.instance.CurrentLevel);
        if (finalTime < bestTime || bestTime < 0) {
            RecordManager.instance.SetNewTime(finalTime); // Set new best Time

            times.text = "Best Time: " + finalTime.ToString("0.00") + "\n"
                + "Current Time: " + finalTime.ToString("0.00");
        } else {
            times.text = "Best Time: " + bestTime.ToString("0.00") + "\n"
                + "Current Time: " + finalTime.ToString("0.00");
        }

        timeNeeded.text = "Bronze: " + RecordManager.instance.GetCurrentLevel().bronzeTime + ", " +
            "Silver: " + RecordManager.instance.GetCurrentLevel().silverTime + ", " +
            "Gold: " + RecordManager.instance.GetCurrentLevel().goldTime;
        RecordManager.Medal medal = RecordManager.instance.GetLevelMedal(RecordManager.instance.CurrentLevel);
        if (medal == RecordManager.Medal.Gold) // Gold level achieved
        {
            Medal.enabled = true;
            Medal.sprite = goldMedal;
        }
        else if (medal == RecordManager.Medal.Silver) // Silver level achieved
        {
            Medal.enabled = true;
            Medal.sprite = silverMedal;
        }
        else if (medal == RecordManager.Medal.Bronze) // Bronze level achieved
        {
            Medal.enabled = true;
            Medal.sprite = bronzeMedal;
        } else {
            Medal.enabled = false;
        }
        //Time.timeScale = 0;
    }

    private void Update() {
        if(finalTime == -1)
            hud.timerText.text = (Time.time - startTime + penalties).ToString("0.00");
        if (Input.GetButtonDown("Restart")) {
            levelLoader.reLoadCurrentScene();
        }
        if (Input.GetButtonDown("Quit")) {
            Time.timeScale = 1;
            levelLoader.returnToMainMenu();
        }
        if (Input.GetButtonDown("Fire1") && levelComplete) {
            levelLoader.NextLevel();
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
