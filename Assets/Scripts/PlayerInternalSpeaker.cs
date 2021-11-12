using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInternalSpeaker : MonoBehaviour
{
    AudioSource speaker;

    [SerializeField]
    AudioClip[] mumbles;

    bool doMumble = false;

    private static PlayerInternalSpeaker instance { get; set; }

    public static AudioSource Speaker
    {
        get
        {
            instance.doMumble = false;
            return instance.speaker;
        }
    }

    public static void Mumble(float duration)
    {
        instance._Mumble(duration);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        speaker = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void _Mumble(float duration)
    {
        doMumble = true;
        StartCoroutine(_EmitMumbles(duration));
    }

    [SerializeField, Range(0, 1)]
    float minVolumeScale = 0.5f;
    [SerializeField, Range(0, 2)]
    float minWaitForNextMumbleScale = 0.7f;
    [SerializeField, Range(0, 2)]
    float maxWaitForNextMumbleScale = 1.4f;
    
    private IEnumerator<WaitForSeconds> _EmitMumbles(float duration)
    {
        float startTime = Time.timeSinceLevelLoad;
        float progress = 0;
        while (doMumble && progress < duration)
        {
            var clip = mumbles[Random.Range(0, mumbles.Length)];
            speaker.PlayOneShot(clip, Random.value * (1 - minVolumeScale) + minVolumeScale);
            yield return new WaitForSeconds(clip.length * Random.Range(minWaitForNextMumbleScale, maxWaitForNextMumbleScale));
            progress = Time.timeSinceLevelLoad - startTime;
        }
    }
}
