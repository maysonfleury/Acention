using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEffects : MonoBehaviour
{ 
    float rotationSpeed = 1f;
    [SerializeField] LightningBoltScript lightning;
    private void Update()
    {
        transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
    }

    public void GameEnd()
    {
        rotationSpeed = 300f;
        lightning.Duration = 0.01f;
        lightning.Generations = 8;
        lightning.ChaosFactor = .25f;
    }
}
