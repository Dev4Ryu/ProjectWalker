using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public AudioMixer audioMixer;

    private Resolution[] resolutions;

    void Start()
    {
        // ===== RESOLUTIONS =====
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Hook up resolution change event
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // ===== FULLSCREEN =====
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // ===== VOLUME =====
        if (audioMixer.GetFloat("MainVolume", out float currentVolumeDb))
        {
            float volumeLinear = Mathf.Pow(10, currentVolumeDb / 20);
            volumeSlider.value = volumeLinear;
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetResolution(int resolutionIndex)
    {
        Debug.Log("Resolution changed to index: " + resolutionIndex);
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetVolume(float sliderValue)
    {
        float volumeDb = sliderValue;
        audioMixer.SetFloat("MainVolume", volumeDb);
        Debug.Log("Volume set to: " + volumeDb + " dB");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen set to: " + isFullscreen);
    }
}
