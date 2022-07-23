using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;

public class UIManager_MainMenu : MonoBehaviour
{
    [SerializeField] Image titleImage;

    [SerializeField] GameObject controlMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject returnButton;

    [SerializeField] TextMeshProUGUI mouseSens;

    SettingsManager settings;
    MusicManager mm;
    public AudioMixer audioMixer;

    private void Awake()
    {
        mm = FindObjectOfType<MusicManager>();
    }

    private void Start()
    {
        mm.GameStart();
        InitializeSettings();
        mm.ChangeSong(1);
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.3f);
    }

    public void NewGame()
    {
        // Delete Old Save
        Debug.Log("Deleting Save File");
        SaveSystem.DeleteGameState();

        // Switch to Main Game Scene
        SceneManager.LoadScene("Acention");
    }

    public void ContinueGame()
    {
        // Switch to Main Game Scene
        SceneManager.LoadScene("Acention");
    }

    public void QuitGame()
    {
        // Save Settings
        SaveSystem.SaveSettings(settings);

        // Quit Application
        Debug.Log("Quitting Game...");
        Application.Quit();
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
        }
        catch
        {
            Debug.Log("No settings save file detected, using defaults.");
            settings = new SettingsManager();
        }
    }

    public void ChangeMouseSens (float sens)
    {
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
}
