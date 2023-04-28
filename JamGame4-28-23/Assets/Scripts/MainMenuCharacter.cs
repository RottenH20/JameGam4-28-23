using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    float rotation;
    HingeJoint2D joint;
    GameObject spawnedBreakable;
    GameObject spawnPoint;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
    }

    private void Update()
    {
        rotation = -1;
    }

    private void FixedUpdate()
    {
        joint.motor = new JointMotor2D
        {
            motorSpeed = rotationSpeed * rotation,
            maxMotorTorque = motorForce,
        };
    }
}
