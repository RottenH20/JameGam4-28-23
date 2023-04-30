using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour {
    public bool fail = false;
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            if (fail) {
                FindObjectOfType<MMFrontEnd>().reLoadCurrentScene();
            } else {
                FindObjectOfType<LevelManager>().CompleteLevel();
            }
        }
    }
}
