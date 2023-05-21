using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAbility : MonoBehaviour
{
    [Line("Regeneration")]
    public bool EnableRegeneration;
    public float RegenerationInterval = 3;
    public float RegenerationHealth = 5;

    [Line("Rage")]
    public bool EnableRage;
    public float RageHealthMaxLimit = 50;
    [Suffix("%")] public float RageDamageBuff = 3;
    [Suffix("%")] public float RageMovementSpeedBuff = 3;
    [Suffix("%")] public float RageReduceDaleyBuff = 3;

    //[Line("Summon Minion")]
    //public bool EnableSummonMinions;
    //public float SummonCooldown = 15;
    //public float SummonEnemyMinCount = 2;
    //public float SummonEnemyMaxCount = 4;
    //public GameObject EnemyPrefab;

    [Line("Buffs")]
    public BuffChangeClass HealthBuff;
    public BuffChangeClass DamageBuff;
    public BuffChangeClass MovementSpeedBuff;
    public BuffChangeClass ReduceDaleyBuff;

    Enemy enemy;
    EnemyStates enemyState;

    float healthMultiplier;
    float damageMultiplier;
    float movementSpeedMultiplier;
    float reduceDelayMultiplier;
   float regenerationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyState = GetComponent<EnemyStates>();

        // Generate Buffs
        if (Random.Range(0,100) < HealthBuff.BuffChange) { HealthBuff._buffValue = Random.Range(HealthBuff.BuffPercentMin / 100, HealthBuff.BuffPercentMax / 100); }
        if (Random.Range(0,100) < DamageBuff.BuffChange) { DamageBuff._buffValue = Random.Range(DamageBuff.BuffPercentMin / 100, DamageBuff.BuffPercentMax) / 100; }
        if (Random.Range(0,100) < MovementSpeedBuff.BuffChange) { MovementSpeedBuff._buffValue = Random.Range(MovementSpeedBuff.BuffPercentMin / 100, MovementSpeedBuff.BuffPercentMax / 100); }
        if (Random.Range(0,100) < ReduceDaleyBuff.BuffChange) { ReduceDaleyBuff._buffValue = Random.Range(ReduceDaleyBuff.BuffPercentMin / 100, ReduceDaleyBuff.BuffPercentMax / 100); }

        healthMultiplier = HealthBuff._buffValue;
        damageMultiplier = DamageBuff._buffValue;
        reduceDelayMultiplier = ReduceDaleyBuff._buffValue;

        enemy.healthMultiplier = healthMultiplier;
        enemyState.damageMultiplier = damageMultiplier;
        enemyState.reduceDelayMultiplier = reduceDelayMultiplier;
    }

    public void Update()
    {
        regenerationTimer += Time.deltaTime;

        /* Rage */

        damageMultiplier = DamageBuff._buffValue;
        reduceDelayMultiplier = ReduceDaleyBuff._buffValue;

        // Rage
        if (enemy.health < RageHealthMaxLimit && EnableRage)
        {
            damageMultiplier += RageDamageBuff / 100;
            reduceDelayMultiplier += RageReduceDaleyBuff / 100;
        }

        enemyState.damageMultiplier = damageMultiplier;
        enemyState.reduceDelayMultiplier = reduceDelayMultiplier;

        /* Regeneration */

        if(regenerationTimer > RegenerationInterval && EnableRegeneration && enemy.health < enemy.maxHealth)
        {
            enemy.AddHealth(RegenerationHealth);
            regenerationTimer = 0;
        }
    }

    [System.Serializable]
    public class BuffChangeClass
    {
        [Suffix("%")] public float BuffChange = 30;
        [Suffix("%")] public float BuffPercentMin = 10;
        [Suffix("%")] public float BuffPercentMax = 25;

        // Private Only
        [HideInInspector] public float _buffValue;
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(BuffChangeClass))]
    public class BuffChangeClassDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var weightRectLabel = new Rect(position.x, position.y, position.width - 115, 18);
            var weightRect = new Rect(position.x, position.y + 20, position.width - 115, 18);

            EditorGUI.LabelField(weightRectLabel, "Chance of an appearance");
            EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("BuffChange"), GUIContent.none);

            var MinMaxRectLabel = new Rect(position.x + position.width - 110, position.y, 110, 18);

            var MinRect = new Rect(position.x + position.width - 110, position.y + 20, 50, 18);
            var MinMaxRect = new Rect(position.x + position.width - 59, position.y + 20, 9, 18);
            var MaxRect = new Rect(position.x + position.width - 50, position.y + 20, 50, 18);

            EditorGUI.LabelField(MinMaxRectLabel, "    Min      -      Max");
            EditorGUI.PropertyField(MinRect, property.FindPropertyRelative("BuffPercentMin"), GUIContent.none);
            EditorGUI.LabelField(MinMaxRect, "-");
            EditorGUI.PropertyField(MaxRect, property.FindPropertyRelative("BuffPercentMax"), GUIContent.none);

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }
    }

#endif
}
