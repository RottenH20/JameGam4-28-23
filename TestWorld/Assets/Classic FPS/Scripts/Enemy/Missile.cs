/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Transform source;


    // Privates
    Transform player;
    int missileLife;
    float timer;

    void Start()
    {
        missileLife = 15;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > missileLife) Destroy(this.gameObject);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        other.SendMessage("DamageIndicatorFunction", source, SendMessageOptions.DontRequireReceiver);
        Destroy(this.gameObject);
    }
}