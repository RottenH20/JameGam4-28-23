using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    float rotation;
    HingeJoint2D joint;
    Animator animator;
    Rigidbody2D rigid;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        rotation = -Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        joint.motor = new JointMotor2D {
            motorSpeed = rotationSpeed * rotation,
            maxMotorTorque = motorForce,
        };
        animator.SetFloat("yVelocity", rigid.velocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contacts[0].normal.y > 0.33f) {
            animator.SetTrigger("Landing");
        }
    }

    public void Damage() {
        animator.SetTrigger("Damage");
    }
}
