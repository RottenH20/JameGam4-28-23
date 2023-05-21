using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using static UnityEngine.Rendering.PostProcessing.PostProcessLayer;

public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        // Set Static Object
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        FindSecret.Clear();
        for (int i = 0; i < SecretRoom.Count; i++)
        {
            FindSecret.Add(false);
        }
        FindItems.Clear();
        for (int i = 0; i < Items.Count; i++)
        {
            FindItems.Add(false);
        }
    }

    public CampaignManager campaign;
    [Line("Trophy Settings")]
    public GameModeManager Gamemode;

    [Line("Gamemode Settings")]
    [HelpBox("These settings are used in \"Deadly Race\" and \"Wave Mode\"\nThey are not used in Arcade Mode and Classic", HelpBoxEnum.Info)]
    public float EnemySpawnRadius = 25;

    [ShowIf("Gamemode.GameMode", 1)]
    public float EnemySpawnRate = 4.2f;
    [ShowIf("Gamemode.GameMode", 1)]
    public Vector2 EnemySpawnSize = new Vector2(4, 6);
    [ShowIf("Gamemode.GameMode", 1)]
    public float DeadlyRaceMultiplier = 1.09f;

    [ShowIf("Gamemode.GameMode", 2)]
    public float MaxWaveTime = 30f;
    [ShowIf("Gamemode.GameMode", 2)]
    public int WaveSpawnSize = 8;
    [ShowIf("Gamemode.GameMode", 2)]
    public float WaveSpawnMultiplier = 1.15f;
    public List<GameObject> EnemyPrefabs;
    [Space()]
    public float ExplosionObjectCount = 25;
    public List<GameObject> ExplosionObjectContainer;



    [Line("Key Doors")]
    public bool redKeys;
    public bool blueKeys;
    public bool yellowKeys;
    public bool greenKeys;


    //[Line("To Dev")]
    [HideInInspector] public List<SecretScript> SecretRoom;
    [HideInInspector] public List<BonusScript> Items;
    [HideInInspector] public List<Enemy> Enemy;
    [HideInInspector] public int WaveLevel;
    [HideInInspector] public List<bool> FindItems;
    [HideInInspector] public List<bool> FindSecret;
    [HideInInspector] public List<bool> FindEnemy;
    [HideInInspector] public GameObject EndPanel, NextLevelBtn;
    [HideInInspector] public TMP_Text TimeTxt, ItemsTxt, SecretsTxt, EnemyTxt;
    [HideInInspector] public Transform Requirement_1, Requirement_2, Requirement_3, Requirement_4, Requirement_5;
    [HideInInspector] public TMPTextStringLocalization NameTxt, TrophyTxt, TrophyTxt2;

    float timer;
    float gamemodeTimer = 999;
    int _DealtyRaceCounter;

    int foundSecret;
    int foundItems;
    int foundEnemy;

    bool EndLevel;

    private void Start()
    {

        EndPanel = GameObject.Find("Player/Main Panels/EndScreen");
        NextLevelBtn = GameObject.Find("Player/Main Panels/EndScreen/Next_Level_Btn");
        NameTxt = GameObject.Find("Player/Main Panels/EndScreen/Level Name Txt").GetComponent<TMPTextStringLocalization>();

        TrophyTxt = GameObject.Find("Player/Main Panels/EndScreen/Base_Info/Trophy/Value_Txt").GetComponent<TMPTextStringLocalization>();
        TimeTxt = GameObject.Find("Player/Main Panels/EndScreen/Base_Info/Time/Value_Txt").GetComponent<TMP_Text>();
        ItemsTxt = GameObject.Find("Player/Main Panels/EndScreen/Base_Info/Item/Value_Txt").GetComponent<TMP_Text>();
        SecretsTxt = GameObject.Find("Player/Main Panels/EndScreen/Base_Info/Secret/Value_Txt").GetComponent<TMP_Text>();
        EnemyTxt = GameObject.Find("Player/Main Panels/EndScreen/Base_Info/Enemy/Value_Txt").GetComponent<TMP_Text>();


        TrophyTxt2 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Trophy/Value_Txt").GetComponent<TMPTextStringLocalization>();
        Requirement_1 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Requirement_1").GetComponent<Transform>();
        Requirement_2 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Requirement_2").GetComponent<Transform>();
        Requirement_3 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Requirement_3").GetComponent<Transform>();
        Requirement_4 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Requirement_4").GetComponent<Transform>();
        Requirement_5 = GameObject.Find("Player/Main Panels/EndScreen/Trophy_Info/Requirement_5").GetComponent<Transform>();

        EndPanel.SetActive(false);

        Transform _special_ui = GameObject.Find("Player/Canvas/UI/Gamemodes_UI").GetComponent<Transform>();
        foreach (Transform child in _special_ui) { child.gameObject.SetActive(false); }

        if (Gamemode.GameMode == GameModes.Classic) { /* Do Nothing */ }

        if (Gamemode.GameMode == GameModes.DeadlyRace)
        {
            _special_ui.Find("Deadly_Race_UI").GetComponent<Transform>().gameObject.SetActive(true);
            for (int i = 0; i < ExplosionObjectCount; i++)
            {
                Instantiate(ExplosionObjectContainer[UnityEngine.Random.Range(0, (ExplosionObjectContainer.Count))], GetRandomPoint(), Quaternion.identity);
            }
        }

        if (Gamemode.GameMode == GameModes.WaveMode)
        {
            _special_ui.Find("Wave_UI").GetComponent<Transform>().gameObject.SetActive(true);
            for (int i = 0; i < ExplosionObjectCount; i++)
            {
                Instantiate(ExplosionObjectContainer[UnityEngine.Random.Range(0, (ExplosionObjectContainer.Count))], GetRandomPoint(), Quaternion.identity);
            }
        }

        if (Gamemode.GameMode == GameModes.ArcadeMode)
        {
            _special_ui.Find("Arcade_UI").GetComponent<Transform>().gameObject.SetActive(true);
        }

        #region Get Settings

        // Anti-Aliassing
        PostProcessLayer _antiAliasing = (PostProcessLayer)GameObject.FindObjectOfType(typeof(PostProcessLayer));
        _antiAliasing.antialiasingMode = (PostProcessLayer.Antialiasing)TinySaveSystem.GetInt("settings_antiAliasing");
        // Post Processing
        PostProcessVolume _pp = (PostProcessVolume)GameObject.FindObjectOfType(typeof(PostProcessVolume));
        _pp.weight = (TinySaveSystem.GetBool("settings_postProcessing") == true ? 1 : 0);

        #endregion
    }

    public void TryFindSecret(SecretScript secretObject)
    {
        for (int i = 0; i < SecretRoom.Count; i++)
        {
            if (SecretRoom[i].secret_id == secretObject.secret_id) { FindSecret[i] = true; foundSecret++; }
        }
    }

    public void TryFindItem(BonusScript itemObject)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].secret_id == itemObject.secret_id) { FindItems[i] = true; foundItems++; }
        }
    }

    public void TryFindEnemy(Enemy enemyObject)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Enemy[i].secret_id == enemyObject.secret_id) { FindEnemy[i] = true; foundEnemy++; }
        }
    }

    private void Update()
    {
        if (!EndLevel) { timer += Time.deltaTime; gamemodeTimer += Time.deltaTime; }

        if (Gamemode.GameMode == GameModes.Classic) { /* Do Nothing */ }

        if (Gamemode.GameMode == GameModes.DeadlyRace)
        {
            GameObject.Find("Player/Canvas/UI/Gamemodes_UI/Deadly_Race_UI/Value_Txt").GetComponent<TMP_Text>().text = Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00") + ":" + ((timer * 100f) % 100).ToString("00");

            if (gamemodeTimer >= EnemySpawnRate)
            {
                _DealtyRaceCounter++;
                for (int i = 0; i < UnityEngine.Random.Range(((int)EnemySpawnSize.y + (int)(EnemySpawnSize.y * DeadlyRaceMultiplier * _DealtyRaceCounter)), ((int)EnemySpawnSize.x + (int)(EnemySpawnSize.x * DeadlyRaceMultiplier * _DealtyRaceCounter))); i++)
                {
                    GameObject rocketInstantiated = (GameObject)Instantiate(EnemyPrefabs[UnityEngine.Random.Range(0, (EnemyPrefabs.Count))], RandomNavmeshLocation(EnemySpawnRadius), Quaternion.identity);
                    rocketInstantiated.GetComponent<EnemyStates>().OrderWaypoints = TargetSelectType.PlayerTarget;
                }

                gamemodeTimer = 0;
            }
        }

        if (Gamemode.GameMode == GameModes.WaveMode)
        {
            TMPTextStringLocalization _tmp = GameObject.Find("Player/Canvas/UI/Gamemodes_UI/Wave_UI/Value_Txt").GetComponent<TMPTextStringLocalization>();
            _tmp.text_tmp.text = _tmp.ShowText.localization.GetString("wave") + ": " + WaveLevel;

            if (gamemodeTimer >= MaxWaveTime)
            {
                WaveLevel++;
                for (int i = 0; i < (WaveSpawnSize + (int)(WaveSpawnSize * WaveSpawnMultiplier * WaveLevel)); i++)
                {
                    GameObject rocketInstantiated = (GameObject)Instantiate(EnemyPrefabs[UnityEngine.Random.Range(0, (EnemyPrefabs.Count))], RandomNavmeshLocation(EnemySpawnRadius), Quaternion.identity);
                    rocketInstantiated.GetComponent<EnemyStates>().OrderWaypoints = TargetSelectType.PlayerTarget;
                }

                gamemodeTimer = 0;
            }
        }

        if (Gamemode.GameMode == GameModes.ArcadeMode)
        {
            TMPTextStringLocalization _tmp = GameObject.Find("Player/Canvas/UI/Gamemodes_UI/Arcade_UI/Value_Txt").GetComponent<TMPTextStringLocalization>();
            _tmp.text_tmp.text = _tmp.ShowText.localization.GetString("score") + ": " + GameManager.Instance.TotalPoints;
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += GameManager.Instance.transform.parent.position;

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;


        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public static Vector3 GetRandomPoint()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 666;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, 666, NavMesh.AllAreas);


        return hit.position;
    }


    #region Trophy in Script

    public void GenerateEndStats()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Trophy _tmpTrophy = TinySaveSystem.GetTrophy(sceneName);

        // Set Time and Cursor
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Disable Player
        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerHealth>().enabled = false;

        // Enable Panel and Disable Timer
        EndLevel = true;
        EndPanel.SetActive(true);
        NameTxt.ShowText.key = campaign.FindKeyFromScene();

        _tmpTrophy.Requirement_1 = CheckTrophyRequirement(Requirement_1, Gamemode.GetRequirement(1));
        _tmpTrophy.Requirement_2 = CheckTrophyRequirement(Requirement_2, Gamemode.GetRequirement(2));
        _tmpTrophy.Requirement_3 = CheckTrophyRequirement(Requirement_3, Gamemode.GetRequirement(3));
        _tmpTrophy.Requirement_4 = CheckTrophyRequirement(Requirement_4, Gamemode.GetRequirement(4));
        _tmpTrophy.Requirement_5 = CheckTrophyRequirement(Requirement_5, Gamemode.GetRequirement(5));

        _tmpTrophy.Dev_Check_Requirements();



        TimeTxt.text = Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00");


        if (Items.Count == 0) { ItemsTxt.text = "100%"; }
        else { ItemsTxt.text = Mathf.Floor((float)foundItems / Items.Count * 100) + "%"; }

        if (SecretRoom.Count == 0) { SecretsTxt.text = "100%"; }
        else { SecretsTxt.text = Mathf.Floor((float)foundSecret / SecretRoom.Count * 100) + "%"; }

        if (Enemy.Count == 0) { EnemyTxt.text = "100%"; }
        else { EnemyTxt.text = Mathf.Floor((float)foundEnemy / Enemy.Count * 100) + "%"; }



        // Generate Trophy
        GenerateTrophy(_tmpTrophy);
        // If Better Save
        if (_tmpTrophy.GetCount() > TinySaveSystem.GetTrophy(sceneName).GetCount()) { TinySaveSystem.SetTrophy(sceneName, _tmpTrophy); }

        // Add Points
        GameSettingsManger _gm = Resources.Load("Game_Settings") as GameSettingsManger;
        switch (_tmpTrophy.GetCount())
        {
            case 1:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.NoneTrophyPoints);
                break;
            case 2:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.BronzeTrophyPoints);
                break;
            case 3:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.SilverTrophyPoints);
                break;
            case 4:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.GoldTrophyPoints);
                break;
            case 5:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.DiamondTrophyPoints);
                break;
            default:
                TinySaveSystem.SetInt("unlockable_point", TinySaveSystem.GetInt("unlockable_point") + _gm.PlatiniumTrophyPoints);
                break;
        }
        this.Log($"You get {_tmpTrophy.GetCount()} point(s)");

        // Next Level Btn
        string checkerNextLevel = campaign.CheckNextLevel();
        if (checkerNextLevel != "Error") { NextLevelBtn.SetActive(true); NextLevelBtn.GetComponent<Button>().onClick.AddListener(delegate () { LoadingScreen.instance.loadingScreen(checkerNextLevel); }); }
        else { NextLevelBtn.SetActive(false); }

        // Disable & Enable Objects
        foreach (Transform child in player.transform)
        {
            if (child.tag != "MainCamera") child.gameObject.SetActive(false);
            if (child.name == "Main Panels") child.gameObject.SetActive(true);
        }
    }

    void GenerateTrophy(Trophy trophy)
    {
        switch (trophy.GetCount())
        {
            case 1:
                TrophyTxt.ShowText.key = "bronze";
                TrophyTxt2.ShowText.key = "bronze";
                break;
            case 2:
                TrophyTxt.ShowText.key = "silver";
                TrophyTxt2.ShowText.key = "silver";
                break;
            case 3:
                TrophyTxt.ShowText.key = "gold";
                TrophyTxt2.ShowText.key = "gold";
                break;
            case 4:
                TrophyTxt.ShowText.key = "platinium";
                TrophyTxt2.ShowText.key = "platinium";
                break;
            case 5:
                TrophyTxt.ShowText.key = "diamond";
                TrophyTxt2.ShowText.key = "diamond";
                break;
            default:
                TrophyTxt.ShowText.key = "none";
                TrophyTxt2.ShowText.key = "none";
                break;
        }
    }

    bool CheckTrophyRequirement(Transform _element, TrophyRequirementClass _requirement)
    {
        switch (_requirement.RequirementType)
        {
            case TrophyRequirement.CompleteLevel:
                _element.gameObject.SetActive(false);
                return true;

            case TrophyRequirement.ItemsPercent:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "items";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor((float)foundItems / Items.Count * 100)}% / {_requirement.RequirementValue}%";
                if (Items.Count == 0) { _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"100% / {_requirement.RequirementValue}%"; return true; }
                if (Mathf.Floor((float)foundItems / Items.Count * 100) >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.Items:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "items";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{foundItems} / {_requirement.RequirementValue}";
                if (foundItems >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.EnemyPercent:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "enemy";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor((float)foundEnemy / Enemy.Count * 100)}% / {_requirement.RequirementValue}%";
                if (Enemy.Count == 0) { _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"100% / {_requirement.RequirementValue}%"; return true; }
                if (Mathf.Floor((float)foundEnemy / Enemy.Count * 100) >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.Enemy:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "enemy";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{foundEnemy}% / {_requirement.RequirementValue}";
                if (foundEnemy >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.SecretsPercent:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "secrets";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor((float)foundSecret / SecretRoom.Count * 100)}% / {_requirement.RequirementValue}%";
                if (SecretRoom.Count == 0) { _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"100% / {_requirement.RequirementValue}%"; return true; }
                if (Mathf.Floor((float)foundSecret / SecretRoom.Count * 100) >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.Secrets:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "secrets";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{foundSecret}% / {_requirement.RequirementValue}";
                if (foundSecret >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.MinimumScore:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "score";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{GameManager.Instance.TotalPoints} / {_requirement.RequirementValue}";
                if (GameManager.Instance.TotalPoints >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.MinimumWave:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "wave";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{WaveLevel} / {_requirement.RequirementValue}";
                if (WaveLevel >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case TrophyRequirement.MoreTimeThan:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "survivedTime";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00")} > {Mathf.Floor(_requirement.RequirementValue / 60).ToString("00") + ":" + (_requirement.RequirementValue % 60).ToString("00")}";
                if (timer >= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00")} > {Mathf.Floor(_requirement.RequirementValue / 60).ToString("00") + ":" + (_requirement.RequirementValue % 60).ToString("00")}";
                    return false;
                }

            case TrophyRequirement.LessTimeThan:
                _element.gameObject.SetActive(true);
                _element.Find("Name_Txt").GetComponent<TMPTextStringLocalization>().ShowText.key = "time";
                _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"{Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00")} < {Mathf.Floor(_requirement.RequirementValue / 60).ToString("00") + ":" + (_requirement.RequirementValue % 60).ToString("00")}";
                if (timer <= _requirement.RequirementValue)
                {
                    return true;
                }
                else
                {
                    _element.Find("Value_Txt").GetComponent<TMP_Text>().text = $"<s>{Mathf.Floor(timer / 60).ToString("00") + ":" + (timer % 60).ToString("00")} < {Mathf.Floor(_requirement.RequirementValue / 60).ToString("00") + ":" + (_requirement.RequirementValue % 60).ToString("00")}";
                    return false;
                }

            default:
                _element.gameObject.SetActive(false);
                return false;
        }
    }

    #endregion


}


#region Trophy

[System.Serializable]
public class Trophy
{
    public bool Requirement_1, Requirement_2, Requirement_3, Requirement_4, Requirement_5;

    public int GetCount()
    {
        int _tmp = 0;

        if (Requirement_1) _tmp++;
        if (Requirement_2) _tmp++;
        if (Requirement_3) _tmp++;
        if (Requirement_4) _tmp++;
        if (Requirement_5) _tmp++;

        return _tmp;
    }

    public void Dev_Check_Requirements()
    {
        Debug.Log($"{Requirement_1}, {Requirement_2}, {Requirement_3}, {Requirement_4}, {Requirement_5}");
    }
}

[System.Serializable]
public enum GameModes
{
    Classic = 0,
    DeadlyRace = 1,
    WaveMode = 2,
    ArcadeMode = 3
}

[System.Serializable]
public class GameModeManager
{
    public GameModes GameMode;
    // Trophy Classic
    public TrophyRequirementClass ClassicTrophy1 = new TrophyRequirementClass(TrophyRequirement.CompleteLevel, 0);
    public TrophyRequirementClass ClassicTrophy2 = new TrophyRequirementClass(TrophyRequirement.EnemyPercent, 100);
    public TrophyRequirementClass ClassicTrophy3 = new TrophyRequirementClass(TrophyRequirement.ItemsPercent, 100);
    public TrophyRequirementClass ClassicTrophy4 = new TrophyRequirementClass(TrophyRequirement.SecretsPercent, 100);
    public TrophyRequirementClass ClassicTrophy5 = new TrophyRequirementClass(TrophyRequirement.LessTimeThan, 360);
    // Trophy Deadly Race
    public TrophyRequirementClass DeadlyRaceTrophy1 = new TrophyRequirementClass(TrophyRequirement.MoreTimeThan, 120);
    public TrophyRequirementClass DeadlyRaceTrophy2 = new TrophyRequirementClass(TrophyRequirement.MoreTimeThan, 180);
    public TrophyRequirementClass DeadlyRaceTrophy3 = new TrophyRequirementClass(TrophyRequirement.MoreTimeThan, 300);
    public TrophyRequirementClass DeadlyRaceTrophy4 = new TrophyRequirementClass(TrophyRequirement.MoreTimeThan, 600);
    public TrophyRequirementClass DeadlyRaceTrophy5 = new TrophyRequirementClass(TrophyRequirement.MoreTimeThan, 1200);
    // Trophy Wave Mode
    public TrophyRequirementClass WaveTrophy1 = new TrophyRequirementClass(TrophyRequirement.MinimumWave, 5);
    public TrophyRequirementClass WaveTrophy2 = new TrophyRequirementClass(TrophyRequirement.MinimumWave, 10);
    public TrophyRequirementClass WaveTrophy3 = new TrophyRequirementClass(TrophyRequirement.MinimumWave, 20);
    public TrophyRequirementClass WaveTrophy4 = new TrophyRequirementClass(TrophyRequirement.MinimumWave, 35);
    public TrophyRequirementClass WaveTrophy5 = new TrophyRequirementClass(TrophyRequirement.MinimumWave, 50);
    // Trophy Arcade Mode
    public TrophyRequirementClass ArcadeModeTrophy1 = new TrophyRequirementClass(TrophyRequirement.MinimumScore, 5000);
    public TrophyRequirementClass ArcadeModeTrophy2 = new TrophyRequirementClass(TrophyRequirement.MinimumScore, 1000);
    public TrophyRequirementClass ArcadeModeTrophy3 = new TrophyRequirementClass(TrophyRequirement.MinimumScore, 5000);
    public TrophyRequirementClass ArcadeModeTrophy4 = new TrophyRequirementClass(TrophyRequirement.MinimumScore, 10000);
    public TrophyRequirementClass ArcadeModeTrophy5 = new TrophyRequirementClass(TrophyRequirement.MinimumScore, 25000);

    public TrophyRequirementClass GetRequirement(int index)
    {
        switch (GameMode)
        {
            case GameModes.DeadlyRace:
                if (index == 1) return DeadlyRaceTrophy1;
                else if (index == 2) return DeadlyRaceTrophy2;
                else if (index == 3) return DeadlyRaceTrophy3;
                else if (index == 4) return DeadlyRaceTrophy4;
                else return DeadlyRaceTrophy5;

            case GameModes.WaveMode:
                if (index == 1) return WaveTrophy1;
                else if (index == 2) return WaveTrophy2;
                else if (index == 3) return WaveTrophy3;
                else if (index == 4) return WaveTrophy4;
                else return WaveTrophy5;

            case GameModes.ArcadeMode:
                if (index == 1) return ArcadeModeTrophy1;
                else if (index == 2) return ArcadeModeTrophy2;
                else if (index == 3) return ArcadeModeTrophy3;
                else if (index == 4) return ArcadeModeTrophy4;
                else return ArcadeModeTrophy5;

            default:
                if (index == 1) return ClassicTrophy1;
                else if (index == 2) return ClassicTrophy2;
                else if (index == 3) return ClassicTrophy3;
                else if (index == 4) return ClassicTrophy4;
                else return ClassicTrophy5;
        }
    }

    public string GetTrophyName(int index)
    {
        switch (index)
        {
            case 1:
                return "bronze";
            case 2:
                return "silver";
            case 3:
                return "gold";
            case 4:
                return "platinium";
            case 5:
                return "diamond";
            default:
                return "none";
        }
    }
}

[System.Serializable]
public class TrophyRequirementClass
{
    public TrophyRequirementClass(TrophyRequirement type, float value)
    {
        RequirementType = type;
        RequirementValue = value;
    }

    public TrophyRequirement RequirementType;
    public float RequirementValue;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(GameModeManager))]
public class GameModeManagerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18), property.FindPropertyRelative("GameMode"), new GUIContent("Gamemode Type"));

        EditorGUI.indentLevel = 1;

        if (property.FindPropertyRelative("GameMode").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + 20, position.width, 18), property.FindPropertyRelative("ClassicTrophy1"), new GUIContent("Requirement Nr. 1"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 40, position.width, 18), property.FindPropertyRelative("ClassicTrophy2"), new GUIContent("Requirement Nr. 2"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 60, position.width, 18), property.FindPropertyRelative("ClassicTrophy3"), new GUIContent("Requirement Nr. 3"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 80, position.width, 18), property.FindPropertyRelative("ClassicTrophy4"), new GUIContent("Requirement Nr. 4"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 100, position.width, 18), property.FindPropertyRelative("ClassicTrophy5"), new GUIContent("Requirement Nr. 5"));
        }
        else if (property.FindPropertyRelative("GameMode").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + 20, position.width, 18), property.FindPropertyRelative("DeadlyRaceTrophy1"), new GUIContent("Requirement Nr. 1"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 40, position.width, 18), property.FindPropertyRelative("DeadlyRaceTrophy2"), new GUIContent("Requirement Nr. 2"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 60, position.width, 18), property.FindPropertyRelative("DeadlyRaceTrophy3"), new GUIContent("Requirement Nr. 3"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 80, position.width, 18), property.FindPropertyRelative("DeadlyRaceTrophy4"), new GUIContent("Requirement Nr. 4"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 100, position.width, 18), property.FindPropertyRelative("DeadlyRaceTrophy5"), new GUIContent("Requirement Nr. 5"));
        }
        else if (property.FindPropertyRelative("GameMode").enumValueIndex == 2)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + 20, position.width, 18), property.FindPropertyRelative("WaveTrophy1"), new GUIContent("Requirement Nr. 1"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 40, position.width, 18), property.FindPropertyRelative("WaveTrophy2"), new GUIContent("Requirement Nr. 2"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 60, position.width, 18), property.FindPropertyRelative("WaveTrophy3"), new GUIContent("Requirement Nr. 3"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 80, position.width, 18), property.FindPropertyRelative("WaveTrophy4"), new GUIContent("Requirement Nr. 4"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 100, position.width, 18), property.FindPropertyRelative("WaveTrophy5"), new GUIContent("Requirement Nr. 5"));
        }
        else if (property.FindPropertyRelative("GameMode").enumValueIndex == 3)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + 20, position.width, 18), property.FindPropertyRelative("ArcadeModeTrophy1"), new GUIContent("Requirement Nr. 1"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 40, position.width, 18), property.FindPropertyRelative("ArcadeModeTrophy2"), new GUIContent("Requirement Nr. 2"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 60, position.width, 18), property.FindPropertyRelative("ArcadeModeTrophy3"), new GUIContent("Requirement Nr. 3"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 80, position.width, 18), property.FindPropertyRelative("ArcadeModeTrophy4"), new GUIContent("Requirement Nr. 4"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + 100, position.width, 18), property.FindPropertyRelative("ArcadeModeTrophy5"), new GUIContent("Requirement Nr. 5"));
        }


        //EditorGUI.HelpBox(new Rect(position.x, position.y + 120, position.width, 18), "Trophy Levels:   None -> Bronze -> Silver -> Gold -> Platinium -> XXXXXXXX", MessageType.None);

        EditorGUI.indentLevel = 0;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 120;
    }
}

[CustomPropertyDrawer(typeof(TrophyRequirementClass))]
public class TrophyRequirementClassDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (property.FindPropertyRelative("RequirementType").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18), property.FindPropertyRelative("RequirementType"), new GUIContent($"{label.text}"));
        }
        else
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 55, 18), property.FindPropertyRelative("RequirementType"), new GUIContent($"{label.text}"));
            EditorGUI.PropertyField(new Rect(position.x + position.width - 65, position.y, 65, 18), property.FindPropertyRelative("RequirementValue"), GUIContent.none);
        }
        EditorGUI.EndProperty();
    }
}

#endif

public enum TrophyRequirement
{
    CompleteLevel = 0,
    ItemsPercent = 1,
    EnemyPercent = 2,
    SecretsPercent = 3,
    LessTimeThan = 7,
    MoreTimeThan = 8,
    MinimumScore = 9,
    MinimumWave = 10,
    Items = 4,
    Enemy = 5,
    Secrets = 6
}

#endregion