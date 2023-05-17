using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class SerializableDictionary {
    public List<string> keys = new List<string>();
    public List<float> values = new List<float>();

    public SerializableDictionary(Dictionary<string,float> dict) {
        keys.AddRange(dict.Keys);
        values.AddRange(dict.Values);
    }
}

public class RecordManager : MonoBehaviour {
    public enum Medal { None = 0, Bronze = 1, Silver = 2, Gold = 3, Author = 4}
    public static RecordManager instance;
    public Campaign currentCampaign;
    public int CurrentLevel { get; set; } = -1;

    //public float[] bestTimes;
    //public float[] bronzeMedals;
    //public float[] silverMedals;
    //public float[] goldMedals;

   
    public Dictionary<string, float> times = new Dictionary<string, float>();
    static string timesPath = "times";

    private void Awake() {
        if (instance == null) {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            LoadTimes();
            if (CurrentLevel == -1) {
                FindCurrentLevel();
            }
        } else {
            Destroy(gameObject);
        }
    }

    void FindCurrentLevel() {
        for(int i = 0; i < currentCampaign.levels.Length; i++) {
            if(SceneManager.GetActiveScene().name == currentCampaign.levels[i].sceneName) {
                Debug.Log("found level " + i.ToString(), this);
                CurrentLevel = i;
                return;
            }
        }
        Debug.Log("No Level found", this);
    }

    public string GetTimesPath() {
        return Application.persistentDataPath + "/" + timesPath + ".json";
    }

    public void SaveTimes() {
        try {
            //Debug.Log("saving " + GetTimesPath());
            //foreach(KeyValuePair<string,float> pair in times) {
            //    Debug.Log(pair.Key + pair.Value.ToString());
            //}
            string jsonData = JsonUtility.ToJson(new SerializableDictionary(times));
            //Debug.Log(jsonData);
            File.WriteAllText(GetTimesPath(), jsonData);
        } catch (System.Exception ex) {
            Debug.LogError("Failed to save times: " + ex.Message);
        }
    }

    public void LoadTimes() {
        try {
            //Debug.Log("loading " + GetTimesPath()); 
            string jsonData = File.ReadAllText(GetTimesPath());
            //Debug.Log(jsonData);
            SerializableDictionary tempData = JsonUtility.FromJson<SerializableDictionary>(jsonData);
            times.Clear();
            for (int i = 0; i < tempData.keys.Count; i++) {
                string key = tempData.keys[i];
                float value = tempData.values[i];
                times[key] = value;
            }
            //foreach (KeyValuePair<string, float> pair in times) {
            //    Debug.Log(pair.Key + pair.Value.ToString());
            //}
        } catch (FileNotFoundException ex) {
            Debug.LogWarning("Failed to load times: " + ex.Message);
        } catch (System.Exception ex) {
            Debug.LogError("Failed to load times: " + ex.Message);
        }
    }

    public void SetNewTime(float time) {
        if (CurrentLevel < 0) {
            Debug.LogError("Tried to save time on invalid level", this);
        } else {
            times[currentCampaign.levels[CurrentLevel].sceneName] = time;
            SaveTimes();
        }
    }

    public float GetLevelTime(int levelNumber) {
        if(levelNumber > -1 && times.TryGetValue(currentCampaign.levels[levelNumber].sceneName,out float time)) {
            return time;
        }
        return -1;
    }

    public Medal GetLevelMedal(int levelNumber) {
        float time = GetLevelTime(levelNumber);
        if (time < 0)
            return Medal.None;
        //Author time not implemented yet
        //if (time <= currentCampaign.levels[levelNumber].authorTime)
        //    return Medal.Author;
        if (time <= currentCampaign.levels[levelNumber].goldTime)
            return Medal.Gold;
        if (time <= currentCampaign.levels[levelNumber].silverTime)
            return Medal.Silver;
        if (time <= currentCampaign.levels[levelNumber].bronzeTime)
            return Medal.Bronze;
        return Medal.None;
    }

    public string GetLevelName(int levelNumber) {
        return currentCampaign.levels[levelNumber].sceneName;
    }

    public string GetCurrentLevelName() {
        return currentCampaign.levels[CurrentLevel].sceneName;
    }

    public string GetCurrentLevelMusic() {
        if (CurrentLevel == -1)
            return "LevelMusic1";
        return currentCampaign.levels[CurrentLevel].music;
    }

    public LevelData GetCurrentLevel() {
        return currentCampaign.levels[CurrentLevel];
    }
}