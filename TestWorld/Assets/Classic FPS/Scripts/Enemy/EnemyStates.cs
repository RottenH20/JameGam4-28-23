/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using Unity.Profiling;
using System.IO;

public class EnemyStates : MonoBehaviour
{
    public EnemyType type;

    [Line("Patrol")]
    public TargetSelectType OrderWaypoints;
    [Suffix("Metres")] public int patrolRange;
    //[ShowIf("OrderWaypoints", 2)] 
    public float ProceduralWaypointRadius = 15f;
    [HelpBox("If 'Procedurally Generated Waypoints' is selected, the waypoint list is not used", HelpBoxEnum.Info)]
    public Transform[] waypoints;

    [Line("Alert")]
    [Suffix("Secound")] public float stayAlertTime;

    [Line("Attack")]
    public EnemyAttackType AttackType;
    public LayerMask raycastMask;

    [Line("Melle")]
    [Suffix("Metres")] public float attackRange;
    public float meleeDamage;
    [Suffix("Secound")] public float melleDelay;

    [Line("Range")]
    [Suffix("Metres")] public float shootRange;
    public GameObject missile;
    public float missileDamage;
    [Suffix("m/s")] public float missileSpeed;
    [Suffix("Secound")] public float rangeDelay;

    [Line("Vision")]
    public Transform vision;
    [Suffix("°")] public float viewAngle;


    [HideInInspector] public AlertState alertState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public IEnemyAI currentState;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public Vector3 lastKnownPosition;
    [HideInInspector] public Vision visionScript;

    float base_meleeDamage;
    float base_melleDelay;
    float base_missileDamage;
    float base_rangeDelay;
    [HideInInspector] public float damageMultiplier;
    //[HideInInspector] public float movementSpeedMultiplier;
    [HideInInspector] public float reduceDelayMultiplier;

    [HideInInspector] public DifficultyLevel difficulty;
    Enemy enemy;

    void Awake()
    {
        // We instantiate each state
        // And we pass the EnemyStates object to them
        visionScript = vision.GetComponent<Vision>();
        alertState = new AlertState(this);
        attackState = new AttackState(this); 
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        DifficultyManager _tmp = (DifficultyManager)Resources.Load("difficulty");
        difficulty = _tmp.CurrentDifficultyLevel();
        enemy = GetComponent<Enemy>();

        // We assign the starting state
        currentState = patrolState;
        base_meleeDamage = meleeDamage;
        base_melleDelay = melleDelay;
        base_missileDamage = missileDamage;
        base_rangeDelay = rangeDelay;
    }

    void Update()
    {
        // Every game frame we perform the actions of the current state
        currentState.UpdateActions();
        if (type == EnemyType.Base) {
            meleeDamage = base_meleeDamage + base_meleeDamage * damageMultiplier * difficulty.damageMultiplier * enemy.PhaseDamageMultiplier;
            missileDamage = base_missileDamage + base_missileDamage * damageMultiplier * difficulty.damageMultiplier * enemy.PhaseDamageMultiplier;
        }
        else {
            meleeDamage = base_meleeDamage + base_meleeDamage * damageMultiplier * difficulty.damageMultiplier * enemy.PhaseDamageMultiplier;
            missileDamage = base_missileDamage + base_missileDamage * damageMultiplier * difficulty.damageMultiplier * enemy.PhaseDamageMultiplier;
        }

        melleDelay = base_melleDelay + base_melleDelay * reduceDelayMultiplier * enemy.PhaseReduceDelayMultiplier;
        rangeDelay = base_rangeDelay + base_rangeDelay * reduceDelayMultiplier * enemy.PhaseReduceDelayMultiplier;
    }

    void OnTriggerEnter(Collider otherObj)
    {
        // After interacting with another object
        // Call the OnTriggerEnter functions according to the current state
        currentState.OnTriggerEnter(otherObj);
    }

    // Function responsible for catching the hero's shots
    // The hero's shooting position is set to be the last known position of the hero's whereabouts
    void HiddenShot(Vector3 shotPosition)
    {
        // Debug.Log("Who Shoot?");
        lastKnownPosition = shotPosition;
        currentState = alertState;
    }

}

public enum EnemyAttackType
{
    Melle = 0, Range = 1, MelleAndRange = 2
}
public enum TargetSelectType
{
    Numerically = 0, Random = 1, ProcedurallyGeneratedWaypoints = 2, PlayerTarget = 3
}