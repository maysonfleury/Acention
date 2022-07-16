using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public float[] position;

    public float timeRemaining;

    public bool gameStart;

    public GameData (RigidBodyMovement player, UIManager ui)
    {
        gameStart = player.gameStart;

        timeRemaining = ui.timeRemaining;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
