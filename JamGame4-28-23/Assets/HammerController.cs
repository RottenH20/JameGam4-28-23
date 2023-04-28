using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    float rotation;
    HingeJoint2D joint;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
    }

    private void Update() {
        rotation = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        joint.motor = new JointMotor2D {
            motorSpeed = rotationSpeed * rotation,
            maxMotorTorque = motorForce,
        };
    }
}
