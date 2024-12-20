using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    RigidBodyMovement player;
    UIManager ui;
    [SerializeField] bool DeleteSave;

    void Start()
    {
        player = FindObjectOfType<RigidBodyMovement>();
        ui = FindObjectOfType<UIManager>();

        try
        {
            GameData data = SaveSystem.LoadGameState();

            player.gameStart = data.gameStart;
            ui.timeRemaining = data.timeRemaining;

            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];

            player.transform.position = position;
        }
        catch
        {
            Debug.Log("[SaveManager]: No Game save file detected, starting new game.");
        }

        #if UNITY_EDITOR
            if(DeleteSave)
                SaveSystem.DeleteGameState();
        #endif
    }
}