using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
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
    [SerializeField] GameObject tip;

    [SerializeField] TextMeshProUGUI mouseSens;

    RigidBodyMovement player;
    GrapplingHook grapple;
    UIManager ui;

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
        tip.SetActive(true);
        returnButton.SetActive(false);
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
        SaveSystem.SaveGameState(player);
        Application.Quit();
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

    public void LudwigCheck (System.Single vol)
    {
        if (vol == 1.0)
        {
            
        }
    }

    public void ChangeMouseSens (float sens)
    {
        player.sensMultiplier = sens;
        mouseSens.text = sens.ToString("f1");
    }

    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("MasterVolume", -80); // Mutes if player goes to lowest option
        }
    }

    public void SetSFXVolume (float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("SFXVolume", -80); 
        }
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        if (volume == -40)
        {
            audioMixer.SetFloat("MusicVolume", -80); 
        }
    }

    public void SetQuality (int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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
                tip.SetActive(false);
            }
        }
	}

}
