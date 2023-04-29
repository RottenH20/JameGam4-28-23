using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSetter : MonoBehaviour {
    private void Start() {
        GetComponent<CinemachineVirtualCamera>().Follow = FindObjectOfType<HammerController>().transform;
    }
}
