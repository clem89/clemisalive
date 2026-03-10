using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
    AudioSource _audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        AudioClip[] loaded = Resources.LoadAll<AudioClip>("Sounds");
        foreach (AudioClip clip in loaded)
        {
            _clips[clip.name] = clip;
        }
    }

    public void Play(string soundName)
    {
        if (_clips.TryGetValue(soundName, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Clip not found: {soundName}");
        }
    }
}
