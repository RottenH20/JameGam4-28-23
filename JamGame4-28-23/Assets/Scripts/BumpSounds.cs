using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BumpSounds : MonoBehaviour {

    AudioSource source;
    public float bumpThreshold = 2f;
    public AudioClip[] bumpSounds;

    void Start() {
        source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.relativeVelocity.sqrMagnitude > (bumpThreshold * bumpThreshold)) {
            source.PlayOneShot(RandomExtensions.RandomChoice(bumpSounds));
        }
    }
}
