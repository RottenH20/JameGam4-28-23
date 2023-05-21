using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{
    // Privates
    public Vector3 cameraDirection;

    void Update()
    {
        cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(cameraDirection);
    }
}