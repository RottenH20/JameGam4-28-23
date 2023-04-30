using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicSkip : MonoBehaviour
{
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if((Time.time-startTime >= 30) || Input.anyKeyDown || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Quit") || Input.GetButtonDown("Restart")) {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
