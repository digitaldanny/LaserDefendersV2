using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AudioSource_e
{
    AUDIO_SOURCE_PLAYER,
    AUDIO_SOURCE_ENEMY
}

public class AudioPlayer : MonoBehaviour
{
    private const float AUDIO_VOLUME_MIN = 0f;
    private const float AUDIO_VOLUME_MAX = 1f;

    [Header("Player Sound Configs")]
    [SerializeField] [Range(AUDIO_VOLUME_MIN, AUDIO_VOLUME_MAX)] private float playerVolume;

    [Header("Enemy Sound Configs")]
    [SerializeField][Range(AUDIO_VOLUME_MIN, AUDIO_VOLUME_MAX)] private float enemyVolume;

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
            default:
                Debug.Log("No audio source provided.");
                break;
        }

        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
}
