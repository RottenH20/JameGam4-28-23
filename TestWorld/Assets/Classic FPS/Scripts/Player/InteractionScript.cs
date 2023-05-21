using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

#if MEET_AND_TALK
using MEET_AND_TALK;
#endif

public class InteractionScript : MonoBehaviour
{
    [Line("Base Settings")]
    public bool Interactable = true;
    public LocalizedString Name;
    public LocalizedString Description;
    public InteractionItemType ItemType;
    public bool ShowPossibilityOfInteracion = true;
    public bool MultipleUses = false;

    [Line("Base Settings")]
    public GameObject InteractionUIPrefab;
    public Vector3 UIPrefabRotation;
    public Vector3 UIPrefabPosition;

    [Line("Unity Event")]
    public UnityEvent WhenInteraction;

    public GameObject UICanvas;
    int _state;

    private void Awake()
    {
        UICanvas = Instantiate(InteractionUIPrefab, this.transform);

        UICanvas.transform.localPosition = UIPrefabPosition;
        UICanvas.transform.Rotate(UIPrefabRotation.x - transform.rotation.x, UIPrefabRotation.y - transform.rotation.y, UIPrefabRotation.z - transform.rotation.z, Space.World);

        UICanvas.transform.Find("Name_Txt").GetComponent<TMP_Text>().text = Name.GetLocalization();
        UICanvas.transform.Find("Description_Txt").GetComponent<TMP_Text>().text = Description.GetLocalization();

        UICanvas.AddComponent<FaceCamera>();
    }

#if MEET_AND_TALK
    public void StartDialogue(DialogueContainerSO dialog)
    {
        this.Test("Start Dialog");
        DialogueManager.Instance.StartDialogue(dialog);
    }
#endif

    public void Interaction()
    {
        if (Interactable)
        {
            WhenInteraction.Invoke();
            if (!MultipleUses) { Interactable = false; }
        }
    }

    public void OnDetection()
    {
        _state = 1;
    }

    public void OnSelected()
    {
        _state = 2;
    }

    public void Update()
    {
        // Rotate
        Vector3 cameraDirection;
        cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0;
        UICanvas.transform.rotation = Quaternion.LookRotation(cameraDirection);

        // UI
        if(_state == 1 && Interactable && ShowPossibilityOfInteracion) // Detection
        {
            UICanvas.gameObject.SetActive(true);
            UICanvas.transform.Find("Name_Txt").GetComponent<TMP_Text>().text = "";
            UICanvas.transform.Find("Description_Txt").GetComponent<TMP_Text>().text = "";
            UICanvas.transform.Find("Background_Normal").GetComponent<Transform>().gameObject.SetActive(false);
            UICanvas.transform.Find("Background_Key").GetComponent<Transform>().gameObject.SetActive(false);
        }
        else if(_state == 2 && Interactable) // Selected
        {
            UICanvas.gameObject.SetActive(true);
            UICanvas.transform.Find("Name_Txt").GetComponent<TMP_Text>().text = Name.GetLocalization();
            UICanvas.transform.Find("Description_Txt").GetComponent<TMP_Text>().text = Description.GetLocalization();
            if (ItemType == InteractionItemType.Normal) UICanvas.transform.Find("Background_Normal").GetComponent<Transform>().gameObject.SetActive(true);
            if (ItemType == InteractionItemType.KeyItem) UICanvas.transform.Find("Background_Key").GetComponent<Transform>().gameObject.SetActive(true);
        }
        else
        {
            UICanvas.gameObject.SetActive(false);
            UICanvas.transform.Find("Name_Txt").GetComponent<TMP_Text>().text = "";
            UICanvas.transform.Find("Description_Txt").GetComponent<TMP_Text>().text = "";
            UICanvas.transform.Find("Background_Normal").GetComponent<Transform>().gameObject.SetActive(false);
            UICanvas.transform.Find("Background_Key").GetComponent<Transform>().gameObject.SetActive(false);
        }
        _state = 0;
    }
}

[System.Serializable]
public enum InteractionItemType
{
    Normal = 0, KeyItem = 1
}
