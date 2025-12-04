using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    //NICAM LIU

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioMixer myMixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider ambienceSlider;

    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text SFXVolumeText;
    [SerializeField] private TMP_Text ambienceVolumeText;



    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetAmbienceVolume();

        }


        settingsPanel.SetActive(false);

    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);

        // Update UI
        float percent = volume * 100f;
        musicVolumeText.text = percent.ToString("F0") + "%";
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Update UI
        float percent = volume * 100f;
        SFXVolumeText.text = percent.ToString("F0") + "%";
    }

    public void SetAmbienceVolume()
    {
        float volume = ambienceSlider.value;
        myMixer.SetFloat("ambience", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("ambienceVolume", volume);

        // Update UI
        float percent = volume * 100f;
        ambienceVolumeText.text = percent.ToString("F0") + "%";
    }

    private void LoadVolume()
    {

        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        ambienceSlider.value = PlayerPrefs.GetFloat("ambienceVolume");


        SetMusicVolume();
        SetSFXVolume();
        SetAmbienceVolume();
    }

}
