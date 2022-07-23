using System;
using UnityEngine;

[System.Serializable]
public class SettingsManager
{
    public float mouseSens;
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public int qualityLevel;
    public bool fullscreenState;

    public SettingsManager()
    {
        mouseSens = 1.0f;
        masterVolume = -10f;
        sfxVolume = -9f;
        musicVolume = -10f;
        qualityLevel = 4;
        fullscreenState = true;
    }

}
