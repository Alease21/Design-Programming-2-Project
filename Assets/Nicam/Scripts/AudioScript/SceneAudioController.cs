using UnityEngine;
using System.Collections.Generic;

public class SceneAudioController : MonoBehaviour
{
    [Header("Music Settings")]
    [Tooltip("Single music track (will loop)")]
    [SerializeField] AudioClip musicTrack;

    [Tooltip("Or multiple music tracks (will play in sequence)")]
    [SerializeField] List<AudioClip> musicPlaylist = new List<AudioClip>();

    [Header("Ambience Settings")]
    [SerializeField] bool hasAmbience = false;
    [SerializeField] AudioClip ambienceClip;

    [Header("SFX Settings")]
    [Tooltip("Button click sound for this scene")]
    [SerializeField] AudioClip buttonClickSFX;

    private void Start()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogWarning("AudioManager not found in scene!");
            return;
        }

        //set music
        if (musicPlaylist.Count > 0)
        {
            //playlist
            AudioManager.instance.ChangePlaylist(musicPlaylist);
        }
        else if (musicTrack != null)
        {
            //use single
            AudioManager.instance.ChangeMusic(musicTrack);
        }

        // Setup Ambience
        if (hasAmbience && ambienceClip != null)
        {
            AudioManager.instance.ChangeAmbience(ambienceClip);
        }
        else
        {
            //stop if false
            AudioManager.instance.StopAmbience();
        }
    }

    //click
    public void PlaySceneButtonClick()
    {
        if (buttonClickSFX != null)
        {
            AudioManager.instance.PlaySFX(buttonClickSFX);
        }
    }
}