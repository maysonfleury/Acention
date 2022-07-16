using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] public float mouseSens;
    [SerializeField] public float masterVolume;
    [SerializeField] public float sfxVolume;
    [SerializeField] public float musicVolume;
    [SerializeField] public int qualityLevel;
    [SerializeField] public bool fullscreenState;

    public void SaveMouseSens (float sens)
    {
        this.mouseSens = sens;
    }

    public void SaveMasterVolume (float volume)
    {
        this.masterVolume = volume;
    }

    public void SaveSFXVolume (float volume)
    {
        this.sfxVolume = volume;
    }
    public void SaveMusicVolume(float volume)
    {
        this.musicVolume = volume;
    }

    public void SaveQuality (int level)
    {
        this.qualityLevel = level;
    }

    public void SaveFullscreen (bool isFullscreen)
    {
        this.fullscreenState = isFullscreen;
    }

}
