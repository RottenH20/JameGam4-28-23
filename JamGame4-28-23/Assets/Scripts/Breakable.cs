using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int health = 1; // Amount of times to hit object before it breaks
    // Rewrite velocity check code, doesn't work

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hammer") // If velocity is not met
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
