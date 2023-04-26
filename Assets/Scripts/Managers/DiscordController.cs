using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.UI;
using System;

public class DiscordController : MonoBehaviour
{
    public Discord.Discord discord;

    private string _area;
    private string _state;
    private long _startTime;

    void Start()
    {
        discord = new Discord.Discord(1100506911234854952, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        var activityManager = discord.GetActivityManager();
        _startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var activity = new Discord.Activity {
            Name = "Acention",
            Details = "In the Menus",
            State = "Beginning the Journey",
            Assets = {LargeImage = "acentionlogo"},
            Timestamps = {Start = _startTime},
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok)
            {
                Debug.Log("Discord status set!");
            } else {
                Debug.LogError("Discord status failed to set!");
            }
        });
    }

    public void SetArea(string area)
    {
        switch (area)
        {
            case "Area 0":
                _area = "Beneath the Bridge";
                break;
            case "Area 1":
                _area = "Traversing the World Tree";
                break;
            case "Area 2":
                _area = "Climbing the Crystal Core";
                break;
            case "Area 3":
                _area = "Soaring through the Sacred Spire";
                break;
            case "Area 4":
                _area = "The True Core";
                break;
        }

        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity {
            Name = "Acention",
            Details = _area,
            State = _state,
            Assets = {LargeImage = "acentionlogo"},
            Timestamps = {Start = _startTime},
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok)
            {
                Debug.Log("Discord status set!");
            } else {
                Debug.LogError("Discord status failed to set!");
            }
        });
    }

    public void SetStatus(string status)
    {
        switch (status)
        {
            case "60":
                _state = "Learning the Ropes";
                break;
            case "45":
                _state = "Gaining Speed";
                break;
            case "30":
                _state = "Feeling the Flow";
                break;
            case "10":
                _state = "Approaching Deaths Door";
                break;
            case "5":
                _state = "In Despair";
                break;
            case "loss":
                _state = "Taken by the Tree";
                break;
            case "win":
                _state = "Ascended Beyond";
                break;
        }

        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity {
            Name = "Acention",
            Details = _area,
            State = _state,
            Assets = {LargeImage = "acentionlogo"},
            Timestamps = {Start = _startTime},
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok)
            {
                Debug.Log("Discord status set!");
            } else {
                Debug.LogError("Discord status failed to set!");
            }
        });
    }

    void Update()
    {
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }
    }
}
