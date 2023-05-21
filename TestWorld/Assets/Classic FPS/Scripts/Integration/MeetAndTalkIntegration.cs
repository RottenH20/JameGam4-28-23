using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if MEET_AND_TALK
using MEET_AND_TALK;
#endif

public class MeetAndTalkIntegration : MonoBehaviour
{
#if MEET_AND_TALK
    MeetAndTalkSettings meetAndTalk;
    public UnityEvent StartDialog, EndDialog;

    public void Awake()
    {
        meetAndTalk = Resources.Load("MeetAndTalkSettings") as MeetAndTalkSettings;
        //meetAndTalk.DialoguePrefab.GetComponent<>
        GameObject test = Instantiate(meetAndTalk.DialoguePrefab);
        test.transform.Find("Dialogue Manager").GetComponent<DialogueManager>()._manager = Resources.Load<LocalizationManager>("Languages");
        test.transform.Find("Dialogue Manager").GetComponent<DialogueManager>().StartDialogueEvent.AddListener(StartDialog.Invoke);
        test.transform.Find("Dialogue Manager").GetComponent<DialogueManager>().EndDialogueEvent.AddListener(EndDialog.Invoke);
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.paused = false;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.paused = true;
    }
#endif
}
