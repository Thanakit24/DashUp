using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private Scene currentScene;

    public static AudioManager instance;
    public AudioSource musicSource;
    public AudioClip menuClip, gameClip;

    public enum MusicTrack { Menu, Game }
    public MusicTrack currentMusic;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SwitchMusic(MusicTrack.Menu, true);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "MainMenu" || currentScene.name == "LevelSelection")
            SwitchMusic(MusicTrack.Menu);
        else
            SwitchMusic(MusicTrack.Game);
    }

    void SwitchMusic(MusicTrack newTrack, bool ignoreDuplicate = false)
    {
        if (!ignoreDuplicate && currentMusic == newTrack) return;
        switch (newTrack)
        {
            case MusicTrack.Menu:
                musicSource.clip = menuClip;
                //StopPlaying("GBM");
                //Play("BGM");
                break;
            case MusicTrack.Game:
                musicSource.clip = gameClip;
                //StopPlaying("BGM");
                //Play("GBM");
                break;
        }
        musicSource.Play();
        currentMusic = newTrack;
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
        s.source.PlayOneShot(s.clip);
        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        // randomize audiosource pitch here

    }

    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

       

        s.source.Stop();


    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //void SwitchMusic(MusicTrack newTrack, bool ignoreDuplicate = false, float fadeOutDuration = 0.2f, float fadeInDuration = 0.25f)
    //{
    //    if (!ignoreDuplicate && currentMusic == newTrack) return;

    //    StartCoroutine(FadeOutAndSwitch(newTrack, fadeOutDuration, fadeInDuration));
    //}

    //IEnumerator FadeOutAndSwitch(MusicTrack newTrack, float fadeOutDuration, float fadeInDuration)
    //{
    //    float startVolume = musicSource.volume;

    //    while (musicSource.volume > 0)
    //    {
    //        musicSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
    //        yield return null;
    //    }

    //    // Ensure the volume is set to 0 to avoid any potential issues
    //    musicSource.volume = 0;

    //    // Switch to the new track
    //    switch (newTrack)
    //    {
    //        case MusicTrack.Menu:
    //            musicSource.clip = menuClip;
    //            break;
    //        case MusicTrack.Game:
    //            musicSource.clip = gameClip;
    //            break;
    //    }

    //    // Start playing the new track
    //    musicSource.Play();

    //    // Gradually increase the volume back to its original level
    //    while (musicSource.volume < startVolume)
    //    {
    //        musicSource.volume += startVolume * Time.deltaTime / fadeInDuration;
    //        yield return null;
    //    }

    //    // Ensure the volume is set to the original level
    //    musicSource.volume = startVolume;

    //    // Update the current music track
    //    currentMusic = newTrack;
    //}

}