/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.1.0a
    Updated in Version: 1.1.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionBarrel : MonoBehaviour
{
    public Sprite Barrel;
    public Sprite BarrelDestroyed;

    public GameObject Explosion;
    public GameObject ExplosionPrefab;
    public float damage;
    public AnimationCurve damageCurve;
    public float radius;
    public float explosionShakeDistance;
    public AudioClip ExplosionSound;
    public LayerMask explosionLayerMask;

    bool exploded;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = Barrel;
    }

    void TakeDamage(float dmg)
    {
        if (!exploded)
        {
            GetComponent<SpriteRenderer>().sprite = BarrelDestroyed;

            Explosion.SetActive(true);
            Explosion.GetComponent<BulletScript>().type = BulleType.Explosion;
            Explosion.GetComponent<BulletScript>().damage = damage;
            Explosion.GetComponent<BulletScript>().damageCurve = damageCurve;
            Explosion.GetComponent<BulletScript>().radius = radius;
            Explosion.GetComponent<BulletScript>().explosionSound = ExplosionSound;
            Explosion.GetComponent<BulletScript>().layerMask = explosionLayerMask;
            Explosion.GetComponent<BulletScript>().explosion = ExplosionPrefab;
            Explosion.GetComponent<BulletScript>().shakeRadius = explosionShakeDistance;
            exploded = true;
        }
    }
}
