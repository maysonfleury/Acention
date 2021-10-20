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
    [SerializeField] TextMeshProUGUI locationMessage;
    Image dash1;
    Image dash2;

    bool transitioning;

    private void Awake()
    {
        player = playerGO.GetComponent<RigidBodyMovement>();
        dash1 = locationMessage.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        dash2 = locationMessage.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (player.grounded)
        {
            dToD = playerGO.transform.position.y / destination.position.y * 100;
            progressPercentage.text = dToD.ToString("F2") + "%";
        }
        if (dToD > 50.0f)
        {
            BroadcastLocation("Maysom");
        }
    }

    public void BroadcastLocation(string locationName)
    {
        locationMessage.gameObject.SetActive(true);
        locationMessage.text = locationName;
        if (!transitioning)
        {
            StartCoroutine(FadeTo(0.0f, 1f));
            transitioning = true;
        }
    }

    
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = locationMessage.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) //wait then proceed
        {
            yield return null;
        }
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            locationMessage.color = newColor;
            dash1.color = newColor;
            dash2.color = newColor;
            yield return null;
        }
    }
}
