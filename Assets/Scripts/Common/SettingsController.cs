using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;

    void OnEnable()
    {
        _masterVolumeSlider.value = SoundManager.MasterVolume;
        //_musicVolumeSlider.value = SoundManager.MusicVolume;
    }

    public void SetMasterVolume()
    {
        SoundManager.SetMasterVolume(_masterVolumeSlider.value);
        //HomeUI.Instance.UpdateSoundButton();
    }

    public void SetMusicVolume()
    {
        SoundManager.SetMusicVolume(_musicVolumeSlider.value);
    }
}
