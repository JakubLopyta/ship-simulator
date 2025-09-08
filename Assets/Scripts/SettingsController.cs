using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Setting Tabs")]
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<Button> buttons;
    private Color translucentButtonColor = new Color32(0, 0, 0, 0);
    private Color selectedButtonColor = new Color32(107, 125, 197, 190);

    [Header("Display")]
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    [Header("Vsync")]
    [SerializeField] private Toggle vsyncToggle;

    [Header("Volume")]
    [SerializeField] private TMP_Text volumeTextValue;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume = 100;

    private void Start()
    {
        //Setting Tabs
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].activeSelf)
            {
                ColorBlock activeButtonColor = buttons[i].colors;
                activeButtonColor.normalColor = selectedButtonColor;
                buttons[i].colors = activeButtonColor;
            }
        }

        //Resolution
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        
        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> resolutionOptions = new List<string>();

        for(int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " × " + filteredResolutions[i].height;
            resolutionOptions.Add(option);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //Display
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) 
        {
            displayModeDropdown.value = 2;
            resolutionDropdown.interactable = true;
        }
        else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            displayModeDropdown.value = 1;
            resolutionDropdown.interactable = false;
        }
        else
        {
            displayModeDropdown.value = 0;
            resolutionDropdown.interactable = true;
        }
        displayModeDropdown.RefreshShownValue();
    }

    public void ShowTab(int index)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(i == index);       
        }
    }

    public void ChangeColor(int index)
    {
        for(int i = 0; i< buttons.Count; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = (i == index) ? selectedButtonColor : translucentButtonColor;
            buttons[i].colors = colorBlock;
        }       
    }

    public void ResetButton()
    {
        if (tabs[0].activeSelf)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            int nativeResIndex = resolutionDropdown.options.Count - 1;
            resolutionDropdown.value = nativeResIndex;

            resolutionDropdown.interactable = false;
            Screen.fullScreen = true;
        }
        if (tabs[1].activeSelf)
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0") + "%";
        }
    }

    public void SetDisplayMode(int displayIndex)
    {
        if (displayIndex == 0)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            resolutionDropdown.interactable = true;
        }
        else if (displayIndex == 1)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            int nativeResIndex = resolutionDropdown.options.Count - 1;
            resolutionDropdown.value = nativeResIndex;

            resolutionDropdown.interactable = false;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            resolutionDropdown.interactable = true;
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVsync(bool vSync)
    {
        if (vSync == true) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;

        PlayerPrefs.SetInt("vSync", QualitySettings.vSyncCount);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0") + "%";

        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }
}
