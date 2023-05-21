using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class AbilityManager : ScriptableObject
{
    public const string k_Path = "Assets/Classic FPS/Resources/Ability.asset";

    private static AbilityManager _instance;
    public static AbilityManager Instance
    {
        get { return _instance; }
    }

    // Property
    public List<PassiveSkillClass> passiveAbilityList;
    public List<AbilityClass> AbilityList;
    //

#if UNITY_EDITOR
    internal static AbilityManager GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AbilityManager>(k_Path);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<AbilityManager>();


            AssetDatabase.CreateAsset(settings, k_Path);
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

[System.Serializable]
public class PassiveSkillClass
{
    public bool UnlockedAtStart;
    public PassiveAbilitySO skill;
    public int price;
}

[System.Serializable]
public class AbilityClass
{
    public bool UnlockedAtStart;
    public BaseAbilityScript skill;
    public int price;
}

#if UNITY_EDITOR
static class AbilityManagerIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider AbilityManagerProvider()
    {
        var provider = new SettingsProvider("Project/Hellish Battle/Ability", SettingsScope.Project)
        {
            label = "Ability",
            guiHandler = (searchContext) =>
            {
                var settings = AbilityManager.GetSerializedSettings();
                AbilityManager ability = (AbilityManager)AssetDatabase.LoadAssetAtPath("Assets/Classic FPS/Resources/Ability.asset", typeof(AbilityManager));

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Ability List", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();

                for (int i = 0; i < ability.AbilityList.Count; i++)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical(GUILayout.Width(150));
                    EditorGUILayout.LabelField(ability.AbilityList[i].skill.name, GUILayout.Width(150));
                    ability.AbilityList[i].UnlockedAtStart = EditorGUILayout.ToggleLeft("Unlocked at Start", ability.AbilityList[i].UnlockedAtStart, GUILayout.Width(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    if (ability.AbilityList[i].skill != null)
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("AbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("skill"), GUIContent.none);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("AbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("skill"), new GUIContent("Empty Ability"));
                    }

                    if (ability.AbilityList[i].UnlockedAtStart)
                    {
                        EditorGUILayout.LabelField("This skill is unlocked from the beginning so you don't need to set its price", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("AbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("price"), new GUIContent("Unlock Price"));
                    }
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("Delete", GUILayout.Width(75), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2 + 2)))
                    {
                        settings.FindProperty("AbilityList").DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    //EditorGUILayout.Space(1);
                }
                if (GUILayout.Button("+ Add New Ability", GUILayout.Height(30)))
                {
                    settings.FindProperty("AbilityList").InsertArrayElementAtIndex(settings.FindProperty("AbilityList").arraySize);
                }

                EditorGUILayout.EndVertical();

                // Passive

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                EditorGUILayout.LabelField("Passive Skill List", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
                for (int i = 0; i < ability.passiveAbilityList.Count; i++)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical(GUILayout.Width(150));
                    EditorGUILayout.LabelField(ability.passiveAbilityList[i].skill.name, GUILayout.Width(150));
                    ability.passiveAbilityList[i].UnlockedAtStart = EditorGUILayout.ToggleLeft("Unlocked at Start", ability.passiveAbilityList[i].UnlockedAtStart, GUILayout.Width(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();

                    if (ability.passiveAbilityList[i].skill != null)
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("passiveAbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("skill"), GUIContent.none);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("passiveAbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("skill"), new GUIContent("Empty Passive Skill"));
                    }

                    if (ability.passiveAbilityList[i].UnlockedAtStart)
                    {
                        EditorGUILayout.LabelField("This skill is unlocked from the beginning so you don't need to set its price", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(settings.FindProperty("passiveAbilityList").GetArrayElementAtIndex(i).FindPropertyRelative("price"), new GUIContent("Unlock Price"));
                    }
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("Delete", GUILayout.Width(75), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2 + 2)))
                    {
                        settings.FindProperty("passiveAbilityList").DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                if (GUILayout.Button("+ Add New Passive Skill", GUILayout.Height(30)))
                {
                    settings.FindProperty("passiveAbilityList").InsertArrayElementAtIndex(settings.FindProperty("passiveAbilityList").arraySize);
                }

                EditorGUILayout.EndVertical();

                settings.ApplyModifiedPropertiesWithoutUndo();
            }
        };

        return provider;
    }
}

class AbilityManagerProvider : SettingsProvider
{
    private SerializedObject m_CustomSettings;

    const string k_Path = "Assets/Classic FPS/Resources/Ability.asset";
    public AbilityManagerProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_Path);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_CustomSettings = AbilityManager.GetSerializedSettings();
    }

}

#endif