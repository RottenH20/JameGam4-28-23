using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableMainMenu : MonoBehaviour
{
    float velocity = 5; // Amount of velocity moving block

    private void FixedUpdate()
    {
        transform.Translate(Vector3.left * velocity * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
