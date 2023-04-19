using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigedron : MonoBehaviour
{
    [SerializeField] private float amplitude;
    [SerializeField] private float speed;
    [SerializeField] private float delay;

    private Vector3 startPosition;


    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Moves the Bigedron up and down in a sine wave
        float y = Mathf.Sin((Time.time + delay) * speed) * amplitude;
        transform.position = startPosition + new Vector3(0f, y, 0f);
    }
}
