using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioMixer SoundMixer;
    public static float MasterVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("master-volume", 1);
        }
        set
        {
            PlayerPrefs.SetFloat("master-volume", value);
        }
    }
    public static float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("music-volume", 1);
        }
        set
        {
            PlayerPrefs.SetFloat("music-volume", value);
        }
    }

    [SerializeField] AudioSource _genericSource;
    [SerializeField] AudioClip _buttonSound;
    [SerializeField] AudioClip _buildSound;
    [SerializeField] AudioClip _collectSound;
    [SerializeField] AudioClip _alertSound;

    Tween _gasSoundTween;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateMasterVolume();
        //UpdateMusicVolume();
    }

    public static void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        Instance.UpdateMasterVolume();
    }

    public static void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        Instance.UpdateMusicVolume();
    }

    public void UpdateMasterVolume()
    {
        SoundMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
    }

    public void UpdateMusicVolume()
    {
        SoundMixer.SetFloat("Music", Mathf.Log10(MusicVolume) * 20);
    }


    public void PlayButtonSound()
    {
        _genericSource.PlayOneShot(_buttonSound);
    }

    public void PlayBuildSound()
    {
        _genericSource.PlayOneShot(_buildSound);
    }

    public void PlayCollectSound()
    {
        _genericSource.PlayOneShot(_collectSound);
    }
    public void PlayAlertSound()
    {
        _genericSource.PlayOneShot(_alertSound);
    }
}
