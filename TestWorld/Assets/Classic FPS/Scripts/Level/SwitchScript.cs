using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchScript : MonoBehaviour
{
    public UnityEvent TurnOff;
    public UnityEvent TurnOn;

    bool isOpen = false;

    private void Start()
    {
        TurnOff.Invoke();
    }

    public void Interaction()
    {
        isOpen = !isOpen;
        if (isOpen) { TurnOn.Invoke(); }
        else { TurnOff.Invoke(); }
    }
}
