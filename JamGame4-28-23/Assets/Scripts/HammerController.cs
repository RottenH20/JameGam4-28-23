using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    public float fallThreshold = 3;
    public float flyThreshold = 2;
    float rotation;
    HingeJoint2D joint;
    Animator animator;
    Rigidbody2D rigid;
    Transform hammerParent;
    AudioSource source;
    public AudioClip bumpSounds;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        hammerParent = transform.GetChild(0);
        source = GetComponent<AudioSource>();
    }

    private void Update() {
        rotation = -Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.Alpha1)) {
            for(int i = 0;i < hammerParent.childCount; i++) {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 1);
            }
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            for (int i = 0; i < hammerParent.childCount; i++) {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 2);
            }
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            for (int i = 0; i < hammerParent.childCount; i++) {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 3);
            }
        }
    }

    private void FixedUpdate() {
        joint.motor = new JointMotor2D {
            motorSpeed = rotationSpeed * rotation,
            maxMotorTorque = motorForce,
        };
        
        if(rigid.velocity.y < -fallThreshold) {
            animator.SetBool("Falling", true);
            animator.SetBool("Left", false);
            animator.SetBool("Right", false);
        } else {
            animator.SetBool("Falling", false);
            if(Mathf.Abs(rigid.velocity.x) > flyThreshold) {
                if(rigid.velocity.x > 0) {
                    animator.SetBool("Right", true);
                    animator.SetBool("Left", false);
                } else {
                    animator.SetBool("Right", false);
                    animator.SetBool("Left", true);
                }
            } else {
                animator.SetBool("Right", false);
                animator.SetBool("Left", false);
            }
        }

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
