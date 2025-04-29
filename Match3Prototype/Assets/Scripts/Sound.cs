using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0, 256)]
    public int priority = 128;
    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;
    public bool loop;

    // type of sound
    public enum type { music, sfx };
    public type soundType;

    [HideInInspector]
    public AudioSource source;
}