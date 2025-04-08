using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioMixerGroup musicMixerGroup;
    public List<AudioClip> musicTracks = new List<AudioClip>();

    [Header("Default Settings")]
    [Range(0f, 1f)] public float defaultVolume = 0.7f;
    public float fadeDuration = 1f;

    private AudioSource[] audioSources;
    private int currentSourceIndex = 0;
    private float currentVolume;
    private Dictionary<string, AudioClip> trackDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // Implementação do Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Cria dois AudioSources para crossfade
        audioSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].outputAudioMixerGroup = musicMixerGroup;
            audioSources[i].loop = true;
            audioSources[i].volume = 0f;
        }

        currentVolume = defaultVolume;

        // Preenche o dicionário de músicas para acesso rápido
        foreach (var track in musicTracks)
        {
            trackDictionary[track.name] = track;
        }
    }

    public void PlayTrack(string trackName, bool forceRestart = false)
    {
        if (!trackDictionary.ContainsKey(trackName))
        {
            Debug.LogWarning($"Track '{trackName}' not found in MusicManager!");
            return;
        }

        AudioClip clipToPlay = trackDictionary[trackName];

        // Se já está tocando a mesma música e não queremos reiniciar
        if (audioSources[currentSourceIndex].clip == clipToPlay && !forceRestart)
        {
            return;
        }

        // Troca para o outro AudioSource
        int newSourceIndex = 1 - currentSourceIndex;
        audioSources[newSourceIndex].clip = clipToPlay;
        audioSources[newSourceIndex].Play();

        StartCoroutine(CrossFade(fadeDuration));

        currentSourceIndex = newSourceIndex;
    }

    private IEnumerator CrossFade(float duration)
    {
        float timer = 0f;
        AudioSource fadingOut = audioSources[1 - currentSourceIndex];
        AudioSource fadingIn = audioSources[currentSourceIndex];

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            fadingIn.volume = Mathf.Lerp(0f, currentVolume, progress);
            fadingOut.volume = Mathf.Lerp(currentVolume, 0f, progress);
            yield return null;
        }

        fadingOut.Stop();
        fadingOut.volume = 0f;
        fadingIn.volume = currentVolume;
    }

    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp(volume, 0f, 1f);
        audioSources[currentSourceIndex].volume = currentVolume;

        // Se estiver usando AudioMixer
        if (musicMixerGroup != null)
        {
            // Converte de 0-1 para decibéis (aproximadamente)
            float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", dB);
        }
    }

    public void StopMusic(float fadeOutDuration = 0f)
    {
        if (fadeOutDuration <= 0f)
        {
            audioSources[0].Stop();
            audioSources[1].Stop();
            audioSources[0].volume = 0f;
            audioSources[1].volume = 0f;
        }
        else
        {
            StartCoroutine(FadeOut(fadeOutDuration));
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        AudioSource activeSource = audioSources[currentSourceIndex];
        float startVolume = activeSource.volume;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f;
    }

    public void PauseMusic()
    {
        audioSources[currentSourceIndex].Pause();
    }

    public void ResumeMusic()
    {
        audioSources[currentSourceIndex].UnPause();
    }
}