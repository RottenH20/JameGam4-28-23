using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour {
    public bool fail = false;
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            if (fail) {
                FindObjectOfType<LevelManager>().RestartLevel();
            } else {
                FindObjectOfType<LevelManager>().CompleteLevel();
            }
        }
    }
}
