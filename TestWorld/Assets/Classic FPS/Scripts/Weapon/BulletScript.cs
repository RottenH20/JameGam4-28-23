/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.3.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class BulletScript : MonoBehaviour 
{
    public BulleType type;

    // Normal Bullet
    [HideInInspector] public float damage;

    // Explosion Bullet
    [HideInInspector] public float radius;
    [HideInInspector] public LayerMask layerMask;
    [HideInInspector] public GameObject explosion;
    [HideInInspector] public AudioClip explosionSound;
    [HideInInspector] public AnimationCurve damageCurve;
    [HideInInspector] public float timeBeforeExplosion = -1f;
    [HideInInspector] public float shakeRadius;

    // Static
    float bulletLife;
    float destroyAfter = 10;
    float timer = 0;

    private void Update()
    {
        bulletLife += Time.deltaTime;
        if (bulletLife > destroyAfter) Destroy(this.gameObject);

        if (type == BulleType.Explosion && timeBeforeExplosion > 0)
        {
            timer += Time.deltaTime;
            if (timer > timeBeforeExplosion)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);
                GameObject explosionInstantiated = (GameObject)Instantiate(explosion, transform.position, Quaternion.identity);
                explosionInstantiated.GetComponent<Explosion>().explosionSound = explosionSound;

                foreach (Collider col in hitColliders)
                {
                    float _distance = Vector3.Distance(col.transform.position, this.transform.position);

                    col.SendMessage("TakeDamage", damage * damageCurve.Evaluate(_distance / radius), SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ExplosionBlood", SendMessageOptions.DontRequireReceiver);
                }

                if (Vector3.Distance(this.transform.position, Camera.main.transform.position) < shakeRadius) { Camera.main.GetComponent<CameraShake>().ShakeCamera(); }

                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (timeBeforeExplosion <= 0)
        {
            if (type == BulleType.Normal)
            {
                ContactPoint contact = collision.contacts[0];
                Collider[] hitColliders = Physics.OverlapSphere(contact.point, .01f, layerMask);
                foreach (Collider col in hitColliders)
                {
                    col.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

                    Enemy _enemy;
                    if (col.transform.TryGetComponent<Enemy>(out _enemy))
                    {
                        Instantiate(_enemy.Blood[Random.Range(0, _enemy.Blood.Length)], transform.position, Quaternion.identity);
                    }
                }
            }

            if (type == BulleType.Explosion && timeBeforeExplosion <= 0)
            {
                ContactPoint contact = collision.contacts[0];
                Collider[] hitColliders = Physics.OverlapSphere(contact.point, radius, layerMask);
                GameObject explosionInstantiated = (GameObject)Instantiate(explosion, contact.point, Quaternion.identity);
                explosionInstantiated.GetComponent<Explosion>().explosionSound = explosionSound;

                foreach (Collider col in hitColliders)
                {
                    float _distance = Vector3.Distance(col.transform.position, this.transform.position);

                    col.SendMessage("TakeDamage", (damage * damageCurve.Evaluate(_distance / radius)), SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ExplosionBlood", SendMessageOptions.DontRequireReceiver);
                }

                if (Vector3.Distance(this.transform.position, Camera.main.transform.position) < shakeRadius) { Camera.main.GetComponent<CameraShake>().ShakeCamera(); }
            }
            Destroy(this.gameObject);
        }
    }
}
