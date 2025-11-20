using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    //NICAM LIU
    [Header("Audio Source Objects")]
    [SerializeField] GameObject musicObject;
    [SerializeField] GameObject sfxObject;
    [SerializeField] GameObject ambienceObject;

    private AudioSource musicSource;
    private AudioSource SFXSource;
    private AudioSource ambienceSource;

    [Header("Current Playing")]
    private List<AudioClip> musicTracks = new List<AudioClip>();
    private int currentTrackIndex = 0;
    private AudioClip currentAmbience;

    [Header("Transition Settings")]
    [SerializeField] float transitionDuration = 1.5f;

    public static AudioManager instance;

    private Coroutine musicTransitionCoroutine;
    private Coroutine ambienceTransitionCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //create persistent, just in case...
            if (musicObject == null)
            {
                musicObject = new GameObject("MusicSource");
                musicObject.transform.SetParent(transform);
                musicSource = musicObject.AddComponent<AudioSource>();
            }
            else
            {
                musicSource = musicObject.GetComponent<AudioSource>();
                if (musicSource == null) musicSource = musicObject.AddComponent<AudioSource>();
                DontDestroyOnLoad(musicObject);
            }

            if (sfxObject == null)
            {
                sfxObject = new GameObject("SFXSource");
                sfxObject.transform.SetParent(transform);
                SFXSource = sfxObject.AddComponent<AudioSource>();
            }
            else
            {
                SFXSource = sfxObject.GetComponent<AudioSource>();
                if (SFXSource == null) SFXSource = sfxObject.AddComponent<AudioSource>();
                DontDestroyOnLoad(sfxObject);
            }

            if (ambienceObject == null)
            {
                ambienceObject = new GameObject("AmbienceSource");
                ambienceObject.transform.SetParent(transform);
                ambienceSource = ambienceObject.AddComponent<AudioSource>();
            }
            else
            {
                ambienceSource = ambienceObject.GetComponent<AudioSource>();
                if (ambienceSource == null) ambienceSource = ambienceObject.AddComponent<AudioSource>();
                DontDestroyOnLoad(ambienceObject);
            }

            //configuring
            musicSource.loop = false;
            ambienceSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //only check if multiple track
        if (musicTracks.Count > 1 && musicSource.clip != null && musicSource.isPlaying)
        {
            //transition at end of tarck
            float timeRemaining = musicSource.clip.length - musicSource.time;
            if (timeRemaining <= transitionDuration / 2 && musicTransitionCoroutine == null)
            {
                PlayNextTrack();
            }
        }
    }

    //smooth transition
    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;

        //more than one, no loop
        musicSource.loop = false;

        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(TransitionMusic(musicTracks[currentTrackIndex]));
    }

    //change music
    public void ChangeMusic(AudioClip newMusic, bool forceChange = false)
    {
        if (newMusic == null) return;

        //check if new is playing
        if (musicSource.clip == newMusic && !forceChange)
        {
            //continue currnt
            return;
        }

        //clear and set to single
        musicTracks.Clear();
        musicTracks.Add(newMusic);
        currentTrackIndex = 0;
        musicSource.loop = true;

        //smooth transituon to new
        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(TransitionMusic(newMusic));
    }

    // Change music playlist and start playing
    public void ChangePlaylist(List<AudioClip> newTracks, int startIndex = 0)
    {
        if (newTracks == null || newTracks.Count == 0) return;

        //playing from this list?
        if (musicTracks.Count > 0 && newTracks.Contains(musicSource.clip))
        {
            //update, continue 
            musicTracks = new List<AudioClip>(newTracks);
            currentTrackIndex = musicTracks.IndexOf(musicSource.clip);
            //update loop
            musicSource.loop = (musicTracks.Count == 1);
            return;
        }

        //new playlist, transition..
        musicTracks = new List<AudioClip>(newTracks);
        currentTrackIndex = Mathf.Clamp(startIndex, 0, musicTracks.Count - 1);

        //set loop base on track
        musicSource.loop = (musicTracks.Count == 1);

        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(TransitionMusic(musicTracks[currentTrackIndex]));
    }

    //stop music fade out
    public void StopMusic()
    {
        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / transitionDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();
        musicSource.volume = startVolume;
        musicTracks.Clear();
    }

    private IEnumerator TransitionMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        //fade out
        for (float t = 0; t < transitionDuration / 2; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / (transitionDuration / 2));
            yield return null;
        }

        //cahnge music
        musicSource.clip = newClip;
        musicSource.Play();

        //fade in
        for (float t = 0; t < transitionDuration / 2; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / (transitionDuration / 2));
            yield return null;
        }

        musicSource.volume = startVolume;
        musicTransitionCoroutine = null;
    }

    //toggle amb
    public void SetAmbienceEnabled(bool enabled)
    {
        if (currentAmbience == null) return;

        if (ambienceTransitionCoroutine != null)
        {
            StopCoroutine(ambienceTransitionCoroutine);
        }

        if (enabled)
        {
            ambienceTransitionCoroutine = StartCoroutine(FadeInAmbience());
        }
        else
        {
            ambienceTransitionCoroutine = StartCoroutine(FadeOutAmbience());
        }
    }

    //cahnge amb
    public void ChangeAmbience(AudioClip newAmbience)
    {
        currentAmbience = newAmbience;

        if (newAmbience != null)
        {
            if (ambienceTransitionCoroutine != null)
            {
                StopCoroutine(ambienceTransitionCoroutine);
            }
            ambienceTransitionCoroutine = StartCoroutine(TransitionAmbience(newAmbience));
        }
        else
        {
            SetAmbienceEnabled(false);
        }
    }

    //stop amb
    public void StopAmbience()
    {
        if (ambienceTransitionCoroutine != null)
        {
            StopCoroutine(ambienceTransitionCoroutine);
        }
        ambienceTransitionCoroutine = StartCoroutine(FadeOutAmbience());
    }

    private IEnumerator FadeInAmbience()
    {
        if (!ambienceSource.isPlaying)
        {
            ambienceSource.clip = currentAmbience;
            ambienceSource.volume = 0;
            ambienceSource.Play();
        }

        float startVolume = ambienceSource.volume;
        float targetVolume = 1f;

        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            ambienceSource.volume = Mathf.Lerp(startVolume, targetVolume, t / transitionDuration);
            yield return null;
        }

        ambienceSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAmbience()
    {
        float startVolume = ambienceSource.volume;

        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            ambienceSource.volume = Mathf.Lerp(startVolume, 0, t / transitionDuration);
            yield return null;
        }

        ambienceSource.volume = 0;
        ambienceSource.Stop();
        currentAmbience = null;
    }

    private IEnumerator TransitionAmbience(AudioClip newClip)
    {
        float startVolume = ambienceSource.volume;

        //fade in
        for (float t = 0; t < transitionDuration / 2; t += Time.deltaTime)
        {
            ambienceSource.volume = Mathf.Lerp(startVolume, 0, t / (transitionDuration / 2));
            yield return null;
        }

        //cahnge
        ambienceSource.clip = newClip;
        ambienceSource.Play();

        //fadein
        for (float t = 0; t < transitionDuration / 2; t += Time.deltaTime)
        {
            ambienceSource.volume = Mathf.Lerp(0, startVolume, t / (transitionDuration / 2));
            yield return null;
        }

        ambienceSource.volume = startVolume;
    }

    //play sfx
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    //click
    public void PlayButtonClick(AudioClip buttonClip)
    {
        PlaySFX(buttonClip);
    }
}