using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] private Slider SESlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SEVolume"))
            LoadSEVolume();
        else
            SetSEVolume();
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusicVolume();
        else
            SetMusicVolume();
    }

    public void SetSEVolume()
    {
        float SEVolume = SESlider.value;
        audioMixer.SetFloat("SEVolume", Mathf.Log10(SEVolume) * 20);
        PlayerPrefs.SetFloat("SEVolume", SEVolume);

    }

    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    private void LoadSEVolume()
    {
        SESlider.value = PlayerPrefs.GetFloat("SEVolume");
        SetSEVolume();
    }

    private void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SetMusicVolume();
    }
}
