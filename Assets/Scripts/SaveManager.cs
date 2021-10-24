using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    RigidBodyMovement player;
    UIManager ui;

    void Start()
    {
        player = FindObjectOfType<RigidBodyMovement>();
        ui = FindObjectOfType<UIManager>();

        try
        {
            GameData data = SaveSystem.LoadGameState();

            player.gameStart = data.gameStart;

            ui.area1_discovered = data.area1_discovered;
            ui.area2_discovered = data.area2_discovered;
            ui.area3_discovered = data.area3_discovered;

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
