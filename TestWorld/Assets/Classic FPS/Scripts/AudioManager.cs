using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    public AudioClip[] clips;
    public AudioSource Source;
    int Music_ID = -1;

    private void Awake()
    {
        // Set Static Object
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        StartCoroutine("PlayMusic");
    }
    IEnumerator PlayMusic()
    {
        Music_ID = Random.Range(0, clips.Length);

        Source.clip = clips[Music_ID];
        Source.volume = 1f;
        Source.Play();

        yield return new WaitForSeconds(clips[Music_ID].length);

        StartCoroutine("PlayMusic");
    }
}
