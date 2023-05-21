using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretScript : MonoBehaviour
{
    [HideInInspector] public int secret_id;

    private void Awake()
    {
        secret_id = Random.Range(0, 999999999);
    }

    private void Start()
    {
        LevelManager.Instance.SecretRoom.Add(this);
        LevelManager.Instance.FindSecret.Add(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.TryFindSecret(this);
        }
    }
}
