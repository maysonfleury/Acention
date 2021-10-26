using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishPlatform : MonoBehaviour
{
    Material vanishMaterial;
    [SerializeField] float vanishDuration = 3f;
    [SerializeField] float vanishFrequency = 5f;

    float timer = 0;
    bool transitioning;

    void Start()
    {
        vanishMaterial = gameObject.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= vanishFrequency && !transitioning)
        {
            StartCoroutine(PlatformVanish(vanishDuration));
        }
    }

    IEnumerator PlatformVanish(float vanishDuration)
    {
        transitioning = true;

        gameObject.GetComponent<BoxCollider>().enabled = false;
        for (float time = 0.0f; time < 1.0f; time += Time.deltaTime / 3f)
        {
            Color transparent = new Color(1, 1, 1, Mathf.Lerp(vanishMaterial.color.a, 0.0f, time));
            vanishMaterial.color = transparent;
            yield return null;
        }
        for (float time = 0.0f; time < 1.0f; time += Time.deltaTime / vanishDuration)
        {            
            yield return null;
        }
        for (float time = 0.0f; time < 1.0f; time += Time.deltaTime / 3f)
        {
            Color opaque = new Color(1, 1, 1, Mathf.Lerp(vanishMaterial.color.a, 1.0f, time));
            vanishMaterial.color = opaque;
            gameObject.GetComponent<BoxCollider>().enabled = true;
            yield return null;
        }

        timer = 0;
        transitioning = false;        
        yield return null;
    }
}

