using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    [Line("Base")]
    public ZoneType type;
    [Line("Enter")]
    public LocalizedString EnterZoneName;
    public LocalizedString EnterZoneDescription;
    [Line("Exit")]
    public LocalizedString ExitZoneName;
    public LocalizedString ExitZoneDescription;

    GameObject PopupScreen;

    private void Awake()
    {
        PopupScreen = Camera.main.transform.parent.Find("Main Panels").Find("PickUp Panel").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        StopCoroutine(GetPanel());
        PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = EnterZoneDescription.GetLocalization();
        PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = EnterZoneName.GetLocalization();
        Camera.main.transform.parent.Find("Weapons").GetComponent<WeaponSwitch>().SafeZone = true;
        StartCoroutine(GetPanel());
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(GetPanel());
        PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = ExitZoneDescription.GetLocalization();
        PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = ExitZoneName.GetLocalization();
        Camera.main.transform.parent.Find("Weapons").GetComponent<WeaponSwitch>().SafeZone = false;
        StartCoroutine(GetPanel());
    }

    IEnumerator GetPanel()
    {
        PopupScreen.GetComponent<CanvasGroup>().alpha = 1;
        yield return new WaitForSeconds(2);
        PopupScreen.GetComponent<CanvasGroup>().alpha = 0;
    }
}

[System.Serializable]
public enum ZoneType
{
    SafeArea
}
