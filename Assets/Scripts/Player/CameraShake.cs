using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine co = null;

    public void StartShake(float magnitude)
    {
        if (co == null)
            co = StartCoroutine(Shake(9999f, magnitude));
    }

    public void StopShake()
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }
    
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos; 
    }


    public IEnumerator ShakeAndQuit(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        Application.Quit();
        transform.localPosition = originalPos; 
    }
}
