using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int health = 1; // Amount of times to hit object before it breaks
    public float minimumVelocity = 3; // Amount of velocity needed to reduce health

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < minimumVelocity && collision.gameObject.tag != "Hammer") // If velocity is not met
        {
            return;
        }

        health--;
        if (health == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
