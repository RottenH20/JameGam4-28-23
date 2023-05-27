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
    AudioSource source;
    public float bumpThreshold = 2f;
    public AudioClip[] bumpSounds;
    public AudioClip[] damageSounds;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        if (RecordManager.instance.CurrentLevel > -1) {
            GameObject hammerPrefab = RecordManager.instance.GetCurrentLevel().hammer;
            if (hammerPrefab != null) {
                Destroy(joint.connectedBody.gameObject);
                GameObject hammer = Instantiate(hammerPrefab, transform.position, Quaternion.identity, transform);
                joint.connectedBody = hammer.GetComponent<Rigidbody2D>();
            }
        }
    }

    private void Update() {
        rotation = -Input.GetAxisRaw("Horizontal")+ Input.GetAxisRaw("HorizontalReverse");

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
        if(joint.connectedBody.transform.localPosition.sqrMagnitude > 2) {
            joint.connectedBody.transform.localPosition = Vector3.zero;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contacts[0].normal.y > 0.33f) {
            animator.SetTrigger("Landing");
        }
        if(collision.relativeVelocity.sqrMagnitude > (bumpThreshold * bumpThreshold)) {
            source.PlayOneShot(RandomExtensions.RandomChoice(bumpSounds));
        }
    }

    public void Damage() {
        animator.SetTrigger("Damage");
        source.PlayOneShot(RandomExtensions.RandomChoice(damageSounds));
    }
}
