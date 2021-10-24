using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public float[] position;

    public bool gameStart;

    public bool area1_discovered;
    public bool area2_discovered;
    public bool area3_discovered;

    public GameData (RigidBodyMovement player, UIManager ui)
    {
        gameStart = player.gameStart;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        area1_discovered = ui.area1_discovered;
        area2_discovered = ui.area2_discovered;
        area3_discovered = ui.area3_discovered;
    }
}
