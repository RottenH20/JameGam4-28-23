/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.1.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Line("Health")]
    public int godHealth;
    public int maxHealth;
    public int startHealth;

    [Line("Armor")]
    public int godArmor;
    public int maxArmor;
    public int startArmor;

    [Line("Audio")]
    public AudioClip hit;
    public AudioClip deathClip;

    [Line("Prefabs & Objects")]
    public FlashScreen flash;

    [Line("Text")]
    public TMP_Text HealthText;
    public TMP_Text ArmorText;

    [Line("Progress Bar")]
    public Slider HealthBar;
    public Slider OverHealthBar;
    public Slider ArmorBar;

    [Line("Damage Indicator")]
    public GameObject DamageIndicatorPrefab;
    public Transform IndicatorParent;
    public float IndicatorShowTime;

    [HideInInspector] public List<Transform> IndicatorPosition;
    [HideInInspector] public List<float> IndicatorTimeContainer;

    [Line("Face")]
    public Sprite Face_200;
    public Sprite Face_150, Face_100, Face_90, Face_75, Face_40, Face_25, Face_10;
    public Image FaceImage;

    [Line("Private Value")]
    [HideInInspector] public float armor;
    [HideInInspector] public float health;

    [HideInInspector] public float WeaponBlockingChange;
    [HideInInspector] public float WeaponDamageReductionPercentage;
    AudioSource source;
    bool armorRegen = false;
    bool healthRegen = false;
    GameSettingsManger _go;

    void Start()
    {
        _go = (GameSettingsManger)Resources.Load("Game_Settings");
        if (_go.EnablePlayerAbility)
        {
            PassiveAbilitySO _passive = GameManager.Instance.PassiveSkill;
            if (_passive.BoostHealth)
            {
                maxHealth += (int)_passive.AdditionalHealth;
                godHealth += (int)_passive.AdditionalHealth;
                startHealth += (int)_passive.AdditionalStartHealth;
            }
            if (_passive.BoostArmor)
            {
                maxArmor += (int)_passive.AdditionalArmor;
                godArmor += (int)_passive.AdditionalArmor;
                startArmor += (int)_passive.AdditionalStartArmor;
            }
            if (_passive.HealthRegeneration) { healthRegen = true; }
            if (_passive.ArmorRegeneration) { armorRegen = true; }
        }

        armor = startArmor;
        health = startHealth;
        source = GetComponent<AudioSource>();

        HealthText.text = health.ToString();
        ArmorText.text = armor.ToString();
    }

    private void Update()
    {
        armor = Mathf.Clamp(armor, 0, godArmor);
        health = Mathf.Clamp(health, -Mathf.Infinity, godHealth);

        if(health <= 0)
        {
            source.PlayOneShot(deathClip);
            GameManager.Instance.PlayerDeath();
        }

        foreach (Transform child in IndicatorParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (_go.EnablePlayerDamageIndicator)
        {
            for (int i = 0; i < IndicatorPosition.Count; i++)
            {
                GameObject _prefab = Instantiate(DamageIndicatorPrefab, IndicatorParent);
                _prefab.SetActive(true);

                // Posiotion
                float AngleRad = Mathf.Atan2(IndicatorPosition[i].position.x - transform.position.x, IndicatorPosition[i].position.z - transform.position.z);
                float AngleDeg = (180 / Mathf.PI) * AngleRad;
                if (transform.rotation.eulerAngles.y > 0) { _prefab.transform.rotation = Quaternion.Euler(0, 0, (AngleDeg - transform.rotation.eulerAngles.y) * -1); }
                else if (transform.rotation.eulerAngles.y < 0) { _prefab.transform.rotation = Quaternion.Euler(0, 0, (AngleDeg + transform.rotation.eulerAngles.y) * -1); }

                IndicatorTimeContainer[i] -= Time.deltaTime;

                // Destroy Time
                if (IndicatorTimeContainer[i] < 0)
                {
                    IndicatorTimeContainer.RemoveAt(i);
                    IndicatorPosition.RemoveAt(i);
                }
            }
        }
    }

    public void AddHealth(float value, bool GodBonus)
    {
        if (!GodBonus)
        {
            if (health > maxHealth) { }
            else if(health + value <= maxHealth) 
            {
                health += value;
            }
            else
            {
                health = maxHealth;
            }
        }
        else
        {
            if (health + value <= godHealth)
            {
                health += value;
            }
            else
            {
                health = godHealth;
            }
        }
        HealthText.text = Mathf.Ceil(health).ToString();
        HealthBar.value = health;
        OverHealthBar.value = health - 100;
        FaceSprite();
    }

    public void AddArmor(float value, bool GodBonus)
    {
        if (!GodBonus)
        {
            if (armor > maxArmor) {  }
            else if (armor + value <= maxArmor)
            {
                armor += value;
            }
            else
            {
                armor = maxArmor;
            }
        }
        else
        {
            if (armor + value <= godArmor)
            {
                armor += value;
            }
            else
            {
                armor = godArmor;
            }
        }
        ArmorText.text = Mathf.Ceil(armor).ToString();
        ArmorBar.value = armor;
        FaceSprite();
    }

    public void TakeDamage(float damage)
    {
        // Reduce Damage if Enable
        if(Random.Range(0,100) < WeaponBlockingChange)
        {
            damage -= damage * (WeaponDamageReductionPercentage / 100);
        }

        // Take Damage
        if (armor > 0 && armor >= damage)
        {
            armor -= damage;
        } else if (armor > 0 && armor < damage)
        {
            damage -= armor;
            armor = 0;
            health -= damage;
        } else
        {
            health -= damage;
        }

        HealthText.text = Mathf.Ceil(health).ToString();
        ArmorText.text = Mathf.Ceil(armor).ToString();

        source.PlayOneShot(hit);        
        flash.TookDamage();
        FaceSprite();

        // Only if Regeneration
        if(armorRegen || healthRegen) StopAllCoroutines();
        if (healthRegen) StartCoroutine(HealthRegenCooldown());
        if (armorRegen) StartCoroutine(ArmorRegenCooldown());
    }

    void FaceSprite()
    {

        if (health >= maxHealth + ((godHealth - maxHealth) / 2)) { FaceImage.sprite = Face_200; }
        if (health <= maxHealth + ((godHealth - maxHealth) / 2) && health > maxHealth) { FaceImage.sprite = Face_150; }
        if (health <= maxHealth && health > 90) { FaceImage.sprite = Face_100; }
        if (health <= 90 && health > 75) { FaceImage.sprite = Face_90; }
        if (health <= 75 && health > 40) { FaceImage.sprite = Face_75; }
        if (health <= 40 && health > 25) { FaceImage.sprite = Face_40; }
        if (health <= 25 && health > 10) { FaceImage.sprite = Face_25; }
        if (health <= 10) { FaceImage.sprite = Face_10; }
    }

    // Health Regen
    IEnumerator HealthRegenCooldown()
    {
        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveAbilitySO selectedPassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        yield return new WaitForSeconds(selectedPassiveSkill.HealthRegenerationWaitingTime);
        StartCoroutine(HealthRegen());
    }
    IEnumerator HealthRegen()
    {
        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveAbilitySO selectedPassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        while (true)
        {
            AddHealth(selectedPassiveSkill.HealthRegenerationAmount, false);
            if (health == maxHealth) { StopCoroutine(HealthRegen()); }
            yield return new WaitForSeconds(selectedPassiveSkill.HealthRegenerationTimeInterval);
        }
    }

    // Armor Regen
    IEnumerator ArmorRegenCooldown()
    {
        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveAbilitySO selectedPassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        yield return new WaitForSeconds(selectedPassiveSkill.ArmorRegenerationWaitingTime);
        StartCoroutine(ArmorRegen());
    }
    IEnumerator ArmorRegen()
    {
        AbilityManager am = (AbilityManager)Resources.Load("Ability");
        PassiveAbilitySO selectedPassiveSkill = am.passiveAbilityList[TinySaveSystem.GetInt("selectedPassiveSkill")].skill;
        while (true)
        {
            AddArmor(selectedPassiveSkill.ArmorRegenerationAmount, false);
            if(armor == maxArmor) { StopCoroutine(ArmorRegen()); }
            yield return new WaitForSeconds(selectedPassiveSkill.ArmorRegenerationTimeInterval);
        }
    }

    public void DamageIndicatorFunction(Transform source)
    {
        IndicatorPosition.Add(source);
        IndicatorTimeContainer.Add(IndicatorShowTime);
    }
}