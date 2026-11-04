using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minimal settings screen: master volume, fullscreen, and a close button.
/// Every field is optional, so you can wire only the parts you have built.
/// Values are stored in PlayerPrefs and re-applied on every launch.
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    private const string VolumeKey = "Settings_MasterVolume";
    private const string FullscreenKey = "Settings_Fullscreen";

    [Header("Optional UI")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Button closeButton;

    [Header("References")]
    public MenuManager menuManager;

    private bool isApplyingSavedValues = false;

    private void Awake()
    {
        if (menuManager == null)
        {
            menuManager = FindFirstObjectByType<MenuManager>();
        }

        LoadAndApply();

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    /// <summary>
    /// Reads saved settings and pushes them into Unity and into the UI.
    /// Safe to call from any scene.
    /// </summary>
    public void LoadAndApply()
    {
        isApplyingSavedValues = true;

        float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        AudioListener.volume = Mathf.Clamp01(volume);
        Screen.fullScreen = fullscreen;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
        }

        isApplyingSavedValues = false;
    }

    public void OnVolumeChanged(float value)
    {
        if (isApplyingSavedValues)
        {
            return;
        }

        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);
        PlayerPrefs.Save();

        Debug.Log("Master volume set to " + AudioListener.volume.ToString("0.00"));
    }

    public void OnFullscreenChanged(bool isOn)
    {
        if (isApplyingSavedValues)
        {
            return;
        }

        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt(FullscreenKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Fullscreen set to " + isOn);
    }

    public void Close()
    {
        if (menuManager != null)
        {
            menuManager.CloseSettings();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Wipes saved settings back to defaults.
    /// </summary>
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteKey(VolumeKey);
        PlayerPrefs.DeleteKey(FullscreenKey);
        PlayerPrefs.Save();

        LoadAndApply();
        Debug.Log("Settings reset to defaults.");
    }
}