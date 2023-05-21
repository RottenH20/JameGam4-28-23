using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapScript : MonoBehaviour
{
    [Line("Settings")]
    public TriggerType triggerType;

    [Line("Damage Settings")]
    public float Damage;

    [Line("Timers")]
    public float delayTime = 0.2f;

    [Line("Transform")]
    public List<GameObject> ObjectToMove;
    public Vector3 StartPosition, EndPosition;

    [Line("Unity Events")]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate, OnCountdown;

    private float timer;
    public List<GameObject> targets;
    private bool activate = false;

    private void Start()
    {
        for(int i =0; i < ObjectToMove.Count; i++) { ObjectToMove[i].transform.localPosition = StartPosition; }
    }

    public void Update()
    {
        timer -= Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.Automatic && activate == false) { StartCoroutine(Activating(delayTime)); }
        targets.Add(other.gameObject);
    }
    public void OnTriggerExit(Collider other)
    {
        targets.Remove(other.gameObject);
    }

    IEnumerator Activating(float time)
    {
        OnCountdown.Invoke();
        activate = true;
        yield return new WaitForSeconds(time);
        for (int i = 0; i < targets.Count; i++) { targets[i].SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver); }
        OnActivate.Invoke();
    }
    IEnumerator Deactivating(float time)
    {
        yield return new WaitForSeconds(time);
        OnDeactivate.Invoke(); activate = false;
    }


    public void MoveToStartPosition()
    {
        for (int i = 0; i < ObjectToMove.Count; i++) { ObjectToMove[i].transform.localPosition = StartPosition; }
    }
    public void MoveToEndPosition()
    {
        for (int i = 0; i < ObjectToMove.Count; i++) { ObjectToMove[i].transform.localPosition = EndPosition; }
    }
    public void StartDeactivate(float time)
    {
        StartCoroutine(Deactivating(time));
    }

    public void ManualActivate()
    {
        if(triggerType == TriggerType.Manual)
        {
            StartCoroutine(Activating(delayTime));
        }
    }

    public enum TriggerType
    {
        Manual, Automatic
    }
}

