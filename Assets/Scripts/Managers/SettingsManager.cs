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
        mouseSens = 1;
        masterVolume = 0;
        sfxVolume = 0;
        musicVolume = 0;
        qualityLevel = 4;
        fullscreenState = true;
    }

}
