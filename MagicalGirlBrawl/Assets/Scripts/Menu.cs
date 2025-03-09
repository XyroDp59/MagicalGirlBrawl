using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Menu
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject optionsMenu;
    
    private FMOD.Studio.EventInstance _musicInstance;
    
    // Options
    [SerializeField] private Slider masterSlider; 
    [SerializeField] private Slider soundtrackSlider; 
    [SerializeField] private Slider sfxSlider;

    
    private FMOD.Studio.Bus _masterBus;
    private FMOD.Studio.Bus _soundtrackBus;
    private FMOD.Studio.Bus _sfxBus;

    void Awake()
    {
        _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        _soundtrackBus = FMODUnity.RuntimeManager.GetBus("bus:/Soundtrack");
        _sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }
    
    // ---------- MENU ------------------- 
    public void OnPlayButton()
    {
        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("Game");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnSettingsButton()
    {
        menu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    // ----------- OPTIONS ---------------------
    public void OnSettingClosed()
    {
        menu.SetActive(true);
        optionsMenu.SetActive(false);
    }
    
    public void OnMasterVolumeChange()
    {
        _masterBus.setVolume(masterSlider.value);
    }

    public void OnSoundtrackVolumeChange()
    {
        _soundtrackBus.setVolume(soundtrackSlider.value);
    }

    public void OnSFXVolumeChange()
    {
        _sfxBus.setVolume(sfxSlider.value);
    }
    
    // ------------ OTHER -----------------
    void Start()
    {
        _musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Soundtrack/Magical Destiny");
        _musicInstance.start();
    }
}
