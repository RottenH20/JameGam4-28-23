using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
using UnityEngine.Events;

public class Enemy : MonoBehaviour {

    [Line("Health")]
    public int maxHealth;
    public float health;
    public Slider HealthBar;

    [Line("Score")]
    [Suffix("Score")] public int ScorePoints = 100;

    [Line("Blood")]
    public int ExplosionBloodAmount;
    public GameObject[] Blood;

    [Line("Sound")]
    public AudioClip[] RangeAttackSound;
    public AudioClip[] MelleAttackSound;
    public AudioClip[] TakeDamageSound;
    public AudioClip[] DeathSound;

    [Line("Dead Settings")]
    public bool SpawnGrave = true;
    public Sprite deadBody;
    public Vector3 DeadBodyCenter = new Vector3(0, 0, 0);
    public Vector3 DeadBodySize = new Vector3(1, 1, .1f);

    [Line("Drop Loot")]
    public LootDrop lootDrop;
    public int RadnomLootChange;

    [Line("Events")]
    public UnityEvent DoWhenDeath;

    [HideInInspector] public AudioSource source;
    EnemyStates es;
    NavMeshAgent nma;
    SpriteRenderer sr;
    BoxCollider bc;
    GameSettingsManger _AM;
    float baseSpeed;

    [HideInInspector] public float PhaseDamageMultiplier = 1;
    [HideInInspector] public float PhaseSpeedMultiplier = 1;
    [HideInInspector] public float PhaseReduceDelayMultiplier = 1;
    [HideInInspector] public float healthMultiplier;
    [HideInInspector] public int secret_id;

    private void Awake()
    {
        secret_id = Random.Range(0, 999999999);
    }

    private void Start()
    {
        DifficultyManager _tmp = (DifficultyManager)Resources.Load("difficulty");
        DifficultyLevel difficulty = _tmp.CurrentDifficultyLevel();

        es = GetComponent<EnemyStates>();
        nma = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider>();
        source = GetComponent<AudioSource>();

        if (es.type == EnemyType.Base) { maxHealth = (int)(maxHealth * difficulty.healthMultiplier) + (int)(maxHealth * healthMultiplier); }
        else { maxHealth = (int)(maxHealth * difficulty.bossHealthMultiplier) + (int)(maxHealth * healthMultiplier); }
        health = maxHealth;
        if (HealthBar != null) HealthBar.maxValue = maxHealth;
        if (HealthBar != null) HealthBar.value = maxHealth;

        LevelManager.Instance.Enemy.Add(this);
        LevelManager.Instance.FindEnemy.Add(false);

        _AM = Resources.Load<GameSettingsManger>("Game_Settings");
        if (HealthBar != null) { if (_AM.EnemyHealthBar) { transform.Find("Canvas").gameObject.SetActive(true); } else { transform.Find("Canvas").gameObject.SetActive(false); } }

        if (es.type == EnemyType.Base) { nma.speed *= difficulty.speedMultiplier; baseSpeed = nma.speed; }
        else { nma.speed *= difficulty.bossHpeedMultiplier; baseSpeed = nma.speed; }

    }

    private void Update()
    {
        if (health <= 0)
        {
            es.enabled = false;
            nma.enabled = false;

            if (SpawnGrave)
            {
                sr.sprite = deadBody;
                bc.center = DeadBodyCenter;
                bc.size = DeadBodySize;
            }
            else
            {
                Destroy(this.transform.gameObject);
            }

            if (TryGetComponent(out EnemyAbility ability)) { ability.enabled = false; }
        }
        else
        {
            // Phase
            nma.speed = baseSpeed * PhaseSpeedMultiplier;
        }
    }

    public void TakeDamage (float damage)
    {
        health -= damage;
        if (HealthBar != null) HealthBar.value = health;

        // Vampirizm Passive
        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveAbilitySO selectedPassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        if (selectedPassiveSkill.Vampirizm)
        {
            if(Random.Range(0f, 1f) > 1 - selectedPassiveSkill.VampirizmChange)
            {
                GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddHealth(damage * selectedPassiveSkill.VampirizmPercent, false);
            }
        }

        // Do when Death
        if (health <= 0 && es.enabled)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            source.PlayOneShot(DeathSound[Random.Range(0, DeathSound.Length)]);
            SpawnDrop();
            DoWhenDeath.Invoke();
            GameManager.Instance.AddPoints(ScorePoints);
            LevelManager.Instance.TryFindEnemy(this);
        }
        else if (health > 0 && es.enabled)
        {
            source.PlayOneShot(TakeDamageSound[Random.Range(0, TakeDamageSound.Length)]);
        }
    }

    public void AddHealth(float value)
    {
        if (health + value <= maxHealth) { health += value; }
        else {  health = maxHealth; }
        // Double Check
        if(health > maxHealth) { health = maxHealth; }
        if (HealthBar != null) HealthBar.value = health;
    }

    public void ExplosionBlood()
    {
        for(int i = 0; i < ExplosionBloodAmount; i++)
        {
            GameObject _temp = Instantiate(Blood[Random.Range(0, Blood.Length)], this.transform.position, this.transform.rotation);
            _temp.transform.position += new Vector3(Random.Range(bc.size.x / -1.75f, bc.size.x / 1.75f), Random.Range(bc.size.y / -1.75f, bc.size.y / 1.75f), Random.Range(-.1f,.1f));
        }
    }

    void SpawnDrop()
    {
        DifficultyManager _tmp = (DifficultyManager)Resources.Load("difficulty");
        DifficultyLevel difficulty = _tmp.CurrentDifficultyLevel();

        if (Random.Range(0f, 1f) >= 1 - difficulty.lootDropChange)
        {
            List<GameObject> guaranteed = lootDrop.GetGuaranteeedLoot();
            List<GameObject> randomLoot = lootDrop.GetRandomLoot(RadnomLootChange);

            for (int i = 0; i < guaranteed.Count; i++)
            {
                Instantiate(guaranteed[i], new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y, transform.position.z + Random.Range(-1, 1)), Quaternion.identity);
            }

            for (int i = 0; i < randomLoot.Count; i++)
            {
                Instantiate(randomLoot[i], new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y, transform.position.z + Random.Range(-1, 1)), Quaternion.identity);
            }
        }
    }
}

public enum EnemyType
{
    Base = 0, Boss = 1
}
