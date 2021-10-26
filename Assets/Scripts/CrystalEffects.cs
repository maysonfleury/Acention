using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEffects : MonoBehaviour
{
    float rotationSpeed = 1f;
    private void Update()
    {
        transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
    }

    public void GameEnd()
    {
        rotationSpeed = 10f;
        
    }
}
