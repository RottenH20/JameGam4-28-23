/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.1.0a
    Updated in Version: 1.1.0a
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    public float damage;
    public float interval;

    // Privates
    Collider coll;

    private void Awake()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine("TryGetDamge", other);
    }
    private void OnTriggerExit(Collider other)
    {
        StopCoroutine("TryGetDamge");
    }

    IEnumerator TryGetDamge(Collider other)
    {
        DifficultyManager _tmp = (DifficultyManager)Resources.Load("difficulty");
        DifficultyLevel difficulty = _tmp.CurrentDifficultyLevel();

        while (true)
        {
            other.SendMessage("TakeDamage", damage *= difficulty.areaDamageMultiplier, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(interval);
        }
    }
}
