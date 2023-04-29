using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableMainMenu : MonoBehaviour
{
    float velocity = 5; // Amount of velocity moving block
    public float rotationSpeed = 10f;

    private int randomIndex;
    public bool right;
    public enum Direction
    {
        Left,
        Right
    }

    private void FixedUpdate()
    {
        if (right)
        {
            transform.position += new Vector3(velocity * Time.deltaTime, 0, 0);
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            transform.position += new Vector3(-velocity * Time.deltaTime, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
