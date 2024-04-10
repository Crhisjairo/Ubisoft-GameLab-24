using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    private const string _volumeKey = "volume";
    private const string _fullscreenKey = "fullscreen";
    private const string _indexResolutionKey = "indexResolution";
    
    private int _defaultResolutionIndex = 0;
    
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        _defaultResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                _defaultResolutionIndex = i;
            }
        }

        // Changer la valeur
        resolutionDropdown.AddOptions(options);
        
        resolutionDropdown.RefreshShownValue();
        
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(_volumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(_volumeKey);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }

        if (PlayerPrefs.HasKey(_fullscreenKey))
        {
            int savedFullscreen = PlayerPrefs.GetInt(_fullscreenKey);
            bool isFullscreen = savedFullscreen == 1;
            SetFullScreen(isFullscreen);
        }

        if (PlayerPrefs.HasKey(_indexResolutionKey))
        {
            SetResolution(PlayerPrefs.GetInt(_indexResolutionKey));
            Debug.Log("Resolution readed: " + PlayerPrefs.GetInt(_indexResolutionKey));
        }
        else
        {
            SetResolution(_defaultResolutionIndex);
        }
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionDropdown.value = resolutionIndex;
        
        Debug.Log("Resolution saved: " + resolutionIndex);

        PlayerPrefs.SetInt(_indexResolutionKey, resolutionIndex);
        PlayerPrefs.Save();
        
        Debug.Log("Resolution readed after save: " + PlayerPrefs.GetInt(_indexResolutionKey));
    }

    // Changer le volume
    public void SetVolume(float volume)
    {
        float processedVolume = Mathf.Log10(volume) * 20;
        
        audioMixer.SetFloat(_volumeKey, processedVolume);
        
        PlayerPrefs.SetFloat(_volumeKey, volume);
        PlayerPrefs.Save();
    }

    // Changer le jeu en fullscreen
    public void SetFullScreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;
        
        PlayerPrefs.SetInt(_fullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
