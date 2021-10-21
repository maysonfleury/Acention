using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
	private float m_TimeScaleRef = 1f;
    private float m_VolumeRef = 1f;
    private bool m_Paused;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlMenu;

    RigidBodyMovement player;
    GrapplingHook grapple;

    public AudioMixer audioMixer;

    void Awake()
    {
        grapple = FindObjectOfType<GrapplingHook>();
        player = FindObjectOfType<RigidBodyMovement>();
	}


    private void MenuOn ()
    {
        Cursor.lockState = CursorLockMode.None;

        player.isPaused = true;
        grapple.isPaused = true;
        pauseMenu.SetActive(true);

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

        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;
        m_Paused = false;
    }

    public void SaveAndQuit ()
    {
        Debug.Log("Qutting game");
        Application.Quit();
    }

    public void ApplyChanges ()
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
    }

    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume (float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
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
	}

}
