using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
//using UnityEditor.UIElements;
using UnityEngine.SceneManagement;

public class CampaignManager : ScriptableObject
{
    public const string k_CampaingPath = "Assets/Classic FPS/Scene/Campaing.asset";

    private static CampaignManager _instance;
    public static CampaignManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    public StringLocalization localization;
    [SerializeField]
    public List<Chapter> chapter = new List<Chapter>();
    [SerializeField]
    public List<Chapter> gamemodes = new List<Chapter>();

    public string FindKeyFromSceneName(string sceneName)
    {
        for(int i = 0; i < chapter.Count; i++)
        {
            for (int j = 0; j < chapter[i].Levels.Count; j++)
            {
                if(chapter[i].Levels[j].sceneName == sceneName) { return chapter[i].Levels[j].key; }
            }
        }
        return "Error";
    }

    public string FindKeyFromScene()
    {
        for (int i = 0; i < chapter.Count; i++)
        {
            for (int j = 0; j < chapter[i].Levels.Count; j++)
            {
                if (chapter[i].Levels[j].sceneName == SceneManager.GetActiveScene().name) { return chapter[i].Levels[j].key; }
            }
        }
        return "Error";
    }

    public string CheckNextLevel()
    {
        for (int i = 0; i < chapter.Count; i++)
        {
            for (int j = 0; j < chapter[i].Levels.Count; j++)
            {
                if (chapter[i].Levels[j].sceneName == SceneManager.GetActiveScene().name) 
                {
                    if (j + 1 < chapter[i].Levels.Count)
                    {
                        return chapter[i].Levels[j+1].sceneName;
                    }
                    else
                    {
                        return "Error";
                    }
                }
            }
        }
        return "Error";
    }

#if UNITY_EDITOR
    internal static CampaignManager GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CampaignManager>(k_CampaingPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<CampaignManager>();


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
static class CampaignManagerIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CampaignManagerProvider()
    {
        var provider = new SettingsProvider("Project/Hellish Battle/Campaing", SettingsScope.Project)
        {
            label = "Campaing",
            guiHandler = (searchContext) =>
            {
                var settings = CampaignManager.GetSerializedSettings();
                CampaignManager Campaing = (CampaignManager)AssetDatabase.LoadAssetAtPath("Assets/Classic FPS/Scene/Campaing.asset",typeof(CampaignManager));


                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("String Localization to Scene Name", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(settings.FindProperty("localization"), new GUIContent("Localization"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField($"Chapters [{settings.FindProperty("chapter").arraySize}]", EditorStyles.largeLabel);

                for (int i = 0; i < settings.FindProperty("chapter").arraySize; i++)
                {
                    int index = i;
                    SerializedProperty property1 = settings.FindProperty("chapter");

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Chapter No. {(i + 1).ToString("00")}", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(75));
                    if (GUILayout.Button("Delete Chapter", EditorStyles.toolbarButton, GUILayout.Width(120))) { property1.DeleteArrayElementAtIndex(index); }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    Campaing.chapter[i]._keyID = 0;
                    for (int x = 0; x < Campaing.localization.keyOption.Count; x++)
                    {
                        if (Campaing.chapter[i].key == Campaing.localization.keyOption[x]) { Campaing.chapter[i]._keyID = x; }
                    }

                    Campaing.chapter[i]._keyID = EditorGUILayout.Popup("Chapter Name Key", Campaing.chapter[i]._keyID, Campaing.localization.keyOption.ToArray());
                    Campaing.chapter[i].key = Campaing.localization.wordList[Campaing.chapter[i]._keyID].key;

                    EditorGUILayout.Space(10);

                    // Levels List Toolbar
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Scene Name", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Localization Key", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"", EditorStyles.boldLabel, GUILayout.Width(75));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    // Levels List
                    for (int x = 0; x < property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").arraySize; x++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").GetArrayElementAtIndex(x).FindPropertyRelative("sceneName"), GUIContent.none);

                        Campaing.chapter[i].Levels[x]._keyID = 0;
                        for (int k = 0; k < Campaing.localization.keyOption.Count; k++)
                        {
                            if (Campaing.chapter[i].Levels[x].key == Campaing.localization.keyOption[k]) { Campaing.chapter[i].Levels[x]._keyID = k; }
                        }

                        Campaing.chapter[i].Levels[x]._keyID = EditorGUILayout.Popup(GUIContent.none, Campaing.chapter[i].Levels[x]._keyID, Campaing.localization.keyOption.ToArray());
                        Campaing.chapter[i].Levels[x].key = Campaing.localization.wordList[Campaing.chapter[i].Levels[x]._keyID].key;

                        //EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").GetArrayElementAtIndex(x).FindPropertyRelative("key"), GUIContent.none);
                        if (GUILayout.Button("Delete", GUILayout.Width(75))) { property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").DeleteArrayElementAtIndex(x); }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("+ Add New Level")) { property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").InsertArrayElementAtIndex(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").arraySize); }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("+ Add New Chapter", GUILayout.Height(38))) { Debug.Log("Tet"); settings.FindProperty("chapter").InsertArrayElementAtIndex(settings.FindProperty("chapter").arraySize); }
                EditorGUILayout.Space(10);
                
                ///
                /// Gamemodes
                /// 
                
                EditorGUILayout.LabelField($"Gamemodes [{settings.FindProperty("gamemodes").arraySize}]", EditorStyles.largeLabel);

                for (int i = 0; i < settings.FindProperty("gamemodes").arraySize; i++)
                {
                    int index = i;
                    SerializedProperty property1 = settings.FindProperty("gamemodes");

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Gamemode Chapter No. {(i + 1).ToString("00")}", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(75));
                    if (GUILayout.Button("Delete Chapter", EditorStyles.toolbarButton, GUILayout.Width(120))) { property1.DeleteArrayElementAtIndex(index); }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    Campaing.gamemodes[i]._keyID = 0;
                    for (int x = 0; x < Campaing.localization.keyOption.Count; x++)
                    {
                        if (Campaing.gamemodes[i].key == Campaing.localization.keyOption[x]) { Campaing.gamemodes[i]._keyID = x; }
                    }

                    Campaing.gamemodes[i]._keyID = EditorGUILayout.Popup("Chapter Name Key", Campaing.gamemodes[i]._keyID, Campaing.localization.keyOption.ToArray());
                    Campaing.gamemodes[i].key = Campaing.localization.wordList[Campaing.gamemodes[i]._keyID].key;
                    EditorGUILayout.Space(10);

                    // Levels List Toolbar
                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Scene Name", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Localization Key", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"", EditorStyles.boldLabel, GUILayout.Width(75));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    // Levels List
                    for (int x = 0; x < property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").arraySize; x++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").GetArrayElementAtIndex(x).FindPropertyRelative("sceneName"), GUIContent.none);
                        Campaing.gamemodes[i].Levels[x]._keyID = 0;
                        for (int k = 0; k < Campaing.localization.keyOption.Count; k++)
                        {
                            if (Campaing.gamemodes[i].Levels[x].key == Campaing.localization.keyOption[k]) { Campaing.gamemodes[i].Levels[x]._keyID = k; }
                        }

                        Campaing.gamemodes[i].Levels[x]._keyID = EditorGUILayout.Popup(GUIContent.none, Campaing.gamemodes[i].Levels[x]._keyID, Campaing.localization.keyOption.ToArray());
                        Campaing.gamemodes[i].Levels[x].key = Campaing.localization.wordList[Campaing.gamemodes[i].Levels[x]._keyID].key;
                        //EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").GetArrayElementAtIndex(x).FindPropertyRelative("key"), GUIContent.none);
                        if (GUILayout.Button("Delete", GUILayout.Width(75))) { property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").DeleteArrayElementAtIndex(x); }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("+ Add New Level")) { property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").InsertArrayElementAtIndex(property1.GetArrayElementAtIndex(i).FindPropertyRelative("Levels").arraySize); }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("+ Add New Chapter", GUILayout.Height(38))) { Debug.Log("Tet"); settings.FindProperty("gamemodes").InsertArrayElementAtIndex(settings.FindProperty("gamemodes").arraySize); }

                settings.ApplyModifiedPropertiesWithoutUndo();
                //var settings = CampaignManager.GetSerializedSettings();
                /*EditorGUILayout.Space();
                EditorGUILayout.PropertyField(settings.FindProperty("localization"), new GUIContent("Localization String"));


                for (int i = 0; i < settings.FindProperty("chapter").arraySize; i++)
                {
                    int index = i;
                    SerializedProperty property1 = settings.FindProperty("chapter");

                    Rect r = EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label($"Chapter No. {(i+1).ToString("00")}");
                    GUILayout.FlexibleSpace();
                    var _oldColor = GUI.backgroundColor; GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Delete Chapter", GUILayout.Width(125), GUILayout.Height(EditorGUIUtility.singleLineHeight))) { property1.DeleteArrayElementAtIndex(index); }
                    GUI.backgroundColor = _oldColor;

                    EditorGUILayout.EndHorizontal();
                    if (settings.FindProperty("chapter").arraySize != index) {
                        GUILayout.BeginVertical("box");
                        EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(i), new GUIContent("Chapter"));
                        GUILayout.EndVertical();
                        GUILayout.Label("");
                    }
                }

                GUILayout.BeginVertical("box");
                var oldColor = GUI.backgroundColor; GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Add New Chapter", GUILayout.Height(50))) { settings.FindProperty("chapter").InsertArrayElementAtIndex(settings.FindProperty("chapter").arraySize); }
                GUI.backgroundColor = oldColor;
                GUILayout.EndVertical();

                settings.ApplyModifiedPropertiesWithoutUndo();*/

            }
        };

        return provider;
    }
}

class CampaignManagerProvider : SettingsProvider
{
    private SerializedObject m_CustomSettings;

    class Styles
    {
        public static GUIContent chapter = new GUIContent("chapter");
        public static GUIContent localization = new GUIContent("localization");
        //public static GUIContent selectedLang = new GUIContent("selectedLang");
    }

    const string k_CampaingPath = "Assets/Classic FPS/Scene/Campaing.asset";
    public CampaignManagerProvider(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_CampaingPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_CustomSettings = CampaignManager.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("chapter"), Styles.chapter);
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("localization"), Styles.localization);
        //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("selectedLang"), Styles.selectedLang);
    }
}

#endif


[System.Serializable]
public class Chapter
{
    public string key = "";
    public List<Level> Levels;

    [HideInInspector] public int _keyID;
}


[System.Serializable]
public class Level
{
    public string key = "";
    public string sceneName;

    [HideInInspector] public int _keyID;
}

#if UNITY_EDITOR
/*
[CustomPropertyDrawer(typeof(Level))]
public class LevelPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty nameType = property.FindPropertyRelative("sceneName");
        SerializedProperty keyType = property.FindPropertyRelative("key");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float widthSize = position.width / 2;

        Rect pos1 = new Rect(position.x, position.y, widthSize, position.height);
        Rect pos2 = new Rect(position.x + widthSize + 5, position.y, widthSize - 5, position.height);

        EditorGUI.PropertyField(pos1, nameType, GUIContent.none);
        EditorGUI.PropertyField(pos2, keyType, GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40;
    }
}

[CustomPropertyDrawer(typeof(Chapter))]
public class ChapterPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty keyType = property.FindPropertyRelative("key");

        Rect labelPosition = new Rect(9, position.y, position.width/2, EditorGUIUtility.singleLineHeight);
        Rect keyLabelPosition = new Rect(position.x + (position.width / 2), position.y, position.width/2, EditorGUIUtility.singleLineHeight);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        position = EditorGUI.PrefixLabel(labelPosition, EditorGUIUtility.GetControlID(FocusType.Passive), new GUIContent(keyType.stringValue != "" ? keyType.stringValue : "Empty Key"));
         Rect pos1 = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
         EditorGUI.PropertyField(keyLabelPosition, keyType, GUIContent.none);

        // Line 2
        GUILayout.BeginHorizontal();
        GUILayout.Label(""); GUILayout.Label(""); GUILayout.Label(""); GUILayout.Label("");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Scene Name");
        GUILayout.Label("Localization Key");
        GUILayout.Label("",GUILayout.Width(50));

        GUILayout.EndHorizontal();

        // List
        for (int i = 0; i < property.FindPropertyRelative("Levels").arraySize; i++)
        {
            //Rect r = EditorGUILayout.BeginHorizontal("window", GUILayout.Height(10));
            Rect r = EditorGUILayout.BeginHorizontal("box");
            EditorGUI.PropertyField(new Rect(r.x, r.y, r.width - 80, r.height), property.FindPropertyRelative("Levels").GetArrayElementAtIndex(i));
            GUILayout.FlexibleSpace();
            var _oldColor = GUI.backgroundColor; GUI.backgroundColor = Color.red; 
            if (GUILayout.Button("Delete", GUILayout.Width(75), GUILayout.Height(EditorGUIUtility.singleLineHeight) )) { property.FindPropertyRelative("Levels").DeleteArrayElementAtIndex(i); }
            GUI.backgroundColor = _oldColor;

            EditorGUILayout.EndHorizontal();
        }

        Rect s = EditorGUILayout.BeginHorizontal("box");
        var oldColor = GUI.backgroundColor; GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add New Level", GUILayout.Height(35))) { property.FindPropertyRelative("Levels").InsertArrayElementAtIndex(property.FindPropertyRelative("Levels").arraySize); }
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndHorizontal();


        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
*/
#endif