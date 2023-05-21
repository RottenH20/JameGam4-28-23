using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour
{
    [Line("Popup Panel Settings")]
    public LocalizedString PickUpLocalization;
    public LocalizedString BonusLocalization;

    [Line("Bonus Type")]
    public BonusType bonusType;
    public bool godBonus;

    [Line("Auto Record")]
    public bool AutoRecordInLevelManager = true;


    [Line("Bonus Settings")]
    [ShowIf("bonusType", 0)] public int Health;
    [ShowIf("bonusType", 1)] public int Armor;
    [ShowIf("bonusType", 2)] public AmmoType ammoType;
    [ShowIf("bonusType", 2)] public int AmmoAmount;
    [ShowIf("bonusType", 3)] public KeyType keyType;

    [Line("Score Bonus")]
    [Suffix("Score")] public int ScorePoints = 50;

    [HideInInspector] public int secret_id;

    private void Awake()
    {
        secret_id = Random.Range(0, 999999999);
    }

    public void Start()
    {
        if (AutoRecordInLevelManager)
        {
            LevelManager.Instance.Items.Add(this);
            LevelManager.Instance.FindItems.Add(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.TryFindItem(this);
        }
    }
}

public enum BonusType
{
    Health = 0, Armor = 1, Ammo = 2, Key = 3
}
public enum KeyType
{
    None = 0, RedKey = 1, BlueKey = 2, YellowKey = 3, GreenKey = 4
}
