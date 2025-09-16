using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using System.Threading;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    public bool sfxMuted;
    public bool musicMuted;
    public float masterVolume;

    public Toggle sfxToggle;
    public Toggle musicToggle;
    public Slider masterVolumeSlider;

    void Awake()
    {

        if (instance != null && instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.priority = s.priority;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //if (SceneManager.GetActiveScene().name == "Title Screen" || SceneManager.GetActiveScene().name == "Start Screen")
        //{
        //    Play("menu_music");
        //}
        //else if (SceneManager.GetActiveScene().name == "Game Screen")
        //{
        //    Play("game_music");
        //}
    }

    private void Update()
    {
        //if (musicToggle == null)
        //{
        //    if (GameObject.FindGameObjectWithTag("music toggle") != null)
        //    {
        //        musicToggle = GameObject.FindGameObjectWithTag("music toggle").GetComponent<UnityEngine.UI.Toggle>();
        //        musicToggle.isOn = !musicMuted;
        //        musicToggle.onValueChanged.AddListener(delegate
        //        {
        //            muteMusic(!musicToggle.isOn);
        //        });
        //        GameObject.FindGameObjectWithTag("music toggle").GetComponent<AudioToggle>().toggleSetting(!musicMuted);

        //    }
        //}

        //if (sfxToggle == null)
        //{
        //    if (GameObject.FindGameObjectWithTag("sfx toggle") != null)
        //    {
        //        sfxToggle = GameObject.FindGameObjectWithTag("sfx toggle").GetComponent<UnityEngine.UI.Toggle>();
        //        sfxToggle.isOn = !sfxMuted;
        //        sfxToggle.onValueChanged.AddListener(delegate
        //        {
        //            muteSFX(!sfxToggle.isOn);
        //        });
        //        GameObject.FindGameObjectWithTag("sfx toggle").GetComponent<AudioToggle>().toggleSetting(!sfxMuted);
        //    }
        //}

        if (SceneManager.GetActiveScene().name == "GameScreen")
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider = GameObject.FindGameObjectWithTag("master volume").GetComponent<Slider>();
                masterVolumeSlider.value = masterVolume;
                AudioListener.volume = masterVolume;
            }

            if (sfxToggle != null)
            {
                sfxToggle = GameObject.FindGameObjectWithTag("sfx toggle").GetComponent<Toggle>();
                sfxToggle.isOn = !sfxMuted;
            }

            if (musicToggle != null)
            {
                musicToggle = GameObject.FindGameObjectWithTag("music toggle").GetComponent<Toggle>();
                musicToggle.isOn = !musicMuted;

            }
        }
    }

    public void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void PlayCustomPitch(string name, float p)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        //float startPitch = s.source.pitch;
        s.source.pitch = p;
        s.source.Play();
        //s.source.pitch = startPitch;
    }

    public void Pause(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
    }

    public void Stop(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void muteMusic(bool mute)
    {
        foreach (Sound s in sounds)
        {
            if (s.soundType == Sound.type.music)
            {
                s.source.mute = mute;
            }
        }

        musicMuted = mute;

        //GameObject.FindGameObjectWithTag("music toggle").GetComponent<AudioToggle>().toggleSetting(!musicMuted);
    }

    public void muteSFX(bool mute)
    {
        foreach (Sound s in sounds)
        {
            if (s.soundType == Sound.type.sfx)
            {
                s.source.mute = mute;
            }
        }

        sfxMuted = mute;

        //GameObject.FindGameObjectWithTag("sfx toggle").GetComponent<AudioToggle>().toggleSetting(!sfxMuted);
    }

    public void changeMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = volume;
    }

    public void sceneChanged(string sceneName, bool continueMusic)
    {
        if (sceneName == "Menu Screen")
        {
            stopAllSFX();
            Stop("game_music");
            Play("menu_music");
        }

        if (sceneName == "Game Screen")
        {
            stopAllSFX();

            if (!continueMusic)
            {
                Stop("menu_music");
                Play("game_music");
            }
        }
    }

    public void stopAllSFX()
    {
        foreach (Sound s in sounds)
        {
            if (s.soundType == Sound.type.sfx)
            {
                Stop(s.name);
            }
        }
    }
}
