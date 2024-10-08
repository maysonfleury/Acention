using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    UIManager ui;
    public bool isGameOver;

    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
    }
    public void EndGame()
    {
        isGameOver = true;
        CameraShake camera = FindObjectOfType<CameraShake>();
        StartCoroutine(camera.ShakeAndQuit(100f, 1.2f));
    }
}
