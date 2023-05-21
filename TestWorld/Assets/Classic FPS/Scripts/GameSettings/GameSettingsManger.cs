#define HELLISH_BATTLE
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameSettingsManger : ScriptableObject
{
    public const string k_CampaingPath = "Assets/Classic FPS/Resources/Game_Settings.asset";

    private static GameSettingsManger _instance;
    public static GameSettingsManger Instance
    {
        get { return _instance; }
    }

    //* Enemy *//
    [SerializeField] public bool EnemyHealthBar = true;

    //* Player *//
    [SerializeField] public bool EnablePlayerAbility = true;

    [SerializeField] public bool EnablePlayerSprint = true;

    [SerializeField] public bool EnablePlayerDamageOverlay = true;
    [SerializeField] public bool EnablePlayerBonusOverlay = true;
    [SerializeField] public bool EnablePlayerDamageIndicator = true;

    //* Player *//
    [SerializeField] public float WeaponSpreadWalk = 10;
    [SerializeField] public float WeaponSpreadRun = 25;
    //[SerializeField] public bool WeaponSpreadCrouch = true;
    [SerializeField] public float WeaponSpreadJump = 50;

    //* Other *//
    [SerializeField] public int NoneTrophyPoints = 0;
    [SerializeField] public int BronzeTrophyPoints = 1;
    [SerializeField] public int SilverTrophyPoints = 2;
    [SerializeField] public int GoldTrophyPoints = 3;
    [SerializeField] public int DiamondTrophyPoints = 4;
    [SerializeField] public int PlatiniumTrophyPoints = 5;

#if UNITY_EDITOR
    internal static GameSettingsManger GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<GameSettingsManger>(k_CampaingPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GameSettingsManger>();


                AssetDatabase.CreateAsset(settings, k_CampaingPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    #endif
}

#if UNITY_EDITOR
static class GameSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider GameSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Hellish Battle/Asset Settings", SettingsScope.Project)
        {
            label = "Asset Settings",
            guiHandler = (searchContext) =>
            {

                EditorGUILayout.HelpBox("Here you can fine-tune game settings that are not available from the user level, so that each developer can choose what they want and don't want in their game. Text with no fields visible next to them shows what is likely to appear in the future].", MessageType.Info, true);
                EditorGUILayout.Space();
                var settings = GameSettingsManger.GetSerializedSettings();

                EditorGUILayout.LabelField($"Enemy Settings", EditorStyles.largeLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Boss & Enemy Settings", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("EnemyHealthBar"), new GUIContent("Enemy Healthbar"));
                GUI.enabled = false;
                EditorGUILayout.LabelField("Boss Healthbar");
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField($"Player Settings", EditorStyles.largeLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Player's Mechanics", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("EnablePlayerSprint"), new GUIContent("Enable Sprint"));
                GUI.enabled = false;
                EditorGUILayout.LabelField("Enable Double Jump");
                EditorGUILayout.LabelField("Enable Crouch");
                EditorGUILayout.LabelField("Enable Dash");
                GUI.enabled = true;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Player Ability", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("EnablePlayerAbility"), new GUIContent("Enable Player Ability"));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Player UI", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("EnablePlayerBonusOverlay"), new GUIContent("Enable Bonus Overlay"));
                EditorGUILayout.PropertyField(settings.FindProperty("EnablePlayerDamageOverlay"), new GUIContent("Enable Damage Overlay"));
                EditorGUILayout.PropertyField(settings.FindProperty("EnablePlayerDamageIndicator"), new GUIContent("Enable Damage Indicator"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField($"Weapon Settings", EditorStyles.largeLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Weapon Spread", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("WeaponSpreadWalk"), new GUIContent("Walk Spread"));
                EditorGUILayout.PropertyField(settings.FindProperty("WeaponSpreadRun"), new GUIContent("Run Spread"));
                //EditorGUILayout.PropertyField(settings.FindProperty("WeaponSpreadCrouch"), new GUIContent("Crouch Spread"));
                EditorGUILayout.PropertyField(settings.FindProperty("WeaponSpreadJump"), new GUIContent("Jump Spread"));
                GUI.enabled = false;
                EditorGUILayout.LabelField("Crouch Spread");
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField($"Other Settings", EditorStyles.largeLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Points Settings", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("NoneTrophyPoints"), new GUIContent("None Trophy Points"));
                EditorGUILayout.PropertyField(settings.FindProperty("BronzeTrophyPoints"), new GUIContent("Bronze Trophy Points"));
                EditorGUILayout.PropertyField(settings.FindProperty("SilverTrophyPoints"), new GUIContent("Silver Trophy Points"));
                EditorGUILayout.PropertyField(settings.FindProperty("GoldTrophyPoints"), new GUIContent("Gold Trophy Points"));
                EditorGUILayout.PropertyField(settings.FindProperty("DiamondTrophyPoints"), new GUIContent("Diamond Trophy Points"));
                EditorGUILayout.PropertyField(settings.FindProperty("PlatiniumTrophyPoints"), new GUIContent("Platinium Trophy Points"));
                EditorGUILayout.EndVertical();

                GUI.enabled = false;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Achievements", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField("Enable In-Game Notification");
                EditorGUILayout.EndVertical();
                GUI.enabled = true;


                settings.ApplyModifiedPropertiesWithoutUndo();
            }
        };

        return provider;
    }
}

class GameSettingsProvider : SettingsProvider
{
    private SerializedObject m_CustomSettings;

    class Styles
    {
        public static GUIContent chapter = new GUIContent("EnemyHealthBar");
    }

    const string k_CampaingPath = "Assets/Classic FPS/Resources/Game_Settings.asset";
    public GameSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_CampaingPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_CustomSettings = GameSettingsManger.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("EnemyHealthBar"), Styles.chapter);
    }
}

#endif