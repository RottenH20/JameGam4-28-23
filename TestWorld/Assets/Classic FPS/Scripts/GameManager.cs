using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Set Static Object
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
       
    }

    // Maximum Bullet Amount
    [Line("Ammo Pool (Max)")]
    [Suffix("Max Count")] public int BulletMax;
    [Suffix("Max Count")] public int ShellMax;
    [Suffix("Max Count")] public int ClipMax;
    [Suffix("Max Count")] public int GrenadeMax;
    [Suffix("Max Count")] public int RocketMax;
    [Suffix("Max Count")] public int EnergyCellMax;
    [Suffix("Max Count")] public int HandGrenadeMax;

    // Start Bullet Amount
    [Line("Ammo Pool (At Start)")]
    [Suffix("Start Count")] public int Bullet;
    [Suffix("Start Count")] public int Shell;
    [Suffix("Start Count")] public int Clip;
    [Suffix("Start Count")] public int Grenade;
    [Suffix("Start Count")] public int Rocket;
    [Suffix("Start Count")] public int EnergyCell;
    [Suffix("Start Count")] public int HandGrenade;

    // Total Points
    [Line("Score Points")]
    [Suffix("Points")] public int TotalPoints;

    [HelpBox("Combo Settings")] public float ComboMultiplier = .5f;
    public int MaxMultiplierLevel = 8;
    public float LostComboTime = 12;
    public float AddScoreToNextLevel = 4;

    [Line("In Game Popup")] public GameObject PopupContainer;
    public GameObject PopupPrefab;


    // Privates
    private GameObject deathScreen;
    private GameObject pauseScreen;
    [HideInInspector] public bool paused;

    // Score Multiplier

    // Czas Do Usunięcia levela
    private float multiplierTimer;
    // Poziom Mnożnika
    private float MultiplierLevel;
    // Ile dodanych punktów do następnwego levela
    private float MultiplierScoreToNextLEvel;

    private TMPro.TMP_Text fps_text;
    private TinyInput tinyInput;

    //Ability
    [HideInInspector] public PassiveAbilitySO PassiveSkill;
    [HideInInspector] public BaseAbilityScript Ability;

    private void Awake()
    {
        // Set Static Object
        if (_instance == null)
        {
            _instance = this;
        }
        InitGame();

        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        Ability = am.AbilityList[TinySaveSystem.GetInt("selectedAbility")].skill;

        tinyInput = InputManager.Instance.input;

        foreach(Transform children in PopupContainer.transform)
        {
            GameObject.Destroy(children.gameObject);
        }

        tinyInput.Menu.Pause.performed += ctx => { PauseMenuGame(); };
    }
    public void OnEnable() { tinyInput.Enable(); }
    public void OnDisable() { tinyInput.Disable(); }

    private void Start()
    {
        fps_text = transform.parent.Find("Canvas/UI").Find("FPS_Count").GetComponent<TMPro.TMP_Text>();
    }

    private void Update()
    {
        if(TinySaveSystem.GetInt("settings_ShowFPS") == 0)
        {
            fps_text.text = $"FPS: {(int)(1.0f / Time.smoothDeltaTime)}";
        }
        else
        {
            fps_text.text = "";
        }

        // Score Combo Multiplier
        multiplierTimer += Time.deltaTime;
        if(multiplierTimer > LostComboTime && MultiplierLevel > 0)
        {
            MultiplierLevel--;
            multiplierTimer = 0; 
            MultiplierScoreToNextLEvel = 0;
        }

        if (MultiplierLevel > 0 )
        {
            GameManager.Instance.transform.parent.Find("Canvas/UI/Score Combo").gameObject.SetActive(true);
            GameManager.Instance.transform.parent.Find("Canvas/UI/Score Combo/Combo_Bar").GetComponent<Slider>().value = ((LostComboTime - multiplierTimer) / LostComboTime);
        }
        else
        {
            GameManager.Instance.transform.parent.Find("Canvas/UI/Score Combo").gameObject.SetActive(false);
        }

        TMPTextStringLocalization _tmp = GameManager.Instance.transform.parent.Find("Canvas/UI/Score Combo/Combo_Txt").GetComponent<TMPTextStringLocalization>();
        _tmp.text_tmp.text = string.Format(_tmp.ShowText.localization.GetString("x_combo"), (MultiplierLevel * ComboMultiplier + 1));

        TMPTextStringLocalization _tmp2 = GameManager.Instance.transform.parent.Find("Canvas/UI/Score Combo/Score_Txt").GetComponent<TMPTextStringLocalization>();
        _tmp2.text_tmp.text = string.Format(_tmp.ShowText.localization.GetString("combo_score"), TotalPoints);

    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Disable Cursor & Disable Death Screen
    void InitGame()
    {
        // Get Screens
        deathScreen = transform.Find("DeathScreen").gameObject;
        deathScreen.SetActive(false);
        pauseScreen = transform.Find("PauseScreen").gameObject;
        pauseScreen.SetActive(false);

        // Enable Moves
        Time.timeScale = 1f;
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        paused = false;

        Application.targetFrameRate = 300;
    }


    // IF Player Death, this disable all Player Script, Enable Cursor and Enable Death Screen
    public void PlayerDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerHealth>().enabled = false;

        foreach (Transform child in player.transform)
        {
            if(child.name == "Weapons") { child.gameObject.SetActive(false); }
        }
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (LevelManager.Instance.Gamemode.GameMode == GameModes.Classic)
        {
            deathScreen.SetActive(true);
        }
        else
        {
            LevelManager.Instance.GenerateEndStats();
        }
    }

    // Return Ammo Value
    public int AmmoLeft(AmmoType _ammo)
    {
        if (_ammo == AmmoType.Bullet) return Bullet;
        if (_ammo == AmmoType.Shell) return Shell;
        if (_ammo == AmmoType.Clip) return Clip;
        if (_ammo == AmmoType.Grenade) return Grenade;
        if (_ammo == AmmoType.Rocket) return Rocket;
        if (_ammo == AmmoType.EnergyCell) return EnergyCell;
        if (_ammo == AmmoType.HandGrenade) return HandGrenade;

        if (_ammo == AmmoType.InfinityAmmo) return 10121993;


        return 0;
    }

    // Setup new Bullet Amount
    public void SetNewAmmoAmount(AmmoType _ammo, int newAmount)
    {
        if (_ammo == AmmoType.Bullet) Bullet = newAmount;
        if (_ammo == AmmoType.Shell) Shell = newAmount;
        if (_ammo == AmmoType.Clip) Clip = newAmount;
        if (_ammo == AmmoType.Grenade) Grenade = newAmount;
        if (_ammo == AmmoType.Rocket) Rocket = newAmount;
        if (_ammo == AmmoType.EnergyCell) EnergyCell = newAmount;
        if (_ammo == AmmoType.HandGrenade) HandGrenade = newAmount;
    }

    // Add new Ammo to old
    public void AddAmmo(AmmoType _ammo, int addAmount)
    {
        if (_ammo == AmmoType.Bullet) Bullet += addAmount;              if (Bullet > BulletMax) { Bullet = BulletMax; }
        if (_ammo == AmmoType.Shell) Shell += addAmount;                if (Shell > ShellMax) { Shell = ShellMax; }
        if (_ammo == AmmoType.Clip) Clip += addAmount;                  if (Clip > ClipMax) { Clip = ClipMax; }
        if (_ammo == AmmoType.Grenade) Grenade += addAmount;            if (Grenade > GrenadeMax) { Grenade = GrenadeMax; }
        if (_ammo == AmmoType.Rocket) Rocket += addAmount;              if (Rocket > RocketMax) { Rocket = RocketMax; }
        if (_ammo == AmmoType.EnergyCell) EnergyCell += addAmount;      if (EnergyCell > EnergyCellMax) { EnergyCell = EnergyCellMax; }
        if (_ammo == AmmoType.HandGrenade) HandGrenade += addAmount;    if (HandGrenade > HandGrenadeMax) { HandGrenade = HandGrenadeMax; }
    }

    // Timed Void
    public void ReturnToMainMenu()
    {
        // Get Screens
        Time.timeScale = 1f;
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = true;
        }
        LoadingScreen.instance.loadingScreen(0);
        Debug.Log("Get to Main Menu");
    }
    // Timed Void
    public void RestartLevel()
    {
        LoadingScreen.instance.loadingScreen(SceneManager.GetActiveScene().name);
    }

    public void ExitTheGame()
    {
        TinySaveSystem.SaveToDisk();
        Application.Quit();
    }

    public void CheckPauseGame()
    {
        if (paused)
        {
            Time.timeScale = 0.0000001f;
            if (Application.isMobilePlatform == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (Application.isMobilePlatform == false)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void StartDialog()
    {
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void EndDialog()
    {
        if (Application.isMobilePlatform == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void PauseMenuGame()
    {
        paused = !paused;
        pauseScreen.SetActive(paused);
        CheckPauseGame();
        Application.targetFrameRate = 300;
    }

    public void AddPoints(int points)
    {
        TotalPoints += (int)(points * (MultiplierLevel * ComboMultiplier + 1));

        // Multiplier
        MultiplierScoreToNextLEvel++;
        multiplierTimer = 0;

        // Next Level Multiplier
        if(MultiplierScoreToNextLEvel >= AddScoreToNextLevel && MultiplierLevel < MaxMultiplierLevel)
        {
            MultiplierLevel++;  
            MultiplierScoreToNextLEvel = 0;
        }
    }

    // In Game Popup
    public IEnumerator SpawnInGamePopup(string Text)
    {
        GameObject popup = Instantiate(PopupPrefab,PopupContainer.transform);
        popup.GetComponent<TMP_Text>().text = Text;
        yield return new WaitForSeconds(3f);
        Destroy(popup);
    }
}

public enum AmmoType
{
    // Bullets
    Bullet = 0, Shell = 1, Clip = 2, Grenade = 3, Rocket = 4, EnergyCell = 5, 
    // Grenades
    HandGrenade = 100,
    // Infinity
    InfinityAmmo = 666
}

public enum ControllerType
{
    Modern = 0,
}