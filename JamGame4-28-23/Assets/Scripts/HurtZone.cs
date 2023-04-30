using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
    LevelManager levelManager;
    public float force = 100;
    // Depending on what we want the HurtZone to do
    // Ex. Add time, Restart game, Move back to last safe point

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Hurt!");
            levelManager.AddPenalty(5);
            Rigidbody2D rigid = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rigid != null) {
                //rigid.AddForce((rigid.position-collision.contacts[0].point).normalized * force,ForceMode2D.Impulse);
                rigid.AddForce(Vector2.up * force,ForceMode2D.Impulse);
            }
        }
    }
}
