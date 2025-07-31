using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Setting Tabs")]
    [SerializeField] private List<GameObject> tabs;

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

    private void Start()
    {
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
            Resolution nativeRes = Screen.currentResolution;
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                if (filteredResolutions[i].width == nativeRes.width && filteredResolutions[i].height == nativeRes.height)
                {
                    currentResolutionIndex = i;
                    resolutionDropdown.value = currentResolutionIndex;
                    resolutionDropdown.RefreshShownValue();
                    break;
                }
            }

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
