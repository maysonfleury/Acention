using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public float magnitude;
    public Vector3 direction;

    private void Start() {
        direction = transform.up;
    }
}
