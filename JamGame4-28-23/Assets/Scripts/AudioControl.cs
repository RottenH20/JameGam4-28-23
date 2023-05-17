using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioControl : MonoBehaviour
{
    private AudioSource[] _audioSources;
    public float fadeSpeed = 0.5f;
    public bool isPlaying;

    public string playingTrackName = "Nothing";
    public int playingTrackIndex;
    public float playingTrackVolume = 0.000f;

    public string lastTrackName = "Nothing";
    public int lastTrackIndex;
    public float lastTrackVolume = 0.000f;

    public static AudioControl instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            _audioSources = GetComponentsInChildren<AudioSource>();
            PlayMusic("MainMenuMusic");
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (instance == this) {
            if (SceneManager.GetActiveScene().name == "Intro" || SceneManager.GetActiveScene().name == "Main Menu") {
                PlayMusic("MainMenuMusic");
            } else {
                PlayMusic(RecordManager.instance.GetCurrentLevelMusic());
            }
        }
    }

    public IEnumerator FadeOutOldMusic_FadeInNewMusic()
    {
        _audioSources[playingTrackIndex].volume = 0.000f;
        _audioSources[playingTrackIndex].Play();
        while (_audioSources[playingTrackIndex].volume < .4f)
        {
            _audioSources[lastTrackIndex].volume -= fadeSpeed;
            _audioSources[playingTrackIndex].volume += fadeSpeed;
            //Debug.Log("Fade: " + lastTrackName + " " + _audioSources[lastTrackIndex].volume.ToString() + " Rise: " + playingTrackName + " " + _audioSources[playingTrackIndex].volume.ToString());
            yield return new WaitForSeconds(0.001f);
            lastTrackVolume = _audioSources[lastTrackIndex].volume;
            playingTrackVolume = _audioSources[playingTrackIndex].volume;
        }
        _audioSources[lastTrackIndex].volume = 0.000f; // Just In Case....
        _audioSources[lastTrackIndex].Stop();

        lastTrackIndex = playingTrackIndex;
        lastTrackName = playingTrackName;
        isPlaying = true;
    }

    public IEnumerator FadeInNewMusic()
    {
        _audioSources[playingTrackIndex].volume = 0.000f;
        _audioSources[playingTrackIndex].Play();
        while (_audioSources[playingTrackIndex].volume < .4f)
        {
            _audioSources[playingTrackIndex].volume += fadeSpeed;
            //Debug.Log("Fading In: " + _audioSources[track_index].volume.ToString());
            yield return new WaitForSeconds(0.001f);
            playingTrackVolume = _audioSources[playingTrackIndex].volume;
        }
        lastTrackIndex = playingTrackIndex;
        lastTrackName = playingTrackName;
        isPlaying = true;
    }

    public void PlayMusic(string transformName)
    {
        if (playingTrackName == transformName)
            return;
        for (int a = 0; a < _audioSources.Length; a++)
        {
            if (_audioSources[a].name == transformName)
            {
                playingTrackIndex = a;
                playingTrackName = transformName;
                break;
            }
        }
        if (isPlaying)
        {
            Debug.Log("Fading in new music - Fading out old music");
            StartCoroutine(FadeOutOldMusic_FadeInNewMusic());
        }
        else
        {
            Debug.Log("Fading in new music");
            StartCoroutine(FadeInNewMusic());
        }
    }
}