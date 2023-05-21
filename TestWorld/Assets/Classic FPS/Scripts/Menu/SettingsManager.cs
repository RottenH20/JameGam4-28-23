using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Line("Selectors")]
    [Header("Game Option")]
    public HorizontalSelector languageSelector;
    public HorizontalSelector difficultySelector;
    [Header("Graphics")]
    public HorizontalSelector resolutionSelector;
    public HorizontalSelector fullscreenSelector;
    public HorizontalSelector qualitySelector;
    public HorizontalSelector limitFpsSelector;
    public HorizontalSelector postProcessingSelector;
    public HorizontalSelector antiAliasingSelector;
    public HorizontalSelector vSyncSelector;
    [Header("Crosshair")]
    public HorizontalSelector enableCrosshair;
    public HorizontalSelector CrosshairType;
    public HorizontalSelector CrosshairSize;
    public HorizontalSelector CrosshairWidth;
    public HorizontalSelector CrosshairHeight;
    public HorizontalSelector CrosshairOffset;
    public HorizontalSelector CrosshairDot;
    public HorizontalSelector DynamicCrosshair;
    public Slider CrosshairColorRed;
    public Slider CrosshairColorGreen;
    public Slider CrosshairColorBlue;
    [Header("HUD")]
    public HorizontalSelector showUISelector;
    public HorizontalSelector playerFaceSelector;
    public HorizontalSelector healthSelector;
    public HorizontalSelector bossBarSelector;
    public HorizontalSelector weaponUISelector;
    public HorizontalSelector scoreUISelector;
    public HorizontalSelector gamemodeUISelector;
    public HorizontalSelector showFPSSelector;
    public HorizontalSelector DamageIndicatorSelector;
    public HorizontalSelector DamageOverlaySelector;
    [Header("Audio")]
    //public HorizontalSelector resolutionSelector;


    public LocalizationManager _lm;

    // Game Option
    [HideInInspector] public int difficultyOption;
    // Graphics
    [HideInInspector] public int resolutionOption;
    [HideInInspector] public bool fullscreenOption;
    [HideInInspector] public int qualityOption;
    [HideInInspector] public int limitFpsOption;
    [HideInInspector] public int vSyncOption;
    [HideInInspector] public bool postProcessingOption;
    [HideInInspector] public int antiAliasingOption;
    // HUD
    [HideInInspector] public int showUIOption;
    [HideInInspector] public int playerFaceOption;
    [HideInInspector] public int healthOption;
    [HideInInspector] public int bossBarOption;
    [HideInInspector] public int weaponUIOption;
    [HideInInspector] public int scoreUIOption;
    [HideInInspector] public int gamemodeUIOption;
    [HideInInspector] public int showFPSOption;
    [HideInInspector] public int DamageIndicatorOption;
    [HideInInspector] public int DamageOverlayOption;
    // Crosshair
    [HideInInspector] public int enableCrosshairOption;
    [HideInInspector] public int CrosshairTypeOption;
    [HideInInspector] public int CrosshairSizeOption;
    [HideInInspector] public int CrosshairWidthOption;
    [HideInInspector] public int CrosshairHeightOption;
    [HideInInspector] public int CrosshairOffsetOption;
    [HideInInspector] public int CrosshairDotOption;
    [HideInInspector] public int DynamicCrosshairOption;
    [HideInInspector] public int CrosshairColorRedOption;
    [HideInInspector] public int CrosshairColorGreenOption;
    [HideInInspector] public int CrosshairColorBlueOption;

    private DifficultyManager _dm;


    // PRivates
    Resolution[] resolutions;

    private void Start()
    {
        _dm = (DifficultyManager)Resources.Load("difficulty");

        // Localization
        languageSelector.KeyList.Clear();
        languageSelector.KeyList.Add("English");
        for (int i = 0; i < _lm.lang.Count; i++) { languageSelector.KeyList.Add(_lm.LocalizationLocalizationName(_lm.lang[i])); }

        // Difficulty
        difficultySelector.KeyList.Clear();
        difficultySelector.localization = _dm.localization;
        for (int i = 0; i < _dm.DifficultySettingsList.Count; i++) { difficultySelector.KeyList.Add(_dm.DifficultySettingsList[i].key); }

        if (Application.isMobilePlatform == false)
        {
            resolutions = Screen.resolutions; // Get all resolution
            LoadSaveSettings();
        }

        if (Application.isMobilePlatform == false)
        {
            // Resolution
            resolutionSelector.KeyList.Clear();
            for (int i = 0; i < resolutions.Length; i++) { resolutionSelector.KeyList.Add(ResolutionToString(resolutions[i])); Screen.fullScreen = true; }
            resolutionSelector.OnChange.AddListener(delegate { resolutionOption = resolutionSelector.index; GraphicSettings(); });

            // Full Screen
            fullscreenSelector.OnChange.AddListener(delegate { fullscreenOption = fullscreenSelector.index == 0 ? true : false; GraphicSettings(); });
        }

        //HUD
        difficultySelector.OnChange.AddListener(delegate { difficultyOption = difficultySelector.index; GraphicSettings(); });

        // Graphics
        limitFpsSelector.OnChange.AddListener(delegate { limitFpsOption = limitFpsSelector.index; GraphicSettings(); });
        qualitySelector.OnChange.AddListener(delegate { qualityOption = qualitySelector.index; GraphicSettings(); });
        vSyncSelector.OnChange.AddListener(delegate { vSyncOption = vSyncSelector.index; GraphicSettings(); });
        postProcessingSelector.OnChange.AddListener(delegate { postProcessingOption = postProcessingSelector.index == 0 ? true : false; GraphicSettings(); });
        antiAliasingSelector.OnChange.AddListener(delegate { antiAliasingOption = antiAliasingSelector.index; GraphicSettings(); });

        //HUD
        showUISelector.OnChange.AddListener(delegate { showUIOption = showUISelector.index; GraphicSettings(); });
        playerFaceSelector.OnChange.AddListener(delegate { playerFaceOption = playerFaceSelector.index; GraphicSettings(); });
        healthSelector.OnChange.AddListener(delegate { healthOption = healthSelector.index; GraphicSettings(); });
        bossBarSelector.OnChange.AddListener(delegate { bossBarOption = bossBarSelector.index; GraphicSettings(); });
        weaponUISelector.OnChange.AddListener(delegate { weaponUIOption = weaponUISelector.index; GraphicSettings(); });
        scoreUISelector.OnChange.AddListener(delegate { scoreUIOption = scoreUISelector.index; GraphicSettings(); });
        gamemodeUISelector.OnChange.AddListener(delegate { gamemodeUIOption = gamemodeUISelector.index; GraphicSettings(); });
        showFPSSelector.OnChange.AddListener(delegate { showFPSOption = showFPSSelector.index; GraphicSettings(); });
        DamageIndicatorSelector.OnChange.AddListener(delegate { DamageIndicatorOption = DamageIndicatorSelector.index; GraphicSettings(); });
        DamageOverlaySelector.OnChange.AddListener(delegate { DamageOverlayOption = DamageOverlaySelector.index; GraphicSettings(); });

        // Crosshair
        enableCrosshair.OnChange.AddListener(delegate { enableCrosshairOption = enableCrosshair.index; GraphicSettings(); });
        CrosshairType.OnChange.AddListener(delegate { CrosshairTypeOption = CrosshairType.index; GraphicSettings(); });
        CrosshairSize.OnChange.AddListener(delegate { CrosshairSizeOption = CrosshairSize.index; GraphicSettings(); });
        CrosshairWidth.OnChange.AddListener(delegate { CrosshairWidthOption = CrosshairWidth.index; GraphicSettings(); });
        CrosshairHeight.OnChange.AddListener(delegate { CrosshairHeightOption = CrosshairHeight.index; GraphicSettings(); });
        CrosshairOffset.OnChange.AddListener(delegate { CrosshairOffsetOption = CrosshairOffset.index; GraphicSettings(); });
        CrosshairDot.OnChange.AddListener(delegate { CrosshairDotOption = CrosshairDot.index; GraphicSettings(); });
        DynamicCrosshair.OnChange.AddListener(delegate { DynamicCrosshairOption = DynamicCrosshair.index; GraphicSettings(); });
        CrosshairColorRed.onValueChanged.AddListener(delegate { CrosshairColorRedOption = (int)CrosshairColorRed.value; GraphicSettings(); });
        CrosshairColorGreen.onValueChanged.AddListener(delegate { CrosshairColorGreenOption = (int)CrosshairColorGreen.value; GraphicSettings(); });
        CrosshairColorBlue.onValueChanged.AddListener(delegate { CrosshairColorBlueOption = (int)CrosshairColorBlue.value; GraphicSettings(); });

        GraphicSettings();

    }

    public void LoadSaveSettings()
    {
        #region Graphic

        // Check Values
        _lm.LoadLocalization();
        if (Application.isMobilePlatform == false)
        {
            if (!TinySaveSystem.HasKey("settings_resolution")) { TinySaveSystem.SetInt("settings_resolution", resolutions.Length - 1); }
            if (!TinySaveSystem.HasKey("settings_fullscreen")) { TinySaveSystem.SetBool("settings_fullscreen", true); }
        }
        //
        if (!TinySaveSystem.HasKey("settings_difficulty")) { TinySaveSystem.SetInt("settings_difficulty", _dm.DefualtDifficulty); }
        //
        if (!TinySaveSystem.HasKey("settings_quality")) { TinySaveSystem.SetInt("settings_quality", 4); }
        if (!TinySaveSystem.HasKey("settings_limitFPS")) { TinySaveSystem.SetInt("settings_limitFPS", 1); }
        if (!TinySaveSystem.HasKey("settings_vSyncFPS")) { TinySaveSystem.SetInt("settings_vSyncFPS", 0); }
        if (!TinySaveSystem.HasKey("settings_postProcessing")) { TinySaveSystem.SetBool("settings_postProcessing", true); }
        if (!TinySaveSystem.HasKey("settings_antiAliasing")) { TinySaveSystem.SetInt("settings_antiAliasing", 0); }
        // HUD
        if (!TinySaveSystem.HasKey("settings_showUI")) { TinySaveSystem.SetInt("settings_showUI", 0); }
        if (!TinySaveSystem.HasKey("settings_playerFace")) { TinySaveSystem.SetInt("settings_playerFace", 0); }
        if (!TinySaveSystem.HasKey("settings_health")) { TinySaveSystem.SetInt("settings_health", 1); }
        if (!TinySaveSystem.HasKey("settings_bossBar")) { TinySaveSystem.SetInt("settings_bossBar", 0); }
        if (!TinySaveSystem.HasKey("settings_weaponUI")) { TinySaveSystem.SetInt("settings_weaponUI", 0); }
        if (!TinySaveSystem.HasKey("settings_scoreUI")) { TinySaveSystem.SetInt("settings_scoreUI", 0); }
        if (!TinySaveSystem.HasKey("settings_gamemodeUI")) { TinySaveSystem.SetInt("settings_gamemodeUI", 0); }
        if (!TinySaveSystem.HasKey("settings_showFPS")) { TinySaveSystem.SetInt("settings_showFPS", 0); }
        if (!TinySaveSystem.HasKey("settings_damageIndicator")) { TinySaveSystem.SetInt("settings_damageIndicator", 0); }
        if (!TinySaveSystem.HasKey("settings_damageOverlay")) { TinySaveSystem.SetInt("settings_damageOverlay", 0); }
        // Crosshair
        if (!TinySaveSystem.HasKey("settings_crosshairEnable")) { TinySaveSystem.SetInt("settings_crosshairEnable", 0); }
        if (!TinySaveSystem.HasKey("settings_crosshairType")) { TinySaveSystem.SetInt("settings_crosshairType", 0); }
        if (!TinySaveSystem.HasKey("settings_crosshairSize")) { TinySaveSystem.SetInt("settings_crosshairSize", 1); }
        if (!TinySaveSystem.HasKey("settings_crosshairWidth")) { TinySaveSystem.SetInt("settings_crosshairWidth", 1); }
        if (!TinySaveSystem.HasKey("settings_crosshairHeight")) { TinySaveSystem.SetInt("settings_crosshairHeight", 1); }
        if (!TinySaveSystem.HasKey("settings_crosshairOffset")) { TinySaveSystem.SetInt("settings_crosshairOffset", 3); }
        if (!TinySaveSystem.HasKey("settings_crosshairDot")) { TinySaveSystem.SetInt("settings_crosshairDot", 0); }
        if (!TinySaveSystem.HasKey("settings_crosshairDynamic")) { TinySaveSystem.SetInt("settings_crosshairDynamic", 0); }
        if (!TinySaveSystem.HasKey("settings_crosshairColor_r")) { TinySaveSystem.SetInt("settings_crosshairColor_r", 15); }
        if (!TinySaveSystem.HasKey("settings_crosshairColor_g")) { TinySaveSystem.SetInt("settings_crosshairColor_g", 255); }
        if (!TinySaveSystem.HasKey("settings_crosshairColor_b")) { TinySaveSystem.SetInt("settings_crosshairColor_b", 0); }

        // Get Values
        if (Application.isMobilePlatform == false)
        {
            resolutionOption = TinySaveSystem.GetInt("settings_resolution"); resolutionSelector.startIndex = resolutionOption;
            fullscreenOption = TinySaveSystem.GetBool("settings_fullscreen"); fullscreenSelector.startIndex = fullscreenOption ? 0 : 1;
        }
        // HUD
        difficultyOption = TinySaveSystem.GetInt("settings_difficulty"); difficultySelector.startIndex = difficultyOption;
        if (difficultySelector.startIndex != difficultySelector.index) { difficultySelector.index = difficultySelector.startIndex; }
        //
        qualityOption = TinySaveSystem.GetInt("settings_quality"); qualitySelector.startIndex = qualityOption;
        limitFpsOption = TinySaveSystem.GetInt("settings_limitFPS"); limitFpsSelector.startIndex = limitFpsOption;
        vSyncOption = TinySaveSystem.GetInt("settings_vSyncFPS"); vSyncSelector.startIndex = vSyncOption;
        postProcessingOption = TinySaveSystem.GetBool("settings_postProcessing"); postProcessingSelector.startIndex = postProcessingOption == false ? 1 : 0;
        antiAliasingOption = TinySaveSystem.GetInt("settings_antiAliasing"); antiAliasingSelector.startIndex = antiAliasingOption;
        // HUD
        showUIOption = TinySaveSystem.GetInt("settings_showUI"); showUISelector.startIndex = showUIOption;
        playerFaceOption = TinySaveSystem.GetInt("settings_playerFace"); playerFaceSelector.startIndex = playerFaceOption;
        healthOption = TinySaveSystem.GetInt("settings_health"); healthSelector.startIndex = healthOption;
        bossBarOption = TinySaveSystem.GetInt("settings_bossBar"); bossBarSelector.startIndex = bossBarOption;
        weaponUIOption = TinySaveSystem.GetInt("settings_weaponUI"); weaponUISelector.startIndex = weaponUIOption;
        scoreUIOption = TinySaveSystem.GetInt("settings_scoreUI"); scoreUISelector.startIndex = scoreUIOption;
        gamemodeUIOption = TinySaveSystem.GetInt("settings_gamemodeUI"); gamemodeUISelector.startIndex = gamemodeUIOption;
        showFPSOption = TinySaveSystem.GetInt("settings_showFPS"); showFPSSelector.startIndex = showFPSOption;
        DamageIndicatorOption = TinySaveSystem.GetInt("settings_damageIndicator"); DamageIndicatorSelector.startIndex = DamageIndicatorOption;
        DamageOverlayOption = TinySaveSystem.GetInt("settings_damageOverlay"); DamageOverlaySelector.startIndex = DamageOverlayOption;
        // Crosshair
        enableCrosshairOption = TinySaveSystem.GetInt("settings_crosshairEnable"); enableCrosshair.startIndex = enableCrosshairOption; enableCrosshair.index = enableCrosshairOption;
        CrosshairTypeOption = TinySaveSystem.GetInt("settings_crosshairType"); CrosshairType.startIndex = CrosshairTypeOption; CrosshairType.index = CrosshairTypeOption;
        CrosshairSizeOption = TinySaveSystem.GetInt("settings_crosshairSize"); CrosshairSize.startIndex = CrosshairSizeOption; CrosshairSize.index = CrosshairSizeOption;
        CrosshairWidthOption = TinySaveSystem.GetInt("settings_crosshairWidth"); CrosshairWidth.startIndex = CrosshairWidthOption; CrosshairWidth.index = CrosshairWidthOption;
        CrosshairHeightOption = TinySaveSystem.GetInt("settings_crosshairHeight"); CrosshairHeight.startIndex = CrosshairHeightOption; CrosshairHeight.index = CrosshairHeightOption;
        CrosshairOffsetOption = TinySaveSystem.GetInt("settings_crosshairOffset"); CrosshairOffset.startIndex = CrosshairOffsetOption; CrosshairOffset.index = CrosshairOffsetOption;
        CrosshairDotOption = TinySaveSystem.GetInt("settings_crosshairDot"); CrosshairDot.startIndex = CrosshairDotOption; CrosshairDot.index = CrosshairDotOption;
        DynamicCrosshairOption = TinySaveSystem.GetInt("settings_crosshairDynamic"); DynamicCrosshair.startIndex = DynamicCrosshairOption; DynamicCrosshair.index = DynamicCrosshairOption;
        CrosshairColorRedOption = TinySaveSystem.GetInt("settings_crosshairColor_r"); CrosshairColorRed.value = CrosshairColorRedOption;
        CrosshairColorGreenOption = TinySaveSystem.GetInt("settings_crosshairColor_g"); CrosshairColorGreen.value = CrosshairColorGreenOption;
        CrosshairColorBlueOption = TinySaveSystem.GetInt("settings_crosshairColor_b"); CrosshairColorBlue.value = CrosshairColorBlueOption;

        #endregion

        // Get Language Selector
        languageSelector.startIndex = 0;
        for (int i = 0; i < _lm.lang.Count; i++)
        {
            if (_lm.lang[i] == _lm.selectedLang) { languageSelector.startIndex = i + 1; }
        }
    }

    public void GraphicSettings()
    {
        if (Application.isMobilePlatform == false && (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.LinuxEditor && Application.platform != RuntimePlatform.OSXEditor))
        {
            // Fullscreen
            Screen.fullScreen = fullscreenOption;
            // Resolution
            Screen.SetResolution(resolutions[resolutionOption].width, resolutions[resolutionOption].height, fullscreenOption);
        }
        // Limit FPS
        Application.targetFrameRate = GetLimitFPS(limitFpsOption);
        QualitySettings.vSyncCount = 0;
        // Quality Level
        QualitySettings.SetQualityLevel(qualityOption);

        QualitySettings.vSyncCount = vSyncOption;

        // Save Values
        //HUD
        TinySaveSystem.SetInt("settings_difficulty", difficultyOption);
        //
        TinySaveSystem.SetInt("settings_resolution", resolutionOption);
        TinySaveSystem.SetBool("settings_fullscreen", fullscreenOption);
        TinySaveSystem.SetInt("settings_health", qualityOption);
        TinySaveSystem.SetInt("settings_limitFPS", limitFpsOption);
        TinySaveSystem.SetInt("settings_vSyncFPS", vSyncOption);
        TinySaveSystem.SetBool("settings_postProcessing", postProcessingOption);
        TinySaveSystem.SetInt("settings_antiAliasing", antiAliasingOption);
        //HUD
        TinySaveSystem.SetInt("settings_showUI", showUIOption);
        TinySaveSystem.SetInt("settings_playerFace", playerFaceOption);
        TinySaveSystem.SetInt("settings_health", healthOption);
        TinySaveSystem.SetInt("settings_bossBar", bossBarOption);
        TinySaveSystem.SetInt("settings_weaponUI", weaponUIOption);
        TinySaveSystem.SetInt("settings_scoreUI", scoreUIOption);
        TinySaveSystem.SetInt("settings_gamemodeUI", gamemodeUIOption);
        TinySaveSystem.SetInt("settings_showFPS", showFPSOption);
        TinySaveSystem.SetInt("settings_damageIndicator", DamageIndicatorOption);
        TinySaveSystem.SetInt("settings_damageOverlay", DamageOverlayOption);
        // Crosshair
        TinySaveSystem.SetInt("settings_crosshairEnable", enableCrosshairOption);
        TinySaveSystem.SetInt("settings_crosshairType", CrosshairTypeOption);
        TinySaveSystem.SetInt("settings_crosshairSize", CrosshairSizeOption);
        TinySaveSystem.SetInt("settings_crosshairWidth", CrosshairWidthOption);
        TinySaveSystem.SetInt("settings_crosshairHeight", CrosshairHeightOption);
        TinySaveSystem.SetInt("settings_crosshairOffset", CrosshairOffsetOption);
        TinySaveSystem.SetInt("settings_crosshairDot", CrosshairDotOption);
        TinySaveSystem.SetInt("settings_crosshairDynamic", DynamicCrosshairOption);
        TinySaveSystem.SetInt("settings_crosshairColor_r", CrosshairColorRedOption);
        TinySaveSystem.SetInt("settings_crosshairColor_g", CrosshairColorGreenOption);
        TinySaveSystem.SetInt("settings_crosshairColor_b", CrosshairColorBlueOption);

    }

    public void ChangeLanguage(int index)
    {
        if (index < 0) { _lm.selectedLang = SystemLanguage.English; }
        else { _lm.selectedLang = _lm.lang[index]; }
        _lm.SaveLocalization(_lm.selectedLang);
    }

    public void ChangeResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, false);
    }

    string ResolutionToString(Resolution res)
    {
        return res.width + " x " + res.height;
    }

    public int GetLimitFPS(int index)
    {
        switch (index)
        {
            case 0:
                return 30;
            case 1:
                return 60;
            case 2:
                return 90;
            case 3:
                return 120;
            case 4:
                return 240;
            case 5:
                return 360;
            default:
                return 999;
        }
    }
}
