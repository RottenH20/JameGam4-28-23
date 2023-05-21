using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Passive Ability", menuName = "Hellish Battle/Passive Skill")]
public class PassiveAbilitySO : ScriptableObject
{
    public Sprite Icon;
    public LocalizedString Name;
    public LocalizedString Description;

    //[Line("Health Regeneration")]
    public bool HealthRegeneration;
    public float HealthRegenerationTimeInterval = 2f;
    public float HealthRegenerationAmount = 5f;
    public float HealthRegenerationWaitingTime = 5f;

    //[Line("Armor Regeneration")]
    public bool ArmorRegeneration;
    public float ArmorRegenerationTimeInterval = 2f;
    public float ArmorRegenerationAmount = 5f;
    public float ArmorRegenerationWaitingTime = 5f;

    //[Line("Boost Health")]
    public bool BoostHealth;
    public float AdditionalHealth = 50;
    public float AdditionalStartHealth = 0;

    //[Line("Boost Armor")]
    public bool BoostArmor;
    public float AdditionalArmor = 50;
    public float AdditionalStartArmor = 0;

    //[Line("Boost Speed")]
    public bool BoostSpeed;
    [Range(1, 2)] public float SpeedIncrease = 1.2f;

    //[Line("Weapon Boost")]
    public bool BoostWeapon;
    [Range(1, 2)] public float RecoilReduce = 1.2f;
    [Range(1, 2)] public float DamageIncrease = 1.2f;
    [Range(1, 2)] public float FireRateIncrease = 1.2f;


    public bool Vampirizm;
    [Range(0, 1)] public float VampirizmChange = .75f;
    [Range(0, 1)] public float VampirizmPercent = .1f;
}

#if UNITY_EDITOR

[CustomEditor(typeof(PassiveAbilitySO))]
public class PassiveAbilitySOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorUtility.SetDirty(target);
        PassiveAbilitySO passive = (PassiveAbilitySO)target;

        // Icon
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Passive Skill Icon", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"), new GUIContent("Passive Skill Icon"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

        // Health Regeneration
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Health Regeneration", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.HealthRegeneration = EditorGUILayout.Toggle(new GUIContent("Health Regeneration Enable"), passive.HealthRegeneration);
        if (passive.HealthRegeneration)
        {
            passive.HealthRegenerationAmount = EditorGUILayout.FloatField(new GUIContent("Amount"), passive.HealthRegenerationAmount);
            passive.HealthRegenerationTimeInterval = EditorGUILayout.FloatField(new GUIContent("Time Interval"), passive.HealthRegenerationTimeInterval);
            passive.HealthRegenerationWaitingTime = EditorGUILayout.FloatField(new GUIContent("Waiting Time"), passive.HealthRegenerationWaitingTime);
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(new GUIContent($"After {passive.HealthRegenerationWaitingTime} seconds of damage taken, every {passive.HealthRegenerationTimeInterval} seconds adds {passive.HealthRegenerationAmount} Health\nAverage regeneration is {passive.HealthRegenerationAmount / passive.HealthRegenerationTimeInterval} Health per secound"));
        }
        EditorGUILayout.EndVertical();

        // Health Regeneration
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Armor Regeneration", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.ArmorRegeneration = EditorGUILayout.Toggle(new GUIContent("Armor Regeneration Enable"), passive.ArmorRegeneration);
        if (passive.ArmorRegeneration)
        {
            passive.ArmorRegenerationAmount = EditorGUILayout.FloatField(new GUIContent("Amount"), passive.ArmorRegenerationAmount);
            passive.ArmorRegenerationTimeInterval = EditorGUILayout.FloatField(new GUIContent("Time Interval"), passive.ArmorRegenerationTimeInterval);
            passive.ArmorRegenerationWaitingTime = EditorGUILayout.FloatField(new GUIContent("Waiting Time"), passive.ArmorRegenerationWaitingTime);
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(new GUIContent($"After {passive.ArmorRegenerationWaitingTime} seconds of damage taken, every {passive.ArmorRegenerationTimeInterval} seconds adds {passive.ArmorRegenerationAmount} Armor\nAverage regeneration is {passive.ArmorRegenerationAmount/passive.ArmorRegenerationTimeInterval} Armor per secound"));
        }
        EditorGUILayout.EndVertical();

        // Boost Health
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Boost Health", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.BoostHealth = EditorGUILayout.Toggle(new GUIContent("Boost Health Enable"), passive.BoostHealth);
        if (passive.BoostHealth)
        {
            passive.AdditionalHealth = EditorGUILayout.FloatField(new GUIContent("Additional Health"), passive.AdditionalHealth);
            passive.AdditionalStartHealth = EditorGUILayout.FloatField(new GUIContent("Additional Start Health"), passive.AdditionalStartHealth);
        }
        EditorGUILayout.EndVertical();

        // Boost Armor
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Boost Armor", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.BoostArmor = EditorGUILayout.Toggle(new GUIContent("Boost Armor Enable"), passive.BoostArmor);
        if (passive.BoostArmor)
        {
            passive.AdditionalArmor = EditorGUILayout.FloatField(new GUIContent("Additional Armor"), passive.AdditionalArmor);
            passive.AdditionalStartArmor = EditorGUILayout.FloatField(new GUIContent("Additional Start Armor"), passive.AdditionalStartArmor);
        }
        EditorGUILayout.EndVertical();

        // Boost Armor
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Boost Speed", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.BoostSpeed = EditorGUILayout.Toggle(new GUIContent("Boost Speed Enable"), passive.BoostSpeed);
        if (passive.BoostSpeed)
        {
            passive.SpeedIncrease = EditorGUILayout.Slider(new GUIContent("Speed Increase"), passive.SpeedIncrease, 1, 2);
        }
        EditorGUILayout.EndVertical();

        // Boost Weapon
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Boost Weapon", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.BoostWeapon = EditorGUILayout.Toggle(new GUIContent("Boost Weapon Enable"), passive.BoostWeapon);
        if (passive.BoostWeapon)
        {
            passive.RecoilReduce = EditorGUILayout.Slider(new GUIContent("Recoil Reduce"), passive.RecoilReduce, 1, 2);
            passive.DamageIncrease = EditorGUILayout.Slider(new GUIContent("Damage Increase"), passive.DamageIncrease, 1, 2);
            passive.FireRateIncrease = EditorGUILayout.Slider(new GUIContent("Fire Rate Increase"), passive.FireRateIncrease, 1, 2);
        }
        EditorGUILayout.EndVertical();

        // Vampirizm
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Vampirizm", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        passive.Vampirizm = EditorGUILayout.Toggle(new GUIContent("Vampirizm Enable"), passive.Vampirizm);
        if (passive.Vampirizm)
        {
            passive.VampirizmChange = EditorGUILayout.Slider(new GUIContent("Vampirizm Change"), passive.VampirizmChange, 0, 1);
            passive.VampirizmPercent = EditorGUILayout.Slider(new GUIContent("Vampirizm Percent"), passive.VampirizmPercent, 0, 1);
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif