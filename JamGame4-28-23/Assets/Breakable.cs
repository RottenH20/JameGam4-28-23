using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int health = 1; // Amount of times to hit object before it breaks
    public float minimumVelocity = 3; // Amount of velocity needed to reduce health
    Rigidbody2D rb;

    private void Start()
    {
        rb = GameObject.FindWithTag("Hammer").GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.magnitude < minimumVelocity) // If velocity is not met
        {
            return;
        }

        if (collision.gameObject.tag == "Hammer") // Make sure it was the hammer that touched
        {
            health--;
            if (health == 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
