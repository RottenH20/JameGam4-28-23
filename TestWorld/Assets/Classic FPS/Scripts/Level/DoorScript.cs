/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.2.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour 
{
    [Line("Base Settings")]
    public DoorType type;
    public DoorTrigger trigger;
    public KeyType keyType;
    public float speed;
    public Vector3 endPosition;

    [Line("Door Material")]
    public Material Base;
    public Material Red, Blue, Yellow, Green;

    // Privates
    Vector3 startPosition;
    GameObject doors;
    bool isOpen = false;
    bool isMoving = false;
    Animator anim;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        doors = this.transform.Find("Door").gameObject;
        startPosition = doors.transform.localPosition;
        anim = doors.GetComponent<Animator>();
        doors.GetComponent<MeshRenderer>().material = GetMaterial();
        if(trigger != DoorTrigger.Interaction)
        {
            if(this.transform.TryGetComponent<InteractionScript>(out InteractionScript interaction))
            {
                interaction.Interactable = false;
            }
        }
    }

    private void Update()
    {
        if (isOpen) { doors.transform.localPosition = Vector3.SmoothDamp(doors.transform.localPosition, (startPosition + endPosition), ref velocity, speed / Time.deltaTime); }
        else { doors.transform.localPosition = Vector3.SmoothDamp(doors.transform.localPosition, startPosition, ref velocity, speed / Time.deltaTime); }
    }

    public void Interaction()
    {
        if (trigger == DoorTrigger.Interaction && CheckKey())
        {
            if (type == DoorType.Slide)
            {
                isOpen = !isOpen; velocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && trigger == DoorTrigger.Automatic && CheckKey())
        {
            if (type == DoorType.Slide)
            {
                isOpen = true; velocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && trigger == DoorTrigger.Automatic && CheckKey())
        {
            if (type == DoorType.Slide)
            {
                isOpen = false; velocity = Vector3.zero;
            }
        }
    }

    public bool CheckKey()
    {
        switch (keyType)
        {
            case KeyType.RedKey:
                if(LevelManager.Instance.redKeys == true) { return true; }
                break;
            case KeyType.BlueKey:
                if (LevelManager.Instance.blueKeys == true) { return true; }
                break;
            case KeyType.YellowKey:
                if (LevelManager.Instance.yellowKeys == true) { return true; }
                break;
            case KeyType.GreenKey:
                if (LevelManager.Instance.greenKeys == true) { return true; }
                break;
            case KeyType.None:
                return true;
            default:
                return false;

        }

        return false;
    } 

    Material GetMaterial()
    {
        switch (keyType)
        {
            case KeyType.RedKey:
                return Red;
            case KeyType.BlueKey:
                return Blue;
            case KeyType.YellowKey:
                return Yellow;
            case KeyType.GreenKey:
                return Green;
            default:
                return Base;
        }
    }

    public void MoveDoor()
    {
        isOpen = !isOpen; velocity = Vector3.zero;
    }
    public void CloseDoor()
    {
        isOpen = false; velocity = Vector3.zero;
    }
    public void OpenDoor()
    {
        isOpen = true; velocity = Vector3.zero;
    }
}

public enum DoorType
{
    Slide = 0
}
public enum DoorTrigger
{
    None = -1, Interaction = 0, Automatic = 1
}
