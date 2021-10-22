using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    RigidBodyMovement player;
    [SerializeField] Transform destination;

    float dToD; // distance to destination

    [SerializeField] TextMeshProUGUI progressPercentage;
    [SerializeField] TextMeshProUGUI locationText;
    [SerializeField] TextMeshProUGUI locationBroadcast; 
    Image dash1;
    Image dash2;

    [SerializeField] TextMeshProUGUI timer;
    float timeRemaining = 3600f;

    bool transitioning;

    bool area1_discovered;
    bool area2_discovered;
    bool area3_discovered;

    [SerializeField] Material skybox1;
    [SerializeField] Material skybox2;
    [SerializeField] Material skybox3;
    [SerializeField] Material skybox4;


    private void Awake()
    {
        player = playerGO.GetComponent<RigidBodyMovement>();
        dash1 = locationBroadcast.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        dash2 = locationBroadcast.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.3f);

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        timer.text = (Mathf.Floor(timeRemaining / 60f)).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60f).ToString("00");


        if (player.grounded)
        {
            dToD = (playerGO.transform.position.y / destination.position.y * 100)-0.88f;
            if (dToD > 100f)
                dToD = 100f;
            progressPercentage.text = dToD.ToString("F2") + "%";
        }                

        if (dToD < 50.0f)
        {          
            UpdateLocation("The World Tree", area1_discovered);
            area1_discovered = true;
            UpdateSkybox(1);
        }
        else if (dToD > 50.0f && dToD < 99f)
        {
            UpdateLocation("Half Way", area2_discovered);
            area2_discovered = true;
            UpdateSkybox(2);
        }
        else if (dToD >= 99f)
        {
            UpdateLocation("Demo Completed", area3_discovered);
            area3_discovered = true;
            UpdateSkybox(3);
        }
    }

    public void UpdateSkybox(int skyboxIndex)
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

        Color opaque = new Color(1, 1, 1, 1);
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
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            locationBroadcast.color = newColor;
            dash1.color = newColor;
            dash2.color = newColor;
            yield return null;
        }
        transitioning = false;
        locationBroadcast.gameObject.SetActive(false);
    }
}
