using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    RigidBodyMovement player;
    [SerializeField] Transform destination;

    float dToD; // distance to destination

    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject reticle;

    [SerializeField] TextMeshProUGUI progressPercentage;
    [SerializeField] TextMeshProUGUI locationText;
    [SerializeField] TextMeshProUGUI locationBroadcast;
    [SerializeField] TextMeshProUGUI finalTime;
    Image dash1;
    Image dash2;

    [SerializeField] TextMeshProUGUI timer;
    float timeRemaining = 3600f;

    bool transitioning;

    [SerializeField] Transform tutorial_start;
    [SerializeField] Transform area1_start;

    public bool area1_discovered;
    public bool area2_discovered;
    public bool area3_discovered;
    public bool area4_discovered;

    [SerializeField] Material skybox1;
    [SerializeField] Material skybox2;
    [SerializeField] Material skybox3;
    [SerializeField] Material skybox4;
    [SerializeField] Material skybox_gameOver;

    bool gameOver;
    bool gameWon;
    MusicManager mm;

    private void Awake()
    {
        player = playerGO.GetComponent<RigidBodyMovement>();
        dash1 = locationBroadcast.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        dash2 = locationBroadcast.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();

        mm = FindObjectOfType<MusicManager>();
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.3f);

        if (player.gameStart == true && !gameOver)
        {
            timer.gameObject.SetActive(true);
            locationText.gameObject.SetActive(true);
            progressPercentage.gameObject.SetActive(true);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timer.text = (Mathf.Floor(timeRemaining / 60f)).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60f).ToString("00");
            }
            else
            {
                if (!gameOver)
                {
                    FindObjectOfType<GameOver>().EndGame();
                    EndGame();
                    timer.gameObject.SetActive(false);
                }
            }          
        }

        if (player.grounded)
        {
            
            dToD = (playerGO.transform.position.y / destination.position.y * 100)-0.88f;
            if (dToD < 0f)
                dToD = 0;
            if (dToD > 100f)
                dToD = 100f;
            progressPercentage.text = dToD.ToString("F2") + "%";
        }
        if (dToD < 33.4f && player.gameStart == true)
        {
            if (!area1_discovered)
                mm.GameStart();
            else
                mm.ChangeSong(1);

            UpdateLocation("The World Tree", area1_discovered);
            area1_discovered = true;
            UpdateSkybox(1);
        }
        else if (dToD >= 33.4f && dToD < 53.4f)
        {
            UpdateLocation("Crystal Core", area2_discovered);
            area2_discovered = true;
            UpdateSkybox(2);
            mm.ChangeSong(2);
        }
        else if (dToD >= 53.4f && dToD < 99f)
        {
            UpdateLocation("Sacred Spire", area3_discovered);
            area3_discovered = true;
            UpdateSkybox(3);
            mm.ChangeSong(3);
        }
        else if (dToD >= 99f )
        {
            UpdateLocation("True Core", area4_discovered);
            area4_discovered = true;
            UpdateSkybox(4);
            mm.ChangeSong(4);
            if (!gameWon && player.gameWon)
                WinGame();
        }    
    }

    void EndGame()
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        MusicManager mm = FindObjectOfType<MusicManager>();
        CrystalEffects crystal = FindObjectOfType<CrystalEffects>();
        gameOver = true;
        progressPercentage.gameObject.SetActive(false);
        locationText.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
        UpdateLocation("The Crystal Hungers", false);
        dash1.rectTransform.position += new Vector3(-45f, 0, 0);
        dash2.rectTransform.position += new Vector3(45f, 0, 0);
        am.Play("gameover_1");
        am.Play("gameover_2");
        am.Play("gameover_3");
        mm.GameOver();
        crystal.GameEnd();
        RenderSettings.skybox = skybox_gameOver;
    }

    void WinGame()
    {
        gameWon = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        reticle.SetActive(false);

        GrapplingHook grapple = FindObjectOfType<GrapplingHook>();
        player.isPaused = true;
        grapple.isPaused = true;
        timer.gameObject.SetActive(false);
        victoryScreen.SetActive(true);
        TimeSpan t = TimeSpan.FromSeconds(3600f - timeRemaining);

        string time = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
        finalTime.text = time;
    }
    public void UpdateSkybox(int skyboxIndex)
    {
        if (!gameOver)
        {
            if (skyboxIndex == 1)
            {
                RenderSettings.skybox = skybox1;
            }
            else if (skyboxIndex == 2)
            {
                RenderSettings.skybox = skybox2;
            }
            else if (skyboxIndex == 3)
            {
                RenderSettings.skybox = skybox3;
            }
            else if (skyboxIndex == 4)
            {
                RenderSettings.skybox = skybox4;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResetPosition()
    {
        if (player.gameStart == true)
            playerGO.transform.position = area1_start.position;
        else
            playerGO.transform.position = tutorial_start.position;
    }
    public void UpdateLocation(string locationName, bool locationDiscovered)
    {      
        locationText.text = locationName;      
        if (!locationDiscovered)
            BroadcastLocation(locationName);
    }
    void BroadcastLocation(string locationName)
    {
        locationBroadcast.gameObject.SetActive(true);
        locationBroadcast.text = locationName;
        if (!transitioning)
        {
            StartCoroutine(FadeTo(0.0f, 2f));
            transitioning = true;
        }
    }

    
    IEnumerator FadeTo(float aValue, float aTime)
    {
        Color opaque;
        if (gameOver)
            opaque = new Color(0.5f, 0, 0, 1);
        else
            opaque = new Color(1, 1, 1, 1);
        locationBroadcast.color = opaque;
        dash1.color = opaque;
        dash2.color = opaque; 

        float alpha = locationBroadcast.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) //wait then proceed
        {
            yield return null;
        }
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor;
            if (gameOver)
                newColor = new Color(0.5f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            else
                newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            locationBroadcast.color = newColor;
            dash1.color = newColor;
            dash2.color = newColor;
            yield return null;
        }
        transitioning = false;
        locationBroadcast.gameObject.SetActive(false);
    }
}
