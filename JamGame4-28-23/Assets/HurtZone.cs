using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
    // Depending on what we want the HurtZone to do
    // Ex. Add time, Restart game, Move back to last safe point

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hurt!");
        }
    }
}
