using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float minVelocity = 2;
    public int health = 1; // Amount of times to hit object before it breaks
    // Rewrite velocity check code, doesn't work

    //ParticleSystem particles;
    public GameObject damageEffectPrefab;
    public GameObject destroyEffectPrefab;

    private void Start() {
        //particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hammer" || collision.relativeVelocity.sqrMagnitude < (minVelocity*minVelocity))
        {
            return;
        }

        health--;
        //particles.Emit(10);
        Destroy(Instantiate(damageEffectPrefab, collision.contacts[0].point, Quaternion.identity),2);


        if (health == 0) {
            //particles.Emit(50);
            //particles.transform.parent = null;
            //Destroy(particles.gameObject, 5);

            Destroy(Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity),2);
            Destroy(gameObject);
        }
    }
}
