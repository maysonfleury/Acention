using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager_MainMenu : MonoBehaviour
{
    [SerializeField] Image titleImage;

    private float m_VolumeRef = 1f;
    private bool m_Paused;

    [SerializeField] GameObject controlMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject returnButton;

    [SerializeField] TextMeshProUGUI mouseSens;

    MusicManager mm;

    private void Awake()
    {
        mm = FindObjectOfType<MusicManager>();
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
        // TODO: Save?
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
