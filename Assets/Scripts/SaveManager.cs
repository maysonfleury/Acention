using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    RigidBodyMovement player;

    void Start()
    {
        player = FindObjectOfType<RigidBodyMovement>();

        try
        {
            GameData data = SaveSystem.LoadGameState();

            player.gameStart = data.gameStart;

            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];

            player.transform.position = position;
        }
        catch
        {
            Debug.Log("no save file detected");
        }
    }


}
