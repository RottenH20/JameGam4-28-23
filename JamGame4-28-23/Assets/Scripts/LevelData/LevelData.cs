using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[CreateAssetMenu(fileName = "NewLevel", menuName = "HammerClimb/LevelData")]
public class LevelData : ScriptableObject {
    public string sceneName = "";
    public string displayName = "New Level";
    public float authorTime = 12f;
    public float goldTime = 16;
    public float silverTime = 30;
    public float bronzeTime = 60;
    public GameObject hammer;
    public string music = "LevelMusic1";
}