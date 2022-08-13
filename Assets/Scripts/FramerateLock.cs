using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLock : MonoBehaviour
{
    int screenRefreshRate;
    void Start()
    {
        screenRefreshRate = Screen.currentResolution.refreshRate;
        Application.targetFrameRate = screenRefreshRate;
    }
}
