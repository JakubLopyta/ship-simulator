using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesController : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField] private bool canUse = true;
    [SerializeField] private SettingsController settingsController;

    [Header("Vsync Setting")]
    [SerializeField] private Toggle vsyncToggle;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("vSync"))
            {
                int vsyncCount = PlayerPrefs.GetInt("vSync");

                if (vsyncCount == 1)
                {
                    QualitySettings.vSyncCount = 1;
                    vsyncToggle.isOn = true;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                    vsyncToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                volumeTextValue.text = localVolume.ToString("0") + "%";
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
        }
    }
}
