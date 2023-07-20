using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AudioSource_e
{
    AUDIO_SOURCE_PLAYER,
    AUDIO_SOURCE_ENEMY,
    AUDIO_SOURCE_BACKGROUND_MUSIC
}

public class AudioPlayer : MonoBehaviour
{
    private const float AUDIO_VOLUME_MIN = 0f;
    private const float AUDIO_VOLUME_MAX = 1f;

    [Header("Player Sound Configs")]
    [SerializeField] [Range(AUDIO_VOLUME_MIN, AUDIO_VOLUME_MAX)] private float playerVolume;

    [Header("Enemy Sound Configs")]
    [SerializeField][Range(AUDIO_VOLUME_MIN, AUDIO_VOLUME_MAX)] private float enemyVolume;

    [Header("Background Music Configs")]
    [SerializeField] private AudioClip actionMusic;
    [SerializeField][Range(AUDIO_VOLUME_MIN, AUDIO_VOLUME_MAX)] private float backgroundMusicVolume;

    private AudioSource audioSourceBackground;
    static private AudioPlayer audioPlayerSingletonInstance; // This will store reference to current object for singleton implementation


    public void PlaySound(AudioClip audioClip, AudioSource_e audioSource)
    {
        float volume = AUDIO_VOLUME_MIN;
        Vector3 position = Camera.main.transform.position; // TODO: for now, audio position will be the same for all sources

        switch(audioSource) {
            case AudioSource_e.AUDIO_SOURCE_PLAYER:
                volume = playerVolume;
                break;
            case AudioSource_e.AUDIO_SOURCE_ENEMY:
                volume = enemyVolume;
                break;
            case AudioSource_e.AUDIO_SOURCE_BACKGROUND_MUSIC:
                volume = backgroundMusicVolume;
                break;
            default:
                Debug.Log("No audio source provided.");
                break;
        }

        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if (audioPlayerSingletonInstance == null)
        {
            // first instance of this game object in the main menu
            // Save this one so that it persists on future scenes
            audioPlayerSingletonInstance = this;
            DontDestroyOnLoad(audioPlayerSingletonInstance);
        }
        else
        {
            // We've already createed an audio player instance, so destroy the new game object.
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // init
        audioSourceBackground = GetComponent<AudioSource>();

        // TODO: For now, I will just have the action music playing constantly.
        // Once I have a starting screen, I will fade between the calm->action music once the gameplay loop begins.
        audioSourceBackground.clip = actionMusic;
        audioSourceBackground.volume = backgroundMusicVolume;
        audioSourceBackground.Play();
    }
}
