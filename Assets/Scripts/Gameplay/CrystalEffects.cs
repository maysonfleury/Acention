using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEffects : MonoBehaviour
{ 
    float rotationSpeed = 1f;
    bool gameOver;
    RigidBodyMovement player;
    [SerializeField] LightningBoltScript lightning;

    private void Awake()
    {
        player = FindObjectOfType<RigidBodyMovement>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
        if (gameOver)
        {           
            float dist = Vector3.Distance(player.transform.position, transform.position);
            Debug.Log(dist.ToString());
            if (dist < 100f)
            {
                SaveSystem.DeleteGameState();
                Application.Quit();
            }
        }
    }

    public void GameEnd()
    {
        rotationSpeed = 300f;
        lightning.Duration = 0.01f;
        lightning.Generations = 8;
        lightning.ChaosFactor = .25f;
        gameOver = true;
    }
}
