/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMissile : MonoBehaviour {

    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float speed;
    Transform player;
    int missileLife;
    float timer;

    void Start()
    {
        missileLife = 7;
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {
        transform.LookAt(player);
        timer += Time.deltaTime;
        if (timer > missileLife)
        Destroy(this.gameObject);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("EnemyHit", damage, SendMessageOptions.DontRequireReceiver);
        }
        Destroy(this.gameObject);
    }
}
