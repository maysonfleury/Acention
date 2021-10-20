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

    bool transitioning;

    bool area1_discovered;
    bool area2_discovered;
    bool area3_discovered;

    private void Awake()
    {
        player = playerGO.GetComponent<RigidBodyMovement>();
        dash1 = locationBroadcast.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        dash2 = locationBroadcast.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (player.grounded)
        {
            dToD = playerGO.transform.position.y / destination.position.y * 100;
            if (dToD > 100f)
                dToD = 100f;
            progressPercentage.text = dToD.ToString("F2") + "%";
        }

        if (dToD < 50.0f)
        {          
            UpdateLocation("Maysom", area1_discovered);
            area1_discovered = true;
        }
        else if (dToD > 50.0f && dToD < 100f)
        {
            UpdateLocation("Half way", area2_discovered);
            area2_discovered = true;
        }
        else if (dToD == 100f)
        {
            UpdateLocation("You made it", area3_discovered);
            area3_discovered = true;
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
