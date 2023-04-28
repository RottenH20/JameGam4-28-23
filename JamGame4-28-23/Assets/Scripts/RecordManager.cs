using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour {
    public static RecordManager instance;

    public float[] bestTimes;
    public float[] bronzeMedals;
    public float[] silverMedals;
    public float[] goldMedals;

    private void Awake() {
        if (instance == null) {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}