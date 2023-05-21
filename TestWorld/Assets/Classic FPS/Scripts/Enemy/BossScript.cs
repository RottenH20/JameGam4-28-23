using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    [Line("Localization")]
    public LocalizedString BossLocalization;

    [Line("Boss Elements")]
    public Enemy Boss;
    public Slider bossBar;

    private StringLocalization Localization;
    private string BossNameKey = "Boss_Name_Key";

    private void Start()
    {
        GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false);
        bossBar = GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI/Boss_Healthbar").GetComponent<Slider>();
        bossBar.maxValue = Boss.maxHealth;
    }

    private void Update()
    {
        bossBar.value = Boss.health;
        if (Boss.health <= 0) { GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false); }

    }

    // SHow Hide UI
    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false);
        GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI/Boss_Name_Txt").GetComponent<TMPro.TMP_Text>().text = Localization.GetString(BossNameKey);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Boss.health > 0) GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(true);
        else { GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false); }
    }
    private void OnTriggerStay(Collider other)
    {
        if (Boss.health > 0) GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(true);
        else { GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false); }
    }

    public void DestroyObject()
    {
        GameManager.Instance.transform.parent.Find("Canvas/UI/Boss_UI").gameObject.SetActive(false);
        Destroy(transform.gameObject);
    }
}
