using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public float magnitude;
    public Vector3 direction;
    public float rotationSpeed;
    private ParticleSystem[] windParticles;

    private void Start()
    {
        direction = transform.up;
        windParticles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        foreach (ParticleSystem wind in windParticles)
        {
            if (wind.isPlaying)
            {
                wind.transform.Rotate(new Vector3(0, 0, rotationSpeed));
            }
        }
    }
}
