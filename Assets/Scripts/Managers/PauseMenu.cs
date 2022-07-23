using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
	private float m_TimeScaleRef = 1f;
    private float m_VolumeRef = 1f;
    private bool m_Paused;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject anykeyButton;
    [SerializeField] GameObject returnButton;

    [SerializeField] GameObject masterSlider;
    [SerializeField] GameObject sfxSlider;
    [SerializeField] GameObject musicSlider;
    [SerializeField] GameObject mouseSlider;
    [SerializeField] TextMeshProUGUI mouseSens;
    [SerializeField] TMP_Dropdown graphicsDropdown;

    RigidBodyMovement player;
    GrapplingHook grapple;
    UIManager ui;
    SettingsManager settings;

    public AudioMixer audioMixer;
    private bool tempControls;

    void Awake()
    {
        grapple = FindObjectOfType<GrapplingHook>();
        player = FindObjectOfType<RigidBodyMovement>();
        ui = FindObjectOfType<UIManager>();

        tempControls = true;
        controlMenu.SetActive(true);
        anykeyButton.SetActive(true);
        returnButton.SetActive(false);
	}

    private void Start()
    {
        InitializeSettings();
    }

    private void MenuOn ()
    {
        Cursor.lockState = CursorLockMode.None;

        player.isPaused = true;
        grapple.isPaused = true;
        pauseMenu.SetActive(true);
        returnButton.SetActive(true);

        m_TimeScaleRef = Time.timeScale;
        Time.timeScale = 0f;

        m_VolumeRef = AudioListener.volume;
        AudioListener.volume = 0f;

        m_Paused = true;
    }


    public void MenuOff ()
    {
        Cursor.lockState = CursorLockMode.Locked;

        player.isPaused = false;
        grapple.isPaused = false;
        pauseMenu.SetActive(false);
        controlMenu.SetActive(false);
        settingsMenu.SetActive(false);
        returnButton.SetActive(false);

        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;
        m_Paused = false;
    }

    public void SaveAndQuit ()
    {
        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;

        SaveSystem.SaveGameState(player, ui);
        SaveSystem.SaveSettings(settings);
        SceneManager.LoadScene("Main Menu");
    }

    public void ApplyChanges ()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void Return ()
    {
        pauseMenu.SetActive(true);
        controlMenu.SetActive(false);        
    }

    private void InitializeSettings()
    {
        try
        {
            Debug.Log("Loading player settings.");
            settings = SaveSystem.LoadSettings();
            
            ChangeMouseSens(settings.mouseSens);
            SetMasterVolume(settings.masterVolume);
            SetSFXVolume(settings.sfxVolume);
            SetMusicVolume(settings.musicVolume);
            SetQuality(settings.qualityLevel);
            SetFullscreen(settings.fullscreenState);

            masterSlider.GetComponent<Slider>().value = settings.masterVolume;
            sfxSlider.GetComponent<Slider>().value = settings.sfxVolume;
            musicSlider.GetComponent<Slider>().value = settings.musicVolume;
            mouseSlider.GetComponent<Slider>().value = settings.mouseSens;
            mouseSens.text = Math.Round((decimal)settings.mouseSens, 1).ToString();
            graphicsDropdown.value = settings.qualityLevel;
        }
        catch
        {
            Debug.Log("No settings save file detected, using defaults.");
            settings = new SettingsManager();
        }
    }

    public void ChangeMouseSens (float sens)
    {
        player.sensMultiplier = sens;
        mouseSens.text = sens.ToString("f1");
        settings.mouseSens = sens;
    }

    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("MasterVolume", -80); // Mutes if player goes to lowest option
        }
        settings.masterVolume = volume;
    }

    public void SetSFXVolume (float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("SFXVolume", -80); 
        }
        settings.sfxVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("MusicVolume", -80); 
        }
        settings.musicVolume = volume;
    }

    public void SetQuality (int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        settings.qualityLevel = qualityLevel;
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        settings.fullscreenState = isFullscreen;
    }

    public void OnMenuStatusChange ()
    {
        if (!m_Paused)
        {
            MenuOn();
        }
        else if (m_Paused)
        {
            MenuOff();
        }
    }

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
            OnMenuStatusChange();
            Cursor.visible = m_Paused; //force the cursor visible if anythign had hidden it
		}
        if(tempControls)
        {
            if (Input.anyKey)
            {
                controlMenu.SetActive(false);
                tempControls = false;
                anykeyButton.SetActive(false);
            }
        }
	}

}
